using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour, IPunObservable
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
            rb.MovePosition(rb.position + m_moveSpeed * Time.fixedDeltaTime * moveDir.normalized);
        }
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        m_moveSpeed = moveSpeed;
    }

    private void OnMove(InputValue value)
    {
        if (pv.IsMine && value != null)
        {
            Vector2 inputDir = value.Get<Vector2>();
            moveDir = new Vector3(inputDir.x, 0, inputDir.y);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if (stream.IsWriting)
        // {
        //     stream.SendNext(moveDir);
        // }
        // else
        // {
        //     moveDir = (Vector3)stream.ReceiveNext();
        // }
    }
}
