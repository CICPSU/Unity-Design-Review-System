using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MarkerInfoCanvasSetup : MonoBehaviour {
	private GameObject markerInfoCanvasRef;

	void Start()
	{
		markerInfoCanvasRef = POI_ReferenceHub.Instance.markerInfoCanvas;
	}

	public void OnMouseUp()
    { 
		markerInfoCanvasRef.SetActive (true);
        markerInfoCanvasRef.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
        markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().nameText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().name;
		markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().positionText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().position.ToString();
		
	}
}
