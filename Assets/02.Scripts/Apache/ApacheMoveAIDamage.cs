using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Tank hp가 0 이하일 때 잠시 meshRenderer를 비활성화 하여 5초 후에 다시 활성화

public class ApacheMoveAIDamage : MonoBehaviourPun
{
    public GameObject explosionPrefab;

    void Start()
    {
        explosionPrefab = Resources.Load<GameObject>("Explosion");
    }

    public void OnDamage()
    {
        photonView.RPC(nameof(OnDamagePun), RpcTarget.All);
    }

    [PunRPC]
    public void OnDamagePun()
    {
        StartCoroutine(ExplosionApache());
    }

    [PunRPC]
    private IEnumerator ExplosionApache()
    {
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            Object expEff = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(expEff, 2.0f);
            PhotonNetwork.Destroy(gameObject);
        }
        yield return new WaitForSeconds(0.1f);
    }

}
