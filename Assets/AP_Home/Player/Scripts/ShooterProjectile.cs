using UnityEngine;
using System;

[DisallowMultipleComponent]
public sealed class ShooterProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 55f;
    [SerializeField] private float lifeTime = 2.8f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float radius = 0.06f;

    private float destroyAt;
    private Transform ownerRoot;

    public void Initialize(float projectileSpeed, float projectileLifetime, LayerMask mask, string targetTag, Transform owner)
    {
        speed = projectileSpeed;
        lifeTime = projectileLifetime;
        hitMask = mask;
        enemyTag = targetTag;
        ownerRoot = owner != null ? owner.root : null;
        destroyAt = Time.time + lifeTime;
    }

    private void Awake()
    {
        destroyAt = Time.time + lifeTime;
    }

    private void Update()
    {
        if (Time.time >= destroyAt)
        {
            Destroy(gameObject);
            return;
        }

        var step = speed * Time.deltaTime;
        var origin = transform.position;
        var direction = transform.forward;

        var hits = Physics.SphereCastAll(origin, radius, direction, step, hitMask, QueryTriggerInteraction.Collide);
        if (hits.Length > 0)
        {
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var hit in hits)
            {
                if (IsOwnerCollider(hit.collider))
                {
                    continue;
                }

                ProcessHit(hit.collider);
                Destroy(gameObject);
                return;
            }
        }

        transform.position += direction * step;
    }

    private bool IsOwnerCollider(Collider hitCollider)
    {
        return ownerRoot != null && hitCollider != null && hitCollider.transform.root == ownerRoot;
    }

    private void ProcessHit(Collider hitCollider)
    {
        if (hitCollider == null)
        {
            return;
        }

        if (hitCollider.CompareTag(enemyTag))
        {
            Destroy(hitCollider.gameObject);
            return;
        }

        var root = hitCollider.transform;
        while (root != null)
        {
            if (root.CompareTag(enemyTag))
            {
                Destroy(root.gameObject);
                return;
            }

            root = root.parent;
        }

        var enemyAgent = hitCollider.GetComponentInParent<EnemyAgent>();
        if (enemyAgent != null)
        {
            Destroy(enemyAgent.gameObject);
        }
    }
}
