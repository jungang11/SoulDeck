using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;

    [Header("Player Data")]
    [SerializeField] private PlayerData currentPlayerData;
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
        if (!pv.IsMine)
        {
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
        }
    }
}