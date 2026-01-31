using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private float distance = 6f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private float collisionBuffer = 0.1f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (target == null)
        {
            var player = GameObject.Find("Player");
            target = player != null ? player.transform : null;
        }

        var angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        var rotation = Quaternion.Euler(pitch, yaw, 0f);
        var pivot = target.position + pivotOffset;
        var desiredPosition = pivot + rotation * new Vector3(0f, 0f, -distance);

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
    }
}
