using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int initialTargetCount = 1;
    [SerializeField] private float spawnOffset = 2f;
    [SerializeField] private float randomPositionRange = 2f;

    private List<GameObject> targets = new List<GameObject>();
    private int currentRound = 1;
   

    private void Start()
    {
        
        SpawnTargets(initialTargetCount);
    }

    private void SpawnTargets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject target = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
            targets.Add(target);

            AdventureEnemy enemy = target.GetComponent<AdventureEnemy>();
            if (enemy != null)
            {
                enemy.OnEnemyDestroyed += DestroyTarget;
            }
        }

  
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * randomPositionRange;
        randomPosition += transform.position;
        randomPosition.y = transform.position.y; // Keep the same y position as the spawner
        randomPosition.y += spawnOffset; // Add the spawn offset

        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomPosition, out navMeshHit, randomPositionRange, NavMesh.AllAreas))
        {
            return navMeshHit.position;
        }

        return transform.position; // Return the spawner position if no valid position found
    }

    private void DestroyTarget(GameObject target)
    {
        targets.Remove(target);
        Destroy(target);

        if (targets.Count == 0)
        {
            currentRound++;
            int newTargetCount = initialTargetCount + currentRound;
            SpawnTargets(newTargetCount);
        }
    }
}