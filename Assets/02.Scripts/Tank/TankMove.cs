using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

// A : 왼쪽으로 회전, D : 오른쪽으로 회전, W : 전진, S : 후진

public class TankMove : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private Transform tr;
    private float tankBodyAngle = 8f;
    private float tankMoveSpeed = 20f;
    private Vector3 moveforward;
    private Vector3 movebackward;

    private Vector3 curPos = Vector3.zero;              // 동기화된 위치값
    private Quaternion curRot = Quaternion.identity;    // 동기화된 회전값

    private Vector2 dir;
    
    void Awake()
    {
        photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;        // PhotonView의 동기화 방식 설정
        photonView.ObservedComponents[0] = this;                            // PhotonView가 관찰할 컴포넌트 설정
        
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        tr = GetComponent<Transform>();

        curPos = tr.position;
        curRot = tr.rotation;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)          // 자신의 탱크일 경우에만 조작 가능
        {
            moveforward = new Vector3(transform.forward.x * tankMoveSpeed, rb.velocity.y, transform.forward.z * tankMoveSpeed);
            movebackward = new Vector3(-transform.forward.x * tankMoveSpeed, rb.velocity.y, -transform.forward.z * tankMoveSpeed);

            /*
            if (Input.GetKey(KeyCode.W))
                rb.velocity = moveforward;
            if (Input.GetKey(KeyCode.S))
                rb.velocity = movebackward;

            if (Input.GetKey(KeyCode.A))
            {
                Quaternion targetRotation = Quaternion.Euler(0, -tankBodyAngle, 0) * tr.rotation;
                tr.rotation = Quaternion.Lerp(tr.rotation, targetRotation, Time.deltaTime * tankBodyAngle);
            }
            if (Input.GetKey(KeyCode.D))
            {
                Quaternion targetRotation = Quaternion.Euler(0, tankBodyAngle, 0) * tr.rotation;
                tr.rotation = Quaternion.Lerp(tr.rotation, targetRotation, Time.deltaTime * tankBodyAngle);
            } */
            
            if (dir.y > 0)
                rb.velocity = moveforward;
            if (dir.y < 0)
                rb.velocity = movebackward;

            if (dir.x < 0)
            {
                Quaternion targetRotation = Quaternion.Euler(0, -tankBodyAngle, 0) * tr.rotation;
                tr.rotation = Quaternion.Lerp(tr.rotation, targetRotation, Time.deltaTime * tankBodyAngle);
            }
            if (dir.x > 0)
            {
                Quaternion targetRotation = Quaternion.Euler(0, tankBodyAngle, 0) * tr.rotation;
                tr.rotation = Quaternion.Lerp(tr.rotation, targetRotation, Time.deltaTime * tankBodyAngle);
            }
        }
        else                            // 다른 플레이어의 탱크일 경우에는 동기화된 위치와 회전값으로 이동
        {
            tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 10.0f);
            tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 10.0f);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)      // PhotonView가 관찰하는 컴포넌트의 상태를 동기화하는 콜백함수
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if (stream.IsReading)
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
