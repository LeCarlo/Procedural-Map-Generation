using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody rb;
    public float speed;
    public float ShiftSpeed;
    float oldSpeed;
    public float rotSpeed;
    public bool hasJumped;
    public int jumpHeight;
    public bool isGrounded;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrounded = true;
        hasJumped = false;
        oldSpeed = speed;
    }
    private void FixedUpdate()
    {
        if (hasJumped && isGrounded)
        {
            isGrounded = false;
            hasJumped = false;
            rb.AddForce(Vector3.up * jumpHeight);
        }
    }
    void Update()
    {
        // Rotate Left
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(-Vector3.up * rotSpeed);
        }
        // Rotate Right
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up * rotSpeed);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            hasJumped = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            oldSpeed = speed;
            speed = ShiftSpeed;
        }
        else
        {
            speed = oldSpeed;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
            hasJumped = false;
        }
    }
}
