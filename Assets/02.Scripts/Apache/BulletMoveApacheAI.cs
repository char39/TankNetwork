using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveApacheAI : MonoBehaviour
{
    private GameObject explosion;
    private Transform tr;
    private Rigidbody rb;
    private float speed = 10000f;

    void Start()
    {
        explosion = Resources.Load<GameObject>("BigExplosionEffect");
        tr = transform;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(tr.forward * speed);
        Invoke("Destroy", 3f);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!coll.gameObject.CompareTag("Apache"))
        {
            Destroy();
        }
    }

    void Destroy()
    {
        float randomYRotation = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);

        GameObject ex = Instantiate(explosion, tr.position, randomRotation);
        ex.transform.SetParent(GameObject.Find("Effects").transform);
        Destroy(gameObject);
    }

}
