using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Ray를 쏘아 마우스 포지션으로 따라

public class TankTurret : MonoBehaviourPun, IPunObservable
{
    private Transform tr;
    private Quaternion curRot = Quaternion.identity;    // 동기화된 회전값


    void Awake()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable;        // PhotonView의 동기화 방식 설정
        photonView.ObservedComponents[0] = this;                            // PhotonView가 관찰할 컴포넌트 설정
        
        tr = GetComponent<Transform>();
        curRot = tr.localRotation;
    }

    void Update()
    {
        if (photonView.IsMine)          // 자신의 터렛일 경우에만 조작 가능
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // 메인 카메라에서 마우스 포지션으로 Ray를 쏨
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 2000f, Color.green);
            if (Physics.Raycast(ray, out hit, 2000f, 1 << 8))
            {
                Vector3 relative = tr.InverseTransformPoint(hit.point);
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                tr.Rotate(0f, angle * Time.deltaTime * 5f, 0f);
            }
        }
        else                            // 다른 플레이어의 터렛일 경우에는 동기화된 위치와 회전값으로 이동
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * 10.0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.localRotation);
        }
        else
        {
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
