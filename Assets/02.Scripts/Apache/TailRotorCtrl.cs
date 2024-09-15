using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailRotorCtrl : MonoBehaviour
{
    private float rotSpeed = 1444f;
    private Transform tr;

    void Start()
    {
        tr = transform;
    }

    void FixedUpdate()
    {
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * -1);
    }
}