using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float height = 1.6f;
    public float sensX = 180f;
    public float sensY = 120f;
    public float minPitch = -35f;
    public float maxPitch = 70f;
    float yaw;
    float pitch;
    void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void LateUpdate()
    {
        if (!target) return;
        yaw += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + Vector3.up * height;
        transform.position = pivot + rot * (Vector3.back * distance);
        transform.rotation = rot;
    }
}
