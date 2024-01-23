using UnityEngine;

public class Target : MonoBehaviour
{
    private TargetSpawner targetSpawner;

    private void Start()
    {
        targetSpawner = FindObjectOfType<TargetSpawner>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            //targetSpawner.TargetHit(gameObject);
        }
    }
}