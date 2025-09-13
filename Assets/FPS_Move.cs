using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Move : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -1f;

    [Header("Mouse Look")]
    public Transform cameraPivot;      // Поворот камери
    public float mouseSensitivity = 180f;
    public float maxLookAngle = 90f;   // у градусах

    private CharacterController cc;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        LookAround();
        MoveCharacter();
    }

    void LookAround()
    {
        float mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseVertical = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        RotateHorizontally(mouseHorizontal);
        RotateVertically(mouseVertical);
    }

    void RotateHorizontally(float mouseHorizontal)
    {
        transform.Rotate(0, mouseHorizontal, 0);
    }

    void RotateVertically(float mouseVertical)
    {
        xRotation -= mouseVertical;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    void MoveCharacter()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // Гравітація (тільки Y)
        velocity.y += gravity * Time.deltaTime;

        // Сумуємо горизонтальний рух і вертикальний (гравітацію)
        Vector3 totalMove = move * speed + velocity;

        cc.Move(totalMove * Time.deltaTime);
    }
}