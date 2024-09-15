using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveRecordTest : MonoBehaviour
{
    private GameObject explosion;
    private Transform tr;
    private Rigidbody rb;
    private float speed = 5000f;
    private float timer = 3.0f;

    void Start()
    {
        explosion = Resources.Load<GameObject>("BigExplosionEffect 1");
        tr = transform;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(tr.forward * speed);
        //Invoke(nameof(Destroy), timer);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Terrain"))
        {
            Destroy();
        }
    }

    void Destroy()
    {
        PlayExplosionEffect();

    }


    void PlayExplosionEffect()
    {
        float randomYRotation = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);
        Instantiate(explosion, tr.position, randomRotation);
        
        Destroy(gameObject);
    }

}
