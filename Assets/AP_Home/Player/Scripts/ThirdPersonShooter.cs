using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerMovement))]
public sealed class ThirdPersonShooter : MonoBehaviour
{
    public static ThirdPersonShooter Instance;

    [SerializeField] private UnityEngine.Camera targetCamera;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private string muzzleChildName = "WeaponRect/WeaponMuzzle";
    [SerializeField] private GameObject projectilePrefab;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        Instance = this;

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

        if (Input.GetMouseButtonDown(0))
        {
            SpawnProjectile();
        }
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

    private void SpawnProjectile()
    {
        if (projectilePrefab == null || muzzleTransform == null || targetCamera == null)
        {
            return;
        }

        var cameraRotation = targetCamera.transform.rotation;
        Instantiate(projectilePrefab, muzzleTransform.position, cameraRotation);
    }
}
