using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class MarkerInfoCanvasRefs : MonoBehaviour {

	public GameObject nameText, positionText;
    public static GameObject activeMarker;

    public void CloseMarkerInfo()
    {
        activeMarker = null;
        gameObject.SetActive(false);

    }

    public void RemovePOI()
    {
        //remove the point from the original handler
        POIButtonManager.originalHandler.RemovePoint(activeMarker.GetComponent<POIInfo>().Point);

        //remove the button from the button manager
        POIButtonManager.instance.RemoveButton((from x in 
                                               POI_ReferenceHub.Instance.POIMenu.GetComponentInChildren<ScrollRect>().GetComponentsInChildren<Text>()
                                               where x.text == activeMarker.name
                                               select x).ToArray()[0].gameObject.transform.parent.gameObject);
        //remove the marker from the marker root
        Destroy(activeMarker);

        CloseMarkerInfo();

        // save
        POIButtonManager.instance.SaveButsToXML();

    }
}
