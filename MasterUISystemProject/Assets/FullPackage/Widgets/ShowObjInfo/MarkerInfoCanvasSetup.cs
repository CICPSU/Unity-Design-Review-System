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
		if(gameObject.name == "Pin")//pin head clicked
        { 
		    markerInfoCanvasRef.SetActive (true);
            markerInfoCanvasRef.transform.GetChild(0).transform.position = Input.mousePosition; 
            /*
		    markerInfoCanvasRef.GetComponent <RectTransform>().position = gameObject.transform.position + new Vector3(.25f,1,0);
		    markerInfoCanvasRef.GetComponent <RectTransform>().LookAt (DIRE.Instance.Head.transform);
		    markerInfoCanvasRef.transform.RotateAround (markerInfoCanvasRef.transform.position, markerInfoCanvasRef.transform.up, 180);
		    markerInfoCanvasRef.GetComponent <RectTransform>().position -= markerInfoCanvasRef.GetComponent <RectTransform>().forward*0.5f;
            */
		    markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().nameText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().name;
		    markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().positionText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().position.ToString();
		}
        else // needle clicked
        { 
			markerInfoCanvasRef.SetActive (true);
            markerInfoCanvasRef.transform.GetChild(0).transform.position = Input.mousePosition;
            /*
			markerInfoCanvasRef.GetComponent <RectTransform>().position = gameObject.transform.parent.position + new Vector3(.25f,1,0);
			markerInfoCanvasRef.GetComponent <RectTransform>().LookAt (DIRE.Instance.Head.transform);
			markerInfoCanvasRef.transform.RotateAround (markerInfoCanvasRef.transform.position, markerInfoCanvasRef.transform.up, 180);
			markerInfoCanvasRef.GetComponent <RectTransform>().position -= markerInfoCanvasRef.GetComponent <RectTransform>().forward*0.5f;
			*/
            markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().nameText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().name;
			markerInfoCanvasRef.GetComponent<MarkerInfoCanvasRefs> ().positionText.GetComponent<Text>().text = transform.parent.GetComponent<POIInfo> ().position.ToString();
		}
	}


}
