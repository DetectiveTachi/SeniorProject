using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject placementObject;

    //private List<GameObject> placedObjects = new List<GameObject>();

    void Update()
    {
        if (Input.touchCount > 0 && Input.touchCount < 2 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = touch.position;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                Debug.Log("We hit a UI Element");
                return;
            }

            Debug.Log("Touch detected, fingerId: " + touch.fingerId);

            TouchToRay(touch.position);
        }
    }

    void TouchToRay(Vector3 touch)
    {
        Ray ray = mainCam.ScreenPointToRay(touch);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            placementObject.SetActive(true);
            placementObject.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z + 1);
            
            
        }
    }
}