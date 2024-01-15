using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Niantic.Lightship.AR.Meshing;

public class SpawnObjectOnClick : MonoBehaviour
{

   public Rigidbody arObject;
   public GameObject objLayer;
   private float skeletonConfidence = 0.0001f;
   //public ARMeshManager meshManager;
    LightshipMeshingExtension m;
    
   

    // Update is called once per frame
    void Update()
    {
        ManomotionManager.Instance.ShouldCalculateGestures(true);
        bool hasConfidence = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.skeleton.confidence > skeletonConfidence;
        TrackingInfo trackingInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
        
        if (hasConfidence)
        {
            
            var palmCenter = trackingInfo.palm_center;
            var depthEstimation = trackingInfo.depth_estimation;
           // meshManager.enabled = !meshManager.enabled;
            Vector3 jointPos = ManoUtils.Instance.CalculateNewPositionDepth(palmCenter, depthEstimation);
            objLayer.transform.position = jointPos;
            objLayer.SetActive(true);

            GestureInfo gestureInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
            ManoGestureTrigger currentGesture = gestureInfo.mano_gesture_trigger;

            if (currentGesture == ManoGestureTrigger.RELEASE_GESTURE)// || currentGesture == ManoGestureTrigger.GRAB_GESTURE)
            {
                SpawnObject();
            }

        }
        else
        {
            objLayer.SetActive(false);
        }
     
       
    }

    public void SpawnObject()
    {
        ManomotionManager.Instance.ShouldCalculateSkeleton3D(true);
        
        //Instantiate(arObject, jointPos, transform.rotation);
      
        Rigidbody rb = Instantiate(arObject, objLayer.transform.position, objLayer.transform.localRotation).GetComponent<Rigidbody>();
        rb.AddForce(objLayer.transform.forward * 10f, ForceMode.Impulse);
       // Handheld.Vibrate();
        
    }
}
