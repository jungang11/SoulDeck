using Unity.Cinemachine;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PhotonView pv;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private CinemachineCamera firstPersonCamera;    // 플레이어의 1인칭 카메라

    private float mouseSensitivity  = 100f; // 마우스 감도
    private Vector2 lookDelta;
    private float xRotation; // 상하 회전값
    private float yRotation; // 좌우 회전값

    private bool isLooking = false;
    public bool IsLooking { get { return isLooking; } set { isLooking = value; } }
    
    private void Awake()
    {
        if (pv.IsMine)
        {
            // 로컬 플레이어의 카메라 활성화
            firstPersonCamera.gameObject.SetActive(true);
        }
        else
        {
            // 원격 플레이어의 카메라 비활성화
            firstPersonCamera.gameObject.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        if (pv.IsMine)
        {
            IsLooking = true;
            SetLockState(CursorLockMode.Locked);
        }
    }

    private void OnDisable()
    {
        if (pv.IsMine)
            SetLockState(CursorLockMode.None);
    }

    public void SetLockState(CursorLockMode mode)
    {
        Cursor.lockState = mode;
    }

    private void LateUpdate()
    {
        if (pv.IsMine && lookDelta != Vector2.zero)
            Look();
    }

    public void SetMouseSetting(float _mouseSensitivity)
    {
        mouseSensitivity  = _mouseSensitivity;
    }

    private void Look()
    {
        yRotation += lookDelta.x * mouseSensitivity * Time.deltaTime;
        xRotation -= lookDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // 카메라 상하 회전
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // 플레이어 좌우 회전
        gameObject.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
    
    // pointer - delta 를 이용해 Vector2 값을 받아옴
    private void OnLook(InputValue value)
    {
        if (value != null && IsLooking)
        {
            lookDelta = value.Get<Vector2>();
        }
    }
}