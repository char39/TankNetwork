using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApacheMoveFix : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tr;
    private Quaternion originalRotation;
    private float moveSpeed = 0;
    private const float ApplyMoveSpeed = 0.02f;
    private float rotSpeed = 0;
    private const float ApplyRotSpeed = 0.02f;
    private float verticalSpeed = 0;
    private const float ApplyVerticalSpeed = 0.1f;
    
    private float recoverRotSpeed = 0.8f;
    private float maxRotationX = 25.0f;
    private float maxRotationZ = 25.0f;
    private bool isGrounded = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        originalRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (Player.instance == null)
            return;
        if (Player.instance.playerInside)
        {
            ApacheRotate();
            ApacheMoveHorizontal();
            ApacheMoveVertical();
        }
    }

    void Update()
    {
        GetOut();
    }



    private void ApacheRotate()
    {
        if (Input.GetKey(KeyCode.A))
            rotSpeed += -ApplyRotSpeed;
        else if (Input.GetKey(KeyCode.D))
            rotSpeed += ApplyRotSpeed;
        else
        {
            if (rotSpeed > 0f) rotSpeed += -ApplyRotSpeed;
            else if (rotSpeed < 0f) rotSpeed += ApplyRotSpeed;
        }
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
    }
    private void ApacheMoveHorizontal()
    {
        if (Input.GetKey(KeyCode.W))
            moveSpeed += ApplyMoveSpeed;
        else if (Input.GetKey(KeyCode.S))
            moveSpeed += -ApplyMoveSpeed;
        else
        {
            if (moveSpeed > 0f) moveSpeed += -ApplyMoveSpeed;
            else if (moveSpeed < 0f) moveSpeed += ApplyMoveSpeed;
        }
        tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
    }
    private void ApacheMoveVertical()
    {
        if (Input.GetKey(KeyCode.C))
            verticalSpeed += ApplyVerticalSpeed;
        else if (Input.GetKey(KeyCode.Z))
            verticalSpeed += -ApplyVerticalSpeed;
        else
        {
            if (verticalSpeed > 0f) verticalSpeed += -ApplyVerticalSpeed;
            else if (verticalSpeed < 0f) verticalSpeed += ApplyVerticalSpeed;
        }
        tr.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.Self);
    }

    private void GetOut()
    {
        if (isGrounded && Input.GetKey(KeyCode.Q))
        {
            if (Player.instance == null)
                return;
            Player.instance.transform.position = transform.position + new Vector3(transform.position.x - 10f, 5f, transform.position.z);
            Player.instance.playerInside = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
            isGrounded = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            Player.instance.playerInside = true;
    }

}
