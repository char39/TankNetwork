using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletMove : MonoBehaviourPun
{
    private GameObject explosion;
    private Transform tr;
    private Rigidbody rb;
    private float speed = 5000f;
    private float timer = 3.0f;

    void Start()
    {
        explosion = Resources.Load<GameObject>("BigExplosionEffect");
        tr = transform;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(tr.forward * speed);
        Invoke(nameof(Destroy), timer);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Terrain"))
        {
            photonView.RPC(nameof(Destroy), RpcTarget.MasterClient);
        }
        if (coll.gameObject.CompareTag("Apache"))
        {
            coll.gameObject.SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);
            photonView.RPC(nameof(Destroy), RpcTarget.MasterClient);
        }
        if (coll.gameObject.CompareTag("Tank"))
        {
            coll.gameObject.transform.parent.parent.SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);
            photonView.RPC(nameof(Destroy), RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void Destroy()
    {
        photonView.RPC(nameof(PlayExplosionEffect), RpcTarget.All);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void PlayExplosionEffect()
    {
        if (photonView.IsMine)
        {
            float randomYRotation = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);
            GameObject explosionInstance = Instantiate(explosion, tr.position, randomRotation);
            Destroy(explosionInstance, 2.0f);
        }
    }

}
