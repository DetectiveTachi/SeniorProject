using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float maxDistance = 10f;
    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        Vector3 randomPosition = GetRandomNavMeshPosition();
        Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        int randomIndex = Random.Range(0, navMeshData.indices.Length - 3);
        Vector3 randomPoint = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[randomIndex]], navMeshData.vertices[navMeshData.indices[randomIndex + 1]], Random.value);
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, maxDistance, NavMesh.AllAreas);
        return hit.position;
    }
}
