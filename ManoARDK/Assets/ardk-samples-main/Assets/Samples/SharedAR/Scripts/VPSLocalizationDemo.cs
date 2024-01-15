using System.Linq;
using Niantic.Lightship.AR.LocationAR;
using Niantic.Lightship.AR.Subsystems;
using Niantic.Lightship.SharedAR.Colocalization;

using Unity.Netcode;

using UnityEngine;

public class VPSLocalizationDemo : MonoBehaviour
{
    [SerializeField]
    private SharedSpaceManager _sharedSpaceManager;
    public SharedSpaceManager SharedSpaceManager;
    public ARLocationManager ARLocationManager;
    public GameObject ConnectionUI;

    private bool _started;

    // Start is called before the first frame update
    void Start()
    {
        SharedSpaceManager.sharedSpaceManagerStateChanged += OnSharedSpaceStateChanged;
        // This demo only targets a single ARLocation, so we can just use the first location.
        // For applications that choose from a list, use the specific ARLocation that you are localizing against
        //  as the room ID
        var vpsTrackingOptions = ISharedSpaceTrackingOptions.CreateVpsTrackingOptions(ARLocationManager.ARLocations.First());
        var roomOptions = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
        ARLocationManager.ARLocations.First().Payload.ToBase64(),
        10,
        "Room Description Here!"
    );
        _sharedSpaceManager.StartSharedSpace(vpsTrackingOptions, roomOptions);
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