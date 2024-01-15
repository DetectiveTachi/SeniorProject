using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int initialTargetCount = 1;
    [SerializeField] private float spawnOffset = 2f;
    [SerializeField] private float randomPositionRange = 2f;

    private List<GameObject> targets = new List<GameObject>();
    private int currentRound = 1;
    public GameObject targetsParent; // Parent object for the spawned targets
    private Vector3 initialParentPosition; // Initial position of the parent object

    private void Start()
    {
        initialParentPosition = targetsParent.transform.position; // Store the initial position of the parent
        SpawnTargets(initialTargetCount);
    }

    private void SpawnTargets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject target = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
            targetsParent.transform.position = initialParentPosition;
            target.transform.parent = targetsParent.transform; // Set the parent of the spawned target
            targets.Add(target);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-randomPositionRange, randomPositionRange) + initialParentPosition.x;
        float y = Random.Range(0, 1f) + initialParentPosition.y;
        float z =  spawnOffset + initialParentPosition.z;
        return new Vector3(x, y, z);
    }

    public void TargetHit(GameObject target)
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