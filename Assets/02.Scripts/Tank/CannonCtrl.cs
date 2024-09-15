using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CannonCtrl : MonoBehaviourPun, IPunObservable
{
    private Transform firePostr;
    private GameObject bullet;
    public float rotSpeed = 100f;       // 회전 속도
    public float upperAngle = -30f;     // 최대 각도
    public float downAngle = 0f;        // 최소 각도
    public float currentRotate = 0f;    // 현재 각도

    private Quaternion curRot = Quaternion.identity;    // 동기화된 회전값

    private bool valueBool = false;
    
    void Awake()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable;        // PhotonView의 동기화 방식 설정
        photonView.ObservedComponents[0] = this;                            // PhotonView가 관찰할 컴포넌트 설정
        
        firePostr = transform.GetChild(0).GetChild(0).transform;

        curRot = transform.localRotation;
    }

    void Start()
    {
        bullet = Resources.Load<GameObject>("Bullet");
    }

    void Update()
    {
        CannonAngle();
        CannonFire();
    }

    private void CannonAngle()
    {
        if (photonView.IsMine)
        {
            float wheel = -Input.GetAxis("Mouse ScrollWheel");      // 마우스 휠 입력
            float angle = wheel * rotSpeed * Time.deltaTime;        // 회전 각도 계산
            if (wheel < 0)
            {
                currentRotate += angle;
                if (currentRotate > upperAngle)
                    transform.Rotate(angle, 0f, 0f);
                else
                    currentRotate = upperAngle;
            }
            else if (wheel > 0)
            {
                currentRotate += angle;
                if (currentRotate < downAngle)
                    transform.Rotate(angle, 0f, 0f);
                else
                    currentRotate = downAngle;
            }
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, curRot, Time.deltaTime * 10.0f);
        }
    }

    private void CannonFire()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (HoverEvent.instance.isEnter) return;
        
        if (valueBool && photonView.IsMine)
        {
            //Fire();     // 자신의 탱크일 경우에만 발사
            valueBool = false;
            photonView.RPC(nameof(Fire), RpcTarget.MasterClient);     // 다른 플레이어에게 발사 이벤트 전달
        }
    }

    private void OnFire(InputValue value)
    {
        if (value.isPressed)
            valueBool = true;
    }

    [PunRPC]
    private void Fire()
    {
        PhotonNetwork.Instantiate(bullet.name, firePostr.position, firePostr.rotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.localRotation);
        }
        else
        {
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
