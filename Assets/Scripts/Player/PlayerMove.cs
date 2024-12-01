using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;
    private PhotonView pv;

    [SerializeField] private float m_moveSpeed;
    private Vector3 moveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        pv = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Move();
        }
    }

    public void SetMoveSpeed(float _moveSpeed)
    {
        m_moveSpeed = _moveSpeed;
    }

    private void Move()
    {
        rb.MovePosition(rb.position + m_moveSpeed * Time.fixedDeltaTime * moveDir);
    }

    private void OnMove(InputValue value)
    {
        if (pv.IsMine && value != null)
        {
            Vector2 inputDir = value.Get<Vector2>();
            moveDir = transform.TransformDirection(new Vector3(inputDir.x, 0, inputDir.y).normalized);
        }
    }
}
