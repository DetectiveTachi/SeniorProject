using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arObjectPrefab;
    [SerializeField] private TargetSpawner targetSpawner;
    private ARRaycastManager arRaycastManager;
    private bool objectSpawned = false;
    private List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        if (!objectSpawned && arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = s_Hits[0].pose;
            Instantiate(arObjectPrefab, hitPose.position, hitPose.rotation);
            objectSpawned = true;

            //targetSpawner.StartingPosition = hitPose.position;
            targetSpawner.gameObject.SetActive(true);
        }
    }
}