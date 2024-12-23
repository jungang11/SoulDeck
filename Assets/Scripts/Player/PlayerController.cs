using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 플레이어 상태
    public enum PlayerState
    {
        None,    // 게임 시작 시 초기화
        Moving,  // 움직이는 중 (게임 플레이 전)
        Playing, // 게임 플레이 중
    }
    private PlayerState currentState;

    public PlayerState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            OnStateChanged(currentState);
        }
    }
    
    private PhotonView pv;

    [Header("Player Data")]
    private PlayerData _playerData;
    public PlayerData PlayerData
    {
        get => _playerData;
        set => _playerData = value;
    } 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Input System And Move Controller")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerMove moveController;
    
    [Header("Additional Player Controller")]
    [SerializeField] private PlayerCamera cameraController;

    [SerializeField] private PlayerHUD playerHUD;
    
    private void Awake()
    {
        OnInitComponents();     // Components 초기화

        PlayerSettings();
    }

    private void Start()
    {
        SetState(PlayerState.Moving);

        InitDataSetting();
    }

    private void InitDataSetting()
    {
        // Local 정보
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        string playerName = PhotonNetwork.LocalPlayer.NickName;
        
        PlayerData = new PlayerData(playerID, playerName);
        
        GameManager.Data.SetPlayer(this);
    }

    private void OnInitComponents()
    {
        pv = GetComponent<PhotonView>();
        moveController = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void PlayerSettings()
    {
        PlayerMoveSetting();
        PlayerCameraSetting();
        PlayerInputSetting();
    }

    private void PlayerMoveSetting()
    {
        moveController.SetMoveSpeed(moveSpeed);
    }

    private void PlayerCameraSetting()
    {
        cameraController.SetMouseSetting(mouseSensitivity);
    }

    private void PlayerInputSetting()
    {
        // 이 객체가 로컬 플레이어가 아니라면 입력 비활성화
        if (playerInput != null && !pv.IsMine)
        {
            playerInput.enabled = false;
        }
    }

    public void UpdatePlayerHUD()
    {
        LogApi.Log("[PlayerController] >> UpdatePlayerHUD");
        playerHUD.UpdateCardHolder();
    }
    
    public void SetState(PlayerState newState)
    {
        CurrentState = newState;
    }

    public void OnStateChanged(PlayerState newState)
    {
        LogApi.Log($"OnStateChanged >> {newState}");
        switch (newState)
        {
            case PlayerState.None:
                break;
            case PlayerState.Moving:
                OnMoving();
                break;
            case PlayerState.Playing:
                OnPlaying();
                break;
        }
    }

    public void OnMoving()
    {
        // 카메라 Setting -> 정면
        
        
        // Enable Player Move
        moveController.IsMoving = true;
        
        // Player HUD
        playerHUD.HidePlayerHUD();
        
        // 카메라 좌우 이동 등 설정
        // cameraController.SetLockState(CursorLockMode.Locked);
        cameraController.SetLockState(CursorLockMode.None);
        cameraController.IsLooking = false;
    }

    public void OnPlaying()
    {
        GameManager.Data.SetPlayer(this);
        
        // 1. 카메라 Setting -> Top Table
        
        
        // 2. Player Input 비활성화 -> Move 하지 않도록
        moveController.IsMoving = false;
        
        // 3. Player HUD 표시
        playerHUD.ShowPlayerHUD();
        
        // 4. Camera LockState 해제 후 Player HUD 상호작용 가능하도록 변경
        cameraController.SetLockState(CursorLockMode.None);
        cameraController.IsLooking = false;

        // + 추가로 카메라 위치 초기화 (Top table이지만 임시로 정면 보도록)
    }
}