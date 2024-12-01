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
    private List<GameObject> playerList = new List<GameObject>();

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

    private void GameStart()
    {
        // 플레이어 위치 할당 후 생성
        playerSpawnPoint = GetPlayerSpawnPosition();

        GameObject playerObject_PhotonObject = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint.Item1, playerSpawnPoint.Item2);
        playerObject_PhotonObject.transform.SetParent(playerContainer);
        playerList.Add(playerObject_PhotonObject);
        
        // 게임이 시작될 때 MasterClient에서 작업 필요한 내용
        if(PhotonNetwork.IsMasterClient)
        {
            pv.RPC("StartGameRPC", RpcTarget.Others);
        }
    }

    private (Vector3, Quaternion) GetPlayerSpawnPosition()
    {
        float angularStart = (360.0f / 8f) * PhotonNetwork.LocalPlayer.GetPlayerID();   // CustomProperty 플레이어 고유 ID
        float x = 5.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
        float z = 5.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);

        // Vector3 playerSpawnPosition = new Vector3(x, 0f, z);
        Vector3 playerSpawnPosition = new Vector3(0f, 0f, 0f);
        Quaternion playerSpawnRotation = Quaternion.Euler(0.0f, angularStart, 0.0f);

        return (playerSpawnPosition, playerSpawnRotation);
    }

    [PunRPC]
    private void StartGameRPC()
    {
        // StartGame 시 Local Players 쪽에서 호출되는 내용(호스트 제외)
        Debug.Log("This Player is Local Player >> Game Start !!");
    }

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
