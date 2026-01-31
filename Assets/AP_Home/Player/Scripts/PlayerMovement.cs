using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private KeyCode toggleCursorLockKey = KeyCode.Escape;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float groundedGraceTime = 0.12f;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -18f;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1.1f;

    public GameObject mainCamera;

    private CharacterController controller;
    private Transform cameraTransform;
    private float verticalVelocity;
    private float originalHeight;
    private Vector3 originalCenter;
    private bool isCrouched;
    private float groundedTimer;
    private Transform visual;
    private Vector3 visualDefaultScale;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = mainCamera != null ? mainCamera.transform : null;

        originalHeight = controller.height;
        originalCenter = controller.center;

        visual = FindVisualTransform();
        if (visual != null)
        {
            visualDefaultScale = visual.localScale;
        }

        if (lockCursorOnStart)
        {
            SetCursorLocked(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleCursorLockKey))
        {
            SetCursorLocked(Cursor.lockState != CursorLockMode.Locked);
        }

        HandleCrouch();
        var move = GetMoveVector();
        ApplyGravityAndJump();
        controller.Move((move + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private Vector3 GetMoveVector()
    {
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var move = new Vector3(input.x, 0f, input.y);

        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        if (cameraTransform != null)
        {
            var forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();
            var right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();
            move = (forward * move.z + right * move.x);
        }

        var targetSpeed = walkSpeed;
        if (!isCrouched && Input.GetKey(sprintKey))
        {
            targetSpeed = sprintSpeed;
        }
        else if (isCrouched)
        {
            targetSpeed = crouchSpeed;
        }

        if (move.sqrMagnitude > 0.001f)
        {
            var targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        return move * targetSpeed;
    }

    private void ApplyGravityAndJump()
    {
        if (controller.isGrounded)
        {
            groundedTimer = groundedGraceTime;
        }
        else
        {
            groundedTimer -= Time.deltaTime;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (!isCrouched && groundedTimer > 0f && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            groundedTimer = 0f;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleCrouch()
    {
        var crouchInput = Input.GetKey(crouchKey);
        if (crouchInput == isCrouched)
        {
            return;
        }

        SetCrouch(crouchInput);
    }

    private static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void SetCrouch(bool crouch)
    {
        var bottom = transform.position.y + controller.center.y - (controller.height * 0.5f);

        isCrouched = crouch;
        controller.height = isCrouched ? crouchHeight : originalHeight;

        var center = controller.center;
        center.x = originalCenter.x;
        center.z = originalCenter.z;
        center.y = bottom - transform.position.y + (controller.height * 0.5f);
        controller.center = center;

        if (visual != null)
        {
            var targetScale = visualDefaultScale;
            targetScale.y = isCrouched ? visualDefaultScale.y * 0.6f : visualDefaultScale.y;
            visual.localScale = targetScale;
        }
    }

    private Transform FindVisualTransform()
    {
        if (TryGetComponent<MeshRenderer>(out var renderer) && TryGetComponent<MeshFilter>(out var filter))
        {
            var visualRoot = new GameObject("Visual");
            visualRoot.transform.SetParent(transform, false);

            var visualRenderer = visualRoot.AddComponent<MeshRenderer>();
            var visualFilter = visualRoot.AddComponent<MeshFilter>();
            visualFilter.sharedMesh = filter.sharedMesh;
            visualRenderer.sharedMaterials = renderer.sharedMaterials;

            renderer.enabled = false;
            filter.mesh = null;

            return visualRoot.transform;
        }

        if (transform.childCount > 0)
        {
            return transform.GetChild(0);
        }

        return null;
    }
}
