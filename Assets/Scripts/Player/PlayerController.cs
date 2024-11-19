using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;

    [Header("Player Data")]
    [SerializeField] private PlayerData currentPlayerData;
    [SerializeField] private float m_PlayerMoveSpeed;

    [Header("Input System And Move Controller")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerMove moveController;
    [SerializeField] private PlayerCamera cameraController;

    private void Awake()
    {
        OnInitComponents();     // Components 초기화

        PlayerSettings();
    }

    private void OnInitComponents()
    {
        moveController = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
        pv = GetComponent<PhotonView>();
    }

    private void PlayerSettings()
    {
        PlayerMoveSetting();
        PlayerInputSetting();
    }

    private void PlayerMoveSetting()
    {
        moveController.SetMoveSpeed(m_PlayerMoveSpeed);
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