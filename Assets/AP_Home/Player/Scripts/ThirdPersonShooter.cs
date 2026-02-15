using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement))]
public sealed class ThirdPersonShooter : MonoBehaviour
{
    [SerializeField] private UnityEngine.Camera targetCamera;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private string muzzleChildName = "WeaponRect/WeaponMuzzle";
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 55f;
    [SerializeField] private float projectileLifetime = 2.8f;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private bool requireAimForShooting = true;
    [SerializeField] private float fireRate = 7f;
    [SerializeField] private float maxDistance = 120f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool drawDebugShot;

    private PlayerMovement playerMovement;
    private float nextShotTime;
    private static Material muzzleFallbackMaterial;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        targetCamera = ResolveCamera();
        muzzleTransform = ResolveMuzzleTransform();
    }

    private void Update()
    {
        if (targetCamera == null)
        {
            targetCamera = ResolveCamera();
            if (targetCamera == null)
            {
                return;
            }
        }

        if (muzzleTransform == null)
        {
            muzzleTransform = ResolveMuzzleTransform();
        }

        if (requireAimForShooting && !Input.GetMouseButton(1))
        {
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            return;
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        nextShotTime = Time.time + 1f / Mathf.Max(0.01f, fireRate);
        Fire();
    }

    private UnityEngine.Camera ResolveCamera()
    {
        if (targetCamera != null)
        {
            return targetCamera;
        }

        if (playerMovement != null && playerMovement.mainCamera != null)
        {
            var linkedCamera = playerMovement.mainCamera.GetComponent<UnityEngine.Camera>();
            if (linkedCamera != null)
            {
                return linkedCamera;
            }
        }

        return UnityEngine.Camera.main;
    }

    private Transform ResolveMuzzleTransform()
    {
        if (muzzleTransform != null)
        {
            return muzzleTransform;
        }

        if (!string.IsNullOrWhiteSpace(muzzleChildName))
        {
            var child = transform.Find(muzzleChildName);
            if (child != null)
            {
                return child;
            }
        }

        // Fallback for nested hierarchies when only leaf name is configured.
        var lookupName = muzzleChildName;
        var slashIndex = lookupName.LastIndexOf('/');
        if (slashIndex >= 0 && slashIndex < lookupName.Length - 1)
        {
            lookupName = lookupName[(slashIndex + 1)..];
        }

        var allChildren = GetComponentsInChildren<Transform>(true);
        foreach (var child in allChildren)
        {
            if (child != null && child != transform && child.name == lookupName)
            {
                return child;
            }
        }

        return null;
    }

    private void Fire()
    {
        var ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (drawDebugShot)
        {
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 0.1f);
        }

        var aimPoint = ray.origin + ray.direction * maxDistance;
        if (TryGetAimHit(ray, out var hit))
        {
            aimPoint = hit.point;
        }

        var muzzlePosition = transform.position + Vector3.up * 1.1f + transform.forward * 0.55f;
        if (muzzleTransform != null)
        {
            muzzlePosition = muzzleTransform.position;
        }

        var shotDirection = (aimPoint - muzzlePosition).normalized;
        if (shotDirection.sqrMagnitude < 0.0001f)
        {
            shotDirection = transform.forward;
        }

        var shotRotation = Quaternion.LookRotation(shotDirection, Vector3.up);
        SpawnProjectile(muzzlePosition, shotRotation);
        SpawnMuzzleFlash(muzzlePosition, shotRotation);
    }

    private bool TryGetAimHit(Ray ray, out RaycastHit validHit)
    {
        var hits = Physics.RaycastAll(ray, maxDistance, hitMask, QueryTriggerInteraction.Collide);
        if (hits.Length == 0)
        {
            validHit = default;
            return false;
        }

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        var ownerRoot = transform.root;
        foreach (var hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.transform.root == ownerRoot)
            {
                continue;
            }

            validHit = hit;
            return true;
        }

        validHit = default;
        return false;
    }

    private void SpawnProjectile(Vector3 muzzlePosition, Quaternion shotRotation)
    {
        if (projectilePrefab == null)
        {
            return;
        }

        var projectile = Instantiate(projectilePrefab, muzzlePosition, shotRotation);
        var projectileComponent = projectile.GetComponent<ShooterProjectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(projectileSpeed, projectileLifetime, hitMask, enemyTag, transform);
            return;
        }

        var rigidbody = projectile.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.velocity = projectile.transform.forward * projectileSpeed;
        }

        Destroy(projectile, projectileLifetime);
    }

    private void SpawnMuzzleFlash(Vector3 muzzlePosition, Quaternion shotRotation)
    {
        if (muzzleFlashPrefab == null)
        {
            return;
        }

        var flashObject = Instantiate(muzzleFlashPrefab, muzzlePosition, shotRotation);
        flashObject.transform.localScale = Vector3.one * 0.18f;

        var particleSystem = flashObject.GetComponent<ParticleSystem>();
        var particleRenderer = flashObject.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null)
        {
            var sharedMaterial = particleRenderer.sharedMaterial;
            var missingShader = sharedMaterial == null || sharedMaterial.shader == null || !sharedMaterial.shader.isSupported;
            if (missingShader)
            {
                particleRenderer.sharedMaterial = GetMuzzleFallbackMaterial();
            }
        }

        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = particleSystem.main;
            main.startSizeMultiplier = 0.09f;
            main.startSpeedMultiplier = 0.08f;
            main.startLifetimeMultiplier = 0.06f;
            main.loop = false;

            var emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = 0f;

            particleSystem.Play();
            var totalLifetime = 0.12f;
            Destroy(flashObject, totalLifetime);
            return;
        }

        Destroy(flashObject, 0.5f);
    }

    private static Material GetMuzzleFallbackMaterial()
    {
        if (muzzleFallbackMaterial != null)
        {
            return muzzleFallbackMaterial;
        }

        var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Particles/Unlit");
        }

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            return null;
        }

        muzzleFallbackMaterial = new Material(shader);
        muzzleFallbackMaterial.color = new Color(1f, 0.86f, 0.42f, 1f);
        return muzzleFallbackMaterial;
    }

}
