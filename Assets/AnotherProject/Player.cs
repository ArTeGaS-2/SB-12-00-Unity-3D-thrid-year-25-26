using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float turnSpeed = 180f;
    public float walkSpeed = 5f;
    public float sprintMult = 1.6f;
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        float curSpeed = walkSpeed * (sprint ? sprintMult : 1f);
        Vector3 dir = (transform.right * x + transform.forward * z).normalized;
        Vector3 v = dir * curSpeed;
        rb.velocity = new Vector3(v.x, rb.velocity.y, v.z);
    }
}