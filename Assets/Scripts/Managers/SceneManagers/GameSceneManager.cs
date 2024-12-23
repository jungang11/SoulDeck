using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        Ready,  // 게임 시작 전
        Game,   // 게임 중
    }

    [Header("플레이어 생성 및 관리")]
    [SerializeField] private GameObject playerPrefab;      // 플레이어 프리팹
    [SerializeField] private Transform playerContainer;
    private (Vector3, Quaternion) playerSpawnPoint;        // 플레이어 생성 위치, 각도 Tuple
    [SerializeField] private List<Player> playerList = new List<Player>();

    [Header("게임 시작 대기")]
    [SerializeField] private TMP_Text logInfoText;
    public float countDownTimer = 5;

    [Header("게임 진행 관리 Component")]
    [SerializeField] private GameSceneUIManager gameSceneUIManager; 
    [SerializeField] private CardManager cardManager;
        
    [Header("Lobby Scene Name (String)")]
    [SerializeField] private string lobbySceneName;

    private CancellationTokenSource cts;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        CheckComponents();

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LocalPlayer.SetLoad(true);
        }
    }

    private void CheckComponents()
    {
        if(cardManager == null || gameSceneUIManager == null)
        {
            Debug.LogError("[CheckComponents] >> there is null game Scene Component");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[OnJoinedRoom] >> Set Player({PhotonNetwork.LocalPlayer.NickName}) ID Property >> {PhotonNetwork.LocalPlayer.ActorNumber}");
        PhotonNetwork.LocalPlayer.SetPlayerID(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"[OnDisconnected Player] >> cause: {cause}");
        SceneManager.LoadScene(lobbySceneName);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(lobbySceneName);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey("Load"))
        {
            int loadCount = GetLoadPlayerCount();
            logInfoText.text = $"Waiting for All Players Load ... >> {loadCount} / {PhotonNetwork.PlayerList.Length}";

            if (loadCount == PhotonNetwork.PlayerList.Length && PhotonNetwork.IsMasterClient)
            {
                // Master Client에서 SetLoadTime 으로 RoomProperty 변경 -> Room 인원 전체 이벤트 실행
                PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.ServerTimestamp);
            }
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("LoadTime"))
        {
            cts?.Cancel();
            cts?.Dispose();

            cts = new CancellationTokenSource();
            StartGameCountdownAsync(cts.Token).Forget();
        }
    }

    private async UniTaskVoid StartGameCountdownAsync(CancellationToken token)
    {
        int loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();
        while (countDownTimer > (PhotonNetwork.ServerTimestamp - loadTime) / 1000f)
        {
            int remainTime = (int)(countDownTimer - (PhotonNetwork.ServerTimestamp - loadTime) / 1000f);
            logInfoText.text = $"All Players Loaded, Starting in: {remainTime + 1}";

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        logInfoText.text = "Game Start!";
        GameStart();

        await UniTask.Delay(1000, cancellationToken: token);
        logInfoText.text = "";
    }

    #region GameStart
    // ReSharper disable Unity.PerformanceAnalysis
    private void GameStart()
    {
        // 플레이어 위치 할당 후 생성
        playerSpawnPoint = GetPlayerSpawnPosition();
        
        // Local 정보
        GameObject playerObject_PhotonObject = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint.Item1, playerSpawnPoint.Item2);
        playerObject_PhotonObject.transform.SetParent(playerContainer);

        StartGameLogic();
    }

    private (Vector3, Quaternion) GetPlayerSpawnPosition()
    {
        // 0으로 임시 설정
        Vector3 playerSpawnPosition = new Vector3(0f, 0f, 0f);
        Quaternion playerSpawnRotation = Quaternion.identity;

        return (playerSpawnPosition, playerSpawnRotation);
    }

    private void StartGameLogic()
    {
        LogApi.Log("StartGameLogic !!");

        GetCardAllPlayer();
    }
    #endregion

    #region Card
    public void GetCardAllPlayer()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            // 마스터가 플레이어 리스트 초기화 및 카드 분배
            GameManager.Data.InitializePlayerList();
            GameManager.Data.DistributeCardsToPlayers();
        }
    }

    /// <summary>
    /// parameter cardIndex(card 숫자) 카드를 제출했다 전달
    /// </summary>
    /// <param name="cardIndex"></param>
    public void SubmitCard(int cardIndex)
    {
        if (pv.IsMine)
        {
            pv.RPC("SubmitCard_RPC", RpcTarget.MasterClient, cardIndex);
        }
    }

    [PunRPC]
    public void SubmitCard_RPC(int cardIndex)
    {
        LogApi.Log($"SubmitCard_RPC 제공받음 >> index: {cardIndex}");
        // TODO 1: MasterClient에서 SubmitCard 호출한 Player의 Card List에서 제출한 카드를 제거
        // TODO 2: 제출한 카드가 올바른 숫자인지(순서에 맞는지) MasterClient에서 확인
    }
    #endregion

    public int GetLoadPlayerCount()
    {
        return PhotonNetwork.PlayerList.Count(player => player.GetLoad());
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
