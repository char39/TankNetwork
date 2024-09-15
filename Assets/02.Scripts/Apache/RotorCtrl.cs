using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorCtrl : MonoBehaviour
{
    private float rotSpeed = 777f;
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
