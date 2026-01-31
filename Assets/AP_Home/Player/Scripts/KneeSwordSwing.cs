using System.Collections;
using UnityEngine;

public sealed class KneeSwordSwing : MonoBehaviour
{
    [SerializeField] private string swordChildName = "Sword";
    [SerializeField] private float swingDuration = 0.25f;
    [SerializeField] private float swingCooldown = 0.15f;
    [SerializeField] private float shoulderSwapXMultiplier = -1f;
    [SerializeField] private float shoulderYOffset = 0.35f;
    [SerializeField] private float shoulderZOffset = 0.05f;
    [SerializeField] private float endYOffset = -0.15f;
    [SerializeField] private float endZOffset = 0.35f;

    private Transform sword;
    private Collider swordCollider;
    private PlayerAttackState attackState;
    private Quaternion kneeDefaultLocalRotation;
    private Vector3 kneeDefaultLocalPosition;
    private bool isSwinging;
    private float nextAllowedTime;

    private void Awake()
    {
        attackState = GetComponentInParent<PlayerAttackState>();
        sword = transform.Find(swordChildName);
        kneeDefaultLocalRotation = transform.localRotation;
        kneeDefaultLocalPosition = transform.localPosition;

        if (sword != null)
        {
            swordCollider = sword.GetComponent<Collider>();
            if (swordCollider != null)
            {
                swordCollider.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (isSwinging || Time.time < nextAllowedTime)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (sword == null || attackState == null)
        {
            return;
        }

        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        isSwinging = true;
        nextAllowedTime = Time.time + swingDuration + swingCooldown;

        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }

        attackState.SetAttacking(true);

        // Two slash variants:
        // - Left shoulder (upper) -> Right side (lower)
        // - Right shoulder (upper) -> Left side (lower)
        var leftToRight = Random.value > 0.5f;

        var rightHandPos = kneeDefaultLocalPosition;
        var leftHandPos = kneeDefaultLocalPosition;
        leftHandPos.x *= shoulderSwapXMultiplier;

        var leftShoulderPos = leftHandPos + new Vector3(0f, shoulderYOffset, shoulderZOffset);
        var rightShoulderPos = rightHandPos + new Vector3(0f, shoulderYOffset, shoulderZOffset);
        var leftEndPos = leftHandPos + new Vector3(0f, endYOffset, endZOffset);
        var rightEndPos = rightHandPos + new Vector3(0f, endYOffset, endZOffset);

        var startPos = leftToRight ? leftShoulderPos : rightShoulderPos;
        var endPos = leftToRight ? rightEndPos : leftEndPos;

        transform.localPosition = startPos;
        Physics.SyncTransforms();

        var startEuler = leftToRight ? new Vector3(-40f, 0f, -60f) : new Vector3(-40f, 0f, 60f);
        var endEuler = leftToRight ? new Vector3(40f, 0f, 60f) : new Vector3(40f, 0f, -60f);
        var startRot = Quaternion.Euler(startEuler);
        var endRot = Quaternion.Euler(endEuler);

        var elapsed = 0f;
        while (elapsed < swingDuration)
        {
            var t = Mathf.Clamp01(elapsed / swingDuration);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            elapsed += Time.fixedDeltaTime;
        }

        transform.localRotation = kneeDefaultLocalRotation;
        transform.localPosition = kneeDefaultLocalPosition;
        Physics.SyncTransforms();

        attackState.SetAttacking(false);

        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }

        isSwinging = false;
    }
}
