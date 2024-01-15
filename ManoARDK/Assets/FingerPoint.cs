using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Niantic.Lightship.AR.Meshing;
public class FingerPoint : MonoBehaviour
{
    public Rigidbody arObject;
    public GameObject objLayer;
    public GameObject FingerObj;
    private float skeletonConfidence = 0.0001f;
    private AROcclusionManager occlusionManager;
    private LightshipMeshingExtension meshManager;

    //public ARMeshManager meshManager;

    private void Start()
    {
        occlusionManager = FindObjectOfType<AROcclusionManager>();
        meshManager = FindObjectOfType<LightshipMeshingExtension>();
    }

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
            Vector3 palmPos = ManoUtils.Instance.CalculateNewPositionDepth(palmCenter, depthEstimation);
            objLayer.transform.position = palmPos;
            objLayer.SetActive(true);


            ManomotionManager.Instance.ShouldCalculateSkeleton3D(true);

            Vector3 jointPos = ManoUtils.Instance.CalculateNewPositionSkeletonJointDepth(new Vector3(trackingInfo.skeleton.joints[8].x, trackingInfo.skeleton.joints[8].y, trackingInfo.skeleton.joints[8].z), depthEstimation);
            FingerObj.transform.position = jointPos;
            FingerObj.SetActive(true);

            GestureInfo gestureInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
            ManoGestureTrigger currentGesture = gestureInfo.mano_gesture_trigger;

           
            if (currentGesture == ManoGestureTrigger.RELEASE_GESTURE || currentGesture == ManoGestureTrigger.GRAB_GESTURE)
            {
               SpawnObject();
            }

           // occlusionManager.enabled = false;
           // meshManager.MaximumIntegrationDistance = 0;

        }
        else
        {
            //cclusionManager.enabled = true;
           // meshManager.MaximumIntegrationDistance = 40;
            FingerObj.SetActive(false);
            objLayer.SetActive(false);
        }


    }

    public void SpawnObject()
    {
        

      // Instantiate(arObject, jointPos, transform.rotation);

       Rigidbody rb = Instantiate(arObject, objLayer.transform.position, objLayer.transform.localRotation).GetComponent<Rigidbody>();
       rb.AddForce(objLayer.transform.forward * 10f, ForceMode.Impulse);
        Handheld.Vibrate();

    }
}
