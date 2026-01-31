using UnityEngine;

public sealed class SwordHitbox : MonoBehaviour
{
    [SerializeField] private string enemyTag = "Enemy";

    private PlayerAttackState attackState;

    private void Awake()
    {
        attackState = GetComponentInParent<PlayerAttackState>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryKillEnemy(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryKillEnemy(other);
    }

    private void TryKillEnemy(Collider other)
    {
        if (attackState == null || !attackState.IsAttacking || other == null)
        {
            return;
        }

        if (!other.CompareTag(enemyTag))
        {
            return;
        }

        Destroy(other.gameObject);
    }
}
