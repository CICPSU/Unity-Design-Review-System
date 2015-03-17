using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMapManager : MonoBehaviour {

	public Camera miniMapCam;
	public GameObject compassPlane;
	public GameObject avatar;
	public Text zoomLabel;
	public Text sizeLabel;
	public GameObject miniMapCanvas;

	public float mapProportionOfScreen = 0.2f;
	public float maxOrthographicSize = 5;
	public bool isMiniActive = false;


	private int zoomLevel = 10;
	private Vector2 relativeClickPostion = Vector2.zero;
	private Vector2 miniMapCenter = Vector2.zero;
	private Vector3 rayStartPostion = Vector2.zero;
	private int botMiniMapEdge;
	private int leftMiniMapEdge;
	private int mapDim;
	private int compassWidth;
	private int compassHeight;


	void Start()
	{
		SetMiniMapCam ();
		SetCompassTransform ();
		zoomLabel.text = "Zoom: " + zoomLevel;
		sizeLabel.text = "Size: " + mapProportionOfScreen;
	}

	void Update(){
		miniMapCenter = new Vector2 (Screen.width - mapDim * 0.5f, Screen.height - 0.5f * mapDim - 30);
		
		// teleport to location clicked on minimap
		if(Input.GetMouseButtonDown(0) && InsideMiniMap()){
			Debug.Log("minimap center: " + miniMapCenter);
			Debug.Log("Mouse Position: " + Input.mousePosition);
			relativeClickPostion.x = (Input.mousePosition.x - miniMapCenter.x) / mapDim;
			relativeClickPostion.y = (Input.mousePosition.y - miniMapCenter.y) / mapDim;
			Debug.Log("relative click: " + relativeClickPostion);
			rayStartPostion = miniMapCam.transform.position + (relativeClickPostion.x*transform.right + relativeClickPostion.y*transform.up) * miniMapCam.orthographicSize;
			RaycastHit hit = new RaycastHit();
			Physics.Raycast(rayStartPostion,Vector3.down,out hit, Mathf.Infinity);
			
			avatar.transform.position = hit.point;
			
		}


		if(Input.GetKeyDown("m"))
		{
			isMiniActive = !isMiniActive;
			miniMapCanvas.SetActive(isMiniActive);
		}


	}

	public bool InsideMiniMap()
	{
		return((Input.mousePosition.x > leftMiniMapEdge) && (Input.mousePosition.y > botMiniMapEdge) && (Input.mousePosition.y < Screen.height - 30));
	}

	public void ChangeZoom(bool isIncrease)
	{
		if (isIncrease) 
		{
			zoomLevel += 10;
		} 
		else 
		{
			zoomLevel -= 10;
		}

		if (zoomLevel > 100) 
		{
			zoomLevel =  100;
		}

		// This is where we will reset the zoom of the ortho camera that is used to capture the minimap.
		float newOrthoSize = maxOrthographicSize - maxOrthographicSize * zoomLevel / 100;
		if (newOrthoSize <= 0) {
			newOrthoSize = .01f;
		}
		miniMapCam.orthographicSize = newOrthoSize;
		SetCompassTransform ();

		// set zoom label
		zoomLabel.text = "Zoom: " + zoomLevel;
	}

	void SetCompassTransform()
	{
		compassPlane.transform.localScale = new Vector3 (miniMapCam.orthographicSize/20, 1, miniMapCam.orthographicSize/20);
		compassPlane.transform.localPosition = new Vector3 (.75f*miniMapCam.orthographicSize, 2.2f , .75f*miniMapCam.orthographicSize);
	}

	void SetMiniMapCam()
	{
		mapDim = (int)(Screen.height * mapProportionOfScreen);
		botMiniMapEdge = Screen.height - mapDim - 30;
		leftMiniMapEdge = Screen.width - mapDim;
		miniMapCam.GetComponent<Camera>().pixelRect = new Rect (leftMiniMapEdge, botMiniMapEdge, mapDim, mapDim);
	}

	public void ChangeSize(bool isIncrease)
	{
		if (isIncrease) 
		{
			mapProportionOfScreen += 0.05f;
		} 
		else 
		{
			mapProportionOfScreen -= 0.05f;
		}

		if (mapProportionOfScreen < .2f)
						mapProportionOfScreen = .2f;

		if (mapProportionOfScreen > .9f)
						mapProportionOfScreen = .9f;

		SetMiniMapCam ();

		sizeLabel.text = "Size: " + mapProportionOfScreen;
	}

}
