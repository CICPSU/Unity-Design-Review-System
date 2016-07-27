using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MarkerInfoCanvasSetup : MonoBehaviour, IAcceptRaycast{

    private GameObject markerInfoCanvasRef;
    
	void Start()
	{
		markerInfoCanvasRef = POI_ReferenceHub.Instance.markerInfoCanvas;
	}
    
    public void RaycastTrigger()
    {
        SetupCanvas();
    }

    public void SetupCanvas()
    {
        MarkerInfoCanvasRefs.activeMarker = gameObject.transform.parent.parent.gameObject;
        markerInfoCanvasRef.SetActive(true);
        markerInfoCanvasRef.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = UIUtilities.SetPopUpPanel(markerInfoCanvasRef.transform.GetChild(0).GetComponent<RectTransform>());
        markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs>().nameText.GetComponent<Text>().text = transform.parent.parent.GetComponent<POIInfo>().name;
        markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs>().positionText.GetComponent<Text>().text = transform.parent.parent.GetComponent<POIInfo>().position.ToString();

    }
}
