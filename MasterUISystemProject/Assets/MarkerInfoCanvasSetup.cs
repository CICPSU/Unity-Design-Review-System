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
		markerInfoCanvasRef.GetComponent <RectTransform>().position = gameObject.transform.position + new Vector3(0.25f,1,0);
		markerInfoCanvasRef.GetComponent <RectTransform>().LookAt (POI_ReferenceHub.Instance.Avatar.transform.position);
		markerInfoCanvasRef.GetComponent <RectTransform>().eulerAngles -= new Vector3 (0, 180, 0);
		markerInfoCanvasRef.GetComponent <RectTransform>().position -= markerInfoCanvasRef.GetComponent <RectTransform>().forward*0.35f;

		markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().nameText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().name;
		markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().positionText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().position.ToString();



	}
}
