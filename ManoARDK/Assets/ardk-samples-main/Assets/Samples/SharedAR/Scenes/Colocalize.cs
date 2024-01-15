using Niantic.Lightship.SharedAR.Colocalization;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Niantic.Lightship.AR.Subsystems;

public class Colocalize : MonoBehaviour
{
    public SharedSpaceManager _sharedSpaceManager;

    public GameObject ConnectionUI;

    [SerializeField]
    private Texture2D _targetImage;

    [SerializeField]
    private float _targetImageSize;


    private bool _started;

    public void StartSharedSpace(string roomName)
    {
        var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(
                    _targetImage, _targetImageSize);
        var roomOptions = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
            roomName,
            10,
            "image tracking colocalization demo"
        );

        _sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomOptions);

        // start as host
      //  NetworkManager.Singleton.StartHost();
        // Or start as client
        // NetworkManager.Singleton.StartClient();
    }

    private void OnSharedSpaceStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs args)
    {
        if (args.Tracking && !_started)
        {
            ConnectionUI.SetActive(true);
            _started = true;
        }
    }

    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
        ConnectionUI.SetActive(false);
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
        ConnectionUI.SetActive(false);
    }
}
