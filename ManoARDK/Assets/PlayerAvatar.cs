using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerAvatar : NetworkBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab; // Reference to the projectile prefab

    [SerializeField]
    private Transform shootTransform; // Reference to the shoot transform

    private void Update()
    {
        if (IsOwner)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                CmdShootProjectileServerRPC(shootTransform.position, shootTransform.rotation);
            }
        }
    }

    [ServerRpc]
    private void CmdShootProjectileServerRPC(Vector3 position, Quaternion rotation)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, position, rotation);
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        else
        {
            Debug.LogError("NetworkObject component not found on the projectile.");
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(projectile.transform.forward * 8f, ForceMode.Acceleration);
        }
        else
        {
            Debug.LogError("Rigidbody component not found on the projectile.");
        }

        Handheld.Vibrate();
    }
}