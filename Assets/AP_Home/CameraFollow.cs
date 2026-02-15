using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private Vector3 aimPivotOffset = new Vector3(0.65f, 1.58f, 0f);
    [SerializeField] private float distance = 6f;
    [SerializeField] private float aimDistance = 3.4f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float aimSensitivityMultiplier = 0.75f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private float normalFov = 60f;
    [SerializeField] private float aimFov = 48f;
    [SerializeField] private float aimBlendSpeed = 12f;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private float collisionBuffer = 0.1f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private UnityEngine.Camera cameraComponent;
    private PlayerMovement playerMovement;
    private float yaw;
    private float pitch;
    private float aimBlend;

    private void Awake()
    {
        if (target == null)
        {
            var player = GameObject.Find("Player");
            target = player != null ? player.transform : null;
        }

        cameraComponent = GetComponent<UnityEngine.Camera>();
        playerMovement = target != null ? target.GetComponent<PlayerMovement>() : null;

        var angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        if (cameraComponent != null)
        {
            cameraComponent.fieldOfView = normalFov;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (playerMovement == null)
        {
            playerMovement = target.GetComponent<PlayerMovement>();
        }

        var isAiming = playerMovement != null && playerMovement.IsAiming;
        aimBlend = Mathf.MoveTowards(aimBlend, isAiming ? 1f : 0f, aimBlendSpeed * Time.deltaTime);

        var sensitivity = mouseSensitivity * Mathf.Lerp(1f, aimSensitivityMultiplier, aimBlend);
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        var rotation = Quaternion.Euler(pitch, yaw, 0f);
        var currentPivotOffset = Vector3.Lerp(pivotOffset, aimPivotOffset, aimBlend);
        var currentDistance = Mathf.Lerp(distance, aimDistance, aimBlend);

        var pivot = target.position
            + target.right * currentPivotOffset.x
            + Vector3.up * currentPivotOffset.y
            + target.forward * currentPivotOffset.z;
        var desiredPosition = pivot + rotation * new Vector3(0f, 0f, -currentDistance);

        var direction = desiredPosition - pivot;
        var distanceToTarget = direction.magnitude;
        if (distanceToTarget > 0.01f)
        {
            if (Physics.SphereCast(pivot, collisionRadius,
                direction.normalized, out var hit, distanceToTarget, collisionMask))
            {
                var safeDistance = Mathf.Max(hit.distance - collisionBuffer, 0.2f);
                desiredPosition = pivot + direction.normalized * safeDistance;
            }
        }

        transform.position = desiredPosition;
        transform.rotation = rotation;

        if (cameraComponent != null)
        {
            cameraComponent.fieldOfView = Mathf.Lerp(normalFov, aimFov, aimBlend);
        }
    }
}
