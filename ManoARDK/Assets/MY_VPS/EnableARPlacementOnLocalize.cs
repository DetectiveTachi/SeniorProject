using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Niantic.Lightship.AR.LocationAR;
using Niantic.Lightship.AR.PersistentAnchors;

public class EnableARPlacementPnLocalize : MonoBehaviour
{

    private ARLocationManager _arLocationManager;

    private ARPlaneManager _arPlaneManager;

    private AR_Placement _arPlacement;

    // Start is called before the first frame update
    void Start()
    {
        _arLocationManager = FindObjectOfType<ARLocationManager>();
        _arPlaneManager = FindObjectOfType<ARPlaneManager>();
        _arPlacement = FindObjectOfType<AR_Placement>();

        _arPlaneManager.enabled = false;
        _arPlacement.enabled = false;

        _arLocationManager.locationTrackingStateChanged += OnLocalized;
    }

    void OnLocalized(ARLocationTrackedEventArgs eventArgs)
    {
        if (eventArgs.Tracking)
        {
            _arPlaneManager.enabled = true;
            _arPlacement.enabled = true;
        }
        else
        {
            _arPlaneManager.enabled = false;
            _arPlacement.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
