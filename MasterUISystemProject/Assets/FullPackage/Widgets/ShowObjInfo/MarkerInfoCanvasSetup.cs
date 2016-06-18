using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MarkerInfoCanvasSetup : MonoBehaviour {
	private GameObject markerInfoCanvasRef;

    //public bool hasRaycastLock = false;

	void Start()
	{
		markerInfoCanvasRef = POI_ReferenceHub.Instance.markerInfoCanvas;
	}
    /*
	public void OnMouseUp()
    {
        if (!hasRaycastLock && RaycastLock.GetLock())
        {
            hasRaycastLock = true;
            MarkerInfoCanvasRefs.activeMarker = gameObject.transform.parent.gameObject;
            markerInfoCanvasRef.SetActive(true);
            markerInfoCanvasRef.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
            markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs>().nameText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo>().name;
            markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs>().positionText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo>().position.ToString();
        }
	}
    */

    public void SetupCanvas()
    {
        MarkerInfoCanvasRefs.activeMarker = gameObject.transform.parent.gameObject;
        markerInfoCanvasRef.SetActive(true);
        markerInfoCanvasRef.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = IConUtilities.SetPopUpPanel(markerInfoCanvasRef.transform.GetChild(0).GetComponent<RectTransform>());
        markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs>().nameText.GetComponent<Text>().text = transform.parent.parent.GetComponent<POIInfo>().name;
        markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs>().positionText.GetComponent<Text>().text = transform.parent.parent.GetComponent<POIInfo>().position.ToString();

    }
}
