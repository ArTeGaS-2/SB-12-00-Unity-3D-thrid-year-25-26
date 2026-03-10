using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 20f;

    private void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
        rb.velocity = transform.forward * speed;
    }
}
