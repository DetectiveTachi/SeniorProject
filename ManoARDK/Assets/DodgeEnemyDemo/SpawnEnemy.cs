using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.ARDK.AR;
using Niantic.Lightship.AR.Meshing;
using UnityEngine.XR.ARFoundation;

public class SpawnEnemy : MonoBehaviour
{
    SpawnObjectOnClick ballShoot;
    LightshipMeshingExtension meshExt;
    ARMeshManager aRMeshManager;
    public GameObject enemy;
    public GameObject StartButton;
    public Camera ARcam;
    ARSession _ARsession;
    // Start is called before the first frame update

    IEnumerator ShowButton()
    {
        yield return new WaitForSeconds(0.5f);
        StartButton.SetActive(true);
       
        
    }
    public void InstantiateOnStart()
    {
        var worldRay = ARcam.ScreenPointToRay(StartButton.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(worldRay, out hit, 1000f))
        {
            if (hit.transform.gameObject.name.Contains("MeshCollider") ||
                hit.transform.gameObject.name.Contains("Interior_"))
            {
                GameObject obj = Instantiate(enemy);
                obj.SetActive(true);
                obj.transform.position = hit.point;
                
                Vector3 plane = Vector3.ProjectOnPlane(Vector3.forward + Vector3.right, hit.normal);
                //Quaternion rotation = Quaternion.LookRotation(plane, hit.normal);
              //  obj.transform.rotation = rotation;
               // obj.transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f, Space.Self);
            }
        }
        Debug.Log("Pressed");
        ballShoot.enabled = true;
       
       // aRMeshManager.meshPrefab.mesh.
        meshExt.enabled = false;
        


        StartButton.SetActive(false);
    }
    void Start()
    {
       
        
        aRMeshManager = FindObjectOfType<ARMeshManager>();

        meshExt = FindObjectOfType<LightshipMeshingExtension>();

        ballShoot = FindObjectOfType<SpawnObjectOnClick>();

        StartCoroutine(ShowButton());
    }

   /* private void OnSessionInitialized(AnyARSessionInitializedArgs args)
    {
        //Now that we've initiated our session, we don't need to do this again so we can remove the callback
        ARSessionFactory.SessionInitialized -= OnSessionInitialized;

        //Here we're saving our AR Session to our '_ARsession' variable, along with any arguments our session contains
        _ARsession = args.Session;
    }


   */ // Update is called once per frame

}
