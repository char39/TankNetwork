using System.Collections;
using System.Collections.Generic;
using Photon.Pun;           // 유니티용 포톤 라이브러리
using Cinemachine;          // 시네머신 라이브러리

public class VirtualCameraLookAt : MonoBehaviourPun
{
    CinemachineVirtualCamera followCam;

    void Start()
    {
        followCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (photonView.IsMine)              // 내 캐릭터일 경우. photonView.IsMine은 현재 로컬 플레이어가 이 객체를 컨트롤하는지 여부를 나타내는 bool 값
        {
            followCam.LookAt = transform;
        }
    }
}
