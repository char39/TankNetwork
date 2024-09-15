using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//z는 위, c는 아래
public class ApacheMove : MonoBehaviour
{
    private Rigidbody rb;
    private Quaternion originalRotation;
    private float moveSpeed = 20.0f;
    private float rotSpeed = 44.0f;
    private float recoverRotSpeed = 0.8f;
    private float maxRotationX = 25.0f;
    private float maxRotationZ = 25.0f;
    private bool isGrounded = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        ApacheCtrl();
    }

    void Update()
    {
        GetOut();
    }

    private void ApacheCtrl()
    {
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.UpArrow))    // W 키를 누르면 위로 이동
            {
                if (rb.velocity.y < 0)
                    rb.AddRelativeForce((Vector3.up + Vector3.forward).normalized * moveSpeed * 3f * Time.deltaTime, ForceMode.VelocityChange);
                else
                    rb.AddRelativeForce((Vector3.up + Vector3.forward).normalized * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                isGrounded = false;
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.DownArrow))    // W 키를 누르면 위로 이동
            {
                if (rb.velocity.y < 0)
                    rb.AddRelativeForce((Vector3.up + Vector3.down).normalized * moveSpeed * 3f * Time.deltaTime, ForceMode.VelocityChange);
                else
                    rb.AddRelativeForce((Vector3.up + Vector3.down).normalized * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                isGrounded = false;
            }
            else if (Input.GetKey(KeyCode.W))    // W 키를 누르면 위로 이동
            {
                if (rb.velocity.y < 0)
                    rb.AddRelativeForce(Vector3.up * moveSpeed * 3f * Time.deltaTime, ForceMode.VelocityChange);
                else
                    rb.AddRelativeForce(Vector3.up * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
                isGrounded = false;
            }
            if (Input.GetKey(KeyCode.S))    // S 키를 누르면 아래로 이동
            {
                if (rb.velocity.y > 0)
                    rb.AddRelativeForce(Vector3.down * moveSpeed * 2f * Time.deltaTime, ForceMode.VelocityChange);
                else
                    rb.AddRelativeForce(Vector3.down * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
            }

            if (isGrounded) return;

            if (Input.GetKey(KeyCode.A))    // A 키를 누르면 왼쪽으로 회전
                transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.D))    // D 키를 누르면 오른쪽으로 회전
                transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.UpArrow))      // UpArrow 키를 누르면 앞으로 회전
                transform.Rotate(Vector3.right * rotSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.DownArrow))    // DownArrow 키를 누르면 뒤로 회전
                transform.Rotate(Vector3.left * rotSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftArrow))    // LeftArrow 키를 누르면 왼쪽으로 회전
                transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.RightArrow))   // RightArrow 키를 누르면 오른쪽으로 회전
                transform.Rotate(Vector3.back * rotSpeed * Time.deltaTime);

            if (Mathf.Abs(rb.velocity.x) > 13f && rb.velocity.x > 0)
                rb.velocity = new Vector3(13f, rb.velocity.y, rb.velocity.z);
            else if (Mathf.Abs(rb.velocity.x) > 13f && rb.velocity.x < 0)
                rb.velocity = new Vector3(-13f, rb.velocity.y, rb.velocity.z);
            if (Mathf.Abs(rb.velocity.y) > 13f && rb.velocity.y > 0)
                rb.velocity = new Vector3(rb.velocity.x, 13f, rb.velocity.z);
            else if (Mathf.Abs(rb.velocity.y) > 13f && rb.velocity.y < 0)
                rb.velocity = new Vector3(rb.velocity.x, -13f, rb.velocity.z);
            if (Mathf.Abs(rb.velocity.z) > 13f && rb.velocity.z > 0)
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 13f);
            else if (Mathf.Abs(rb.velocity.z) > 13f && rb.velocity.z < 0)
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -13f);
        

        // 키를 놓으면 원래 회전 상태로 돌아감.
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            Quaternion targetRotation = Quaternion.Euler(originalRotation.eulerAngles.x, transform.rotation.eulerAngles.y, originalRotation.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * recoverRotSpeed);
        }
    }

    private void GetOut()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
            isGrounded = true;
    }
    private void OnTriggerEnter(Collider other)
    {

    }

}
