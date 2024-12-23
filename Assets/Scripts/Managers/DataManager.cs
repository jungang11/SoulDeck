using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

// 현재 Room 안의 플레이어 관리
// 현재 Play 중인 게임의 카드 정보 관리
public class DataManager : MonoBehaviour
{
    public List<PlayerData> currentRoomPlayerList = new List<PlayerData>(); // 현재 방 인원 Player 객체 List
    
    // Master Data
    // 전체 카드 -> 책상에 올려져있는 카드들 -> 마지막 카드만 판단
    // 플레이어들 보유 카드 -> 가지고 있는 모든 카드
    // 승리 : 플레이어들 보유 카드 합이 0, Life > 0
    // 패배 : 플레이더들 보유 카드 합 > 0, Life <= 0
    // 계산은 Master Client 역할
    public Dictionary<int, List<int>> playerCardDict = new(); // 플레이어 id - 가지고 있는 cardIndexList. Master가 관리할 데이터
    public int currentLife;                             // 공용 Life
    public List<int> cardListOnTable = new List<int>(); // 책상 위 올려져있는 카드 리스트
    public List<int> allCards = new List<int>();        // Room 내 유저들이 가지고 있는 모든 Card Index List
    public int lastCardIndex;                           // 가장 최근에 Submit한 Card Index
    public List<int> getCardList = new List<int>();     // 유저들 제공 위해 마스터가 계산한 카드 목록
    
    // Local Data
    public PlayerController player;
    private int[] localCardIndexArray;
    public int[] LocalCardIndexArray => localCardIndexArray;
    
    System.Random rand = new System.Random();
    
    private int currentGameLevel; // 현재 진행중인 게임 레벨
    public int CurrentGameLevel => currentGameLevel;

    public PhotonView photonView;

