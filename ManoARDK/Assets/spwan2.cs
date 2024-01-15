using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class spwan2 : NetworkBehaviour
{
    [SerializeField] private GameObject RAobject;
    [SerializeField] private GameObject objLayer;

    private float skeletonConfidence = 0.0001f;
    // Update is called once per frame
    void Update()
    {
        
        HandTrackServer();
    }

    
    //[ServerRpc]
    private void HandTrackServer()
    {
        ManomotionManager.Instance.ShouldCalculateGestures(true);
        
        
        GestureInfo gestureinfo = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
        ManoGestureTrigger currentg = gestureinfo.mano_gesture_trigger;
        if (currentg == ManoGestureTrigger.RELEASE_GESTURE || currentg == ManoGestureTrigger.GRAB_GESTURE)
        {
            SpawningServerRPC();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawningServerRPC(ServerRpcParams serverRpcParams = default)
    {
        ManomotionManager.Instance.ShouldCalculateSkeleton3D(true);
        
        
        //TrackingInfo tracking = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
        
        var clientId = serverRpcParams.Receive.SenderClientId;
        bool hasConfidence = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.skeleton.confidence > skeletonConfidence;
        TrackingInfo trackingInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;


        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            objLayer.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);

        }
        else
        {
            objLayer.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }

        NetworkObject objLayerNet = Instantiate(objLayer).GetComponent<NetworkObject>();
        objLayerNet.Spawn();

        if (hasConfidence)
        {

            
            var palmCenter = trackingInfo.palm_center;
            var depthEstimation = trackingInfo.depth_estimation;
            // meshManager.enabled = !meshManager.enabled;
            Vector3 jointPos = ManoUtils.Instance.CalculateNewPositionDepth(palmCenter, depthEstimation);
            objLayerNet.transform.position = jointPos;
            
        }
        else
        {
            objLayerNet.transform.position = new Vector3(0, 0, -3);
        }

        if (NetworkManager.ConnectedClients.ContainsKey(clientId)) {
            var client = NetworkManager.ConnectedClients[clientId];
            //Vector3 newp = client.PlayerObject.transform.position tranform (jointpositioning)
            SpawnObject(client.PlayerObject.transform); 
           
        }
        else
        {
            SpawnObject(objLayerNet.transform);
           // SpawnObject(objLayer.transform);
        }
        // Calculate the offset from the camera position
        

        // Check if the current instance is the host or the client
        //if (IsServer)
        //{
            // Spawn the object on the host
            
        //}
        //else if (IsClient)
        //{
        //    // Send a request to the server to spawn the object on the client
        //    SpawnObjectClientRPC(spawnPosition);
        //}
    }
  

    private void SpawnObject(Transform spawnPosition)
    {
        GameObject spawnedObject = Instantiate(RAobject, spawnPosition.transform.position, spawnPosition.transform.localRotation);
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();

        // Rest of the code...
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        else
        {
            Debug.LogError("NetworkObject component not found on the spawned object.");
        }

        Rigidbody shoot = spawnedObject.GetComponent<Rigidbody>();
        if (shoot != null)
        {
            // Use the spawned object's forward direction
            shoot.AddForce(spawnedObject.transform.forward * 8f, ForceMode.Acceleration);
        }
        else
        {
            Debug.LogError("Rigidbody component not found on the spawned object.");
        }

        Handheld.Vibrate();
    }

}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class spwan2 : NetworkBehaviour
{
    [SerializeField] private GameObject RAobject;

    // Update is called once per frame
    void Update()
    {

        HandTrackServer();
    }


    //[ServerRpc]
    private void HandTrackServer()
    {
        ManomotionManager.Instance.ShouldCalculateGestures(true);
        GestureInfo gestureinfo = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
        ManoGestureTrigger currentg = gestureinfo.mano_gesture_trigger;

        if (currentg == ManoGestureTrigger.RELEASE_GESTURE)
        {
            SpawningServerRPC();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawningServerRPC(ServerRpcParams serverRpcParams = default)
    {
        ManomotionManager.Instance.ShouldCalculateSkeleton3D(true);
        TrackingInfo tracking = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
        float depthcalc = tracking.depth_estimation;
        Vector3 jointpositioning = ManoUtils.Instance.CalculateNewPositionSkeletonJointDepth(new Vector3(tracking.skeleton.joints[8].x,
        tracking.skeleton.joints[8].y, tracking.skeleton.joints[8].z), depthcalc);
        var clientId = serverRpcParams.Receive.SenderClientId;
        Vector3 spawnPosition = jointpositioning;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            var client = NetworkManager.ConnectedClients[clientId];
            //Vector3 newp = client.PlayerObject.transform.position tranform (jointpositioning)
            SpawnObject(client.PlayerObject.transform.position);
        }
        else
        {
            SpawnObject(spawnPosition);
        }
        // Calculate the offset from the camera position


        // Check if the current instance is the host or the client
        //if (IsServer)
        //{
        // Spawn the object on the host

        //}
        //else if (IsClient)
        //{
        //    // Send a request to the server to spawn the object on the client
        //    SpawnObjectClientRPC(spawnPosition);
        //}
    }
    [ClientRpc]
    private void SpawnObjectClientRPC(Vector3 spawnPosition)
    {
        // Spawn the object on the client
        SpawnObject(spawnPosition);
    }

    private void SpawnObject(Vector3 spawnPosition)
    {
        GameObject spawnedObject = Instantiate(RAobject, spawnPosition, Quaternion.identity);
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();

        // Rest of the code...
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        else
        {
            Debug.LogError("NetworkObject component not found on the spawned object.");
        }

        Rigidbody shoot = spawnedObject.GetComponent<Rigidbody>();
        if (shoot != null)
        {
            // Use the spawned object's forward direction
            shoot.AddForce(spawnedObject.transform.forward * 8f, ForceMode.Acceleration);
        }
        else
        {
            Debug.LogError("Rigidbody component not found on the spawned object.");
        }

        Handheld.Vibrate();
    }

}
*/