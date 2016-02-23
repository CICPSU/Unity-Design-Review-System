using UnityEngine;
using System.Collections;

public class MarkerInfoCanvasRefs : MonoBehaviour {

	public GameObject nameText, positionText;
    public static GameObject activeMarker;

    public void CloseMarkerInfo()
    {
        activeMarker.GetComponent<MarkerInfoCanvasSetup>().hasRaycastLock = false;
        RaycastLock.GiveLock();
        gameObject.SetActive(false);

    }
}