    private void Awake()
    {
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
            
            if (photonView.ViewID != 991)
                photonView.ViewID = 991;
        }
    }

    private void Start()
    {
        currentGameLevel = 3;
        currentLife = currentGameLevel;
        localCardIndexArray = new int[CurrentGameLevel];
        
        InitializePlayerList();
    }

    #region Getcard_MasterClient
    // 현재 방에 있는 플레이어 리스트 초기화 (마스터 전용)
    public void InitializePlayerList()
    {
        currentRoomPlayerList.Clear();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            currentRoomPlayerList.Add(new PlayerData(player.ActorNumber, player.NickName));
        }
    }
    
    /// <summary>
    /// 카드 분배 로직 (마스터 클라이언트)
    /// </summary>
    public void DistributeCardsToPlayers()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int totalCards = currentRoomPlayerList.Count * currentGameLevel;
        allCards = GenerateUniqueCardList(totalCards);

        for (int i = 0; i < currentRoomPlayerList.Count; i++)
        {
            List<int> playerCards = allCards.Skip(i * currentGameLevel).Take(currentGameLevel).ToList();
            currentRoomPlayerList[i].CardIndexList = playerCards;
            playerCardDict[i] = playerCards;

            // 각 플레이어에게 카드 전달
            photonView.RPC(nameof(SyncPlayerCards), PhotonNetwork.PlayerList[i], playerCards.ToArray());
        }
    }

    public void UpdatePlayerCardDict(int playerID, List<int> playerCards)
    {
        playerCardDict[playerID] = playerCards;
    }

    /// <summary>
    /// 카드 상태 동기화
    /// </summary>
    [PunRPC]
    public void SyncPlayerCards(int[] cards)
    {
        LogApi.Log($"카드 받음: {string.Join(", ", cards)}");
        localCardIndexArray = cards; // 할당 수정 필요
        
        player.UpdatePlayerHUD();
    }

    /// <summary>
    /// 중복 없는 숫자 리스트 생성
    /// </summary>
    private List<int> GenerateUniqueCardList(int count)
    {
        HashSet<int> uniqueNumbers = new HashSet<int>();
        while (uniqueNumbers.Count < count)
        {
            uniqueNumbers.Add(rand.Next(1, 101));
        }
        return uniqueNumbers.ToList();
    }
    #endregion

    #region SubmitCard
    // 1. PlayerHUD 버튼 클릭 이벤트에서 바로 호출할 수 있도록 SubmitCard() 메서드 구현, 어차피 DataManager는 싱글톤
    // 2. PlayerHUD Card Button 이벤트에서 SubmitCard(int cardIndex)로 card Index를 보내줌
    // 3. local card List 에서 Index 위치에 있는 Card를 제출한 것으로 수행(List에서 제거 -> 배열로 구현하는게 나을수도?, 포톤 parameter는 array만 되는 이유도 있음)
    // 3-1. localCardIndexArray에서 해당 Index 제거
    // 3-2. Index 제거 후 UpdateCardHolder(이건 playerHUD에서 Button 클릭 이벤트 시 수행하도록 추가)
    // 4. Target.Master로 RPC 함수 (SubmitCard_RPC)
    // 5. Master에서 계산하는 로직 구현 (CheckSubmitCard() 로 하면 되나?)
    // 6. 해당 메서드에서 계산 후 RPC로 모두에게 결과 처리 내용 뿌리기. -> prevCardIndex 업데이트 후 뿌리기.
    public void SubmitCard(int cardIndex)
    {
        // TODO. PlayerHUD 버튼 parameter 0부터 보내는걸로 변경 필요
        int cardToSubmitIndex = cardIndex - 1;
        
        // 제출 카드가 로컬 리스트에 있는지 확인
        if (cardToSubmitIndex < 0 || cardToSubmitIndex >= localCardIndexArray.Length)
        {
            LogApi.LogError("Invalid card index");
            return;
        }

        int cardToSubmit = localCardIndexArray[cardToSubmitIndex]; // param으로 1부터 보내고 있음
        LogApi.Log($"내려는 카드 index:{cardToSubmitIndex}, 카드: {cardToSubmit}, 카드 리스트 {string.Join(",",localCardIndexArray)}");

        // 로컬 리스트에서 카드 제거
        localCardIndexArray[cardToSubmitIndex] = -1;    // 비어있음을 의미

        // MasterClient로 카드 제출 요청
        photonView.RPC(nameof(SubmitCard_RPC), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, cardToSubmit);
    }
    
    [PunRPC]
    public void SubmitCard_RPC(int playerActorNumber, int submittedCard)
    {
        LogApi.Log($"Player {playerActorNumber} submitted card: {submittedCard}");
        
        cardListOnTable.Add(submittedCard);

        photonView.RPC(nameof(UpdateCardState), RpcTarget.All, lastCardIndex);

        CheckSubmitCard(submittedCard);
    }
    
    [PunRPC]
    public void UpdateCardState(int newLastCardIndex)
    {
        lastCardIndex = newLastCardIndex;
        LogApi.Log($"Last card updated: {lastCardIndex}");
        player.UpdatePlayerHUD();
    }

    // TODO. 수정 필요
    // CheckSubmitCard 에서 card가 lastCardIndex보다 큰 것을 비교하는게 아니라, card를 냈을 때
    // allCard 내에 card보다 Index가 낮은 card가 있는지 체크함.
    // 만약 체크 후 낮은 card가 있다? -> 낮은 card 갯수만큼 Life Decrease
    // 그리고 낮은 Card를 가지고 있는 Player의 Local Card List에서 그 카드 제거 필요
    // ** 체크할 사항
    // Check만 Master가 하고, 누가 더 낮은 카드를 가지고 있는지는 Local에서 체크 후 Master에게 보내주는게 낫지 않나?
    // CheckSubmitCard -> [RPC.All]HasLowIndex(여기서 localIndexArray 업데이트) -> [Rpc.Master]낮았던 카드의 숫자와 갯수를 보내줌
    // 이후 Master에서 Life 계산 후 뿌려주기
    // 이 때 Life가 0이하가 된다 -> 패배 / Life가 0보다 크다 -> 지속
    // Life가 0보다 크면서 모든 플레이어의 localCard가 비었다 -> 승리, 레벨 증가 후 게임 시작 -> 카드 재분배
    
    // * local Card가 비었는지 판단하는 내용 -> master에서 현재까지 낸 Card Count를 체크 후, 처음 분배했던 카드의 수와 같아지면 0으로 판단하는 방법 있음
    private void CheckSubmitCard(int submittedCard)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        // allCards에서 submittedCard 보다 작은 카드가 있는지 확인, 이 때 cardListOntable. 이미 제출한 카드는 체크하면 안됨 수정 필요
        List<int> lowerCards = allCards.Where(card => card < submittedCard).ToList();
        if (lowerCards.Count == 0)
        {
            LogApi.Log("lastCardIndex보다 작은 카드 없음");
            return;
        }
        else if (lowerCards.Count > 0)
        {
            LogApi.Log($"Lower cards found: {string.Join(", ", lowerCards)}");
            
            // 여기서 만약 lowerCards Count가 Life보다 클 경우 바로 패배 로직
            if (currentLife > lowerCards.Count)
                GameOver();
            else
                currentLife -= lowerCards.Count;

            foreach (var player in currentRoomPlayerList)
            {
                // TODO. 낮은 카드들 제거 로직
                // 어떻게 보낼지 고민..
                // ** (별로) CheckSubmitCard_RPC로 모두에게 계산해서 가지고 오라고 하기?
                // Master가 Dictionary로 모든 카드 데이터 관리, 낮은 카드가 있는지 현재 메서드에서 Master 내부적으로 바로 체크
                // **Master가 Server역할을 해야함 (Client 동시) Host방식
            }
        }
    }

    [PunRPC]
    public void CheckSubmitCard_RPC()
    {
        
    }

    public void GameWin()
    {
        // TODO. 게임 승리
        // 게임 승리 후 Level++ 한 후 게임 시작 로직 재수행 필요
    }

    [PunRPC]
    public void GameWin_RPC()
    {
        
    }

    // Master에서 게임 오버 로직 수행 후 RPC로 뿌려주기
    public void GameOver()
    {
        // TODO. 데이터 관련 초기화 필요
        // 이후 RPC 뿌리기
        // 게임 패배 UI 표시도 이 부분에서 수행 필요
        
        photonView.RPC(nameof(GameOver_RPC), RpcTarget.All);
    }

    [PunRPC]
    public void GameOver_RPC()
    {
        LogApi.Log("GameOver");
    }
    
    // ++ LocalPlayer에서 각자 계산 => 업데이트 된 prevCardIndex 보다 작은 Index의 Card를 가지고 있다?
    // -> 작은 Index의 Card들을 제거(LocalIndexList), 해당 Count Master가 계산하여 Life Decrease 후 뿌리기

    #endregion

    public void SetPlayer(PlayerController _player)
    {
        player = _player;
    }
    
    public void GetCard()
    {
        InitializePlayerList();
        DistributeCardsToPlayers();
    }
}
