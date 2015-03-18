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
	public RectTransform miniMapRender;
	public RectTransform miniMapMask;

	public float mapProportionOfScreen = 0.2f;
	public float maxOrthographicSize = 5;
	public bool isMiniActive = false;


	private int zoomLevel = 10;
	private Vector2 relativeClickPostion = Vector2.zero;
	private Vector2 miniMapCenter = Vector2.zero;
	private Vector3 rayStartPostion = Vector2.zero;
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
		miniMapCenter = new Vector2 (miniMapMask.position.x - 0.5f*miniMapMask.sizeDelta.x, miniMapMask.position.y -0.5f*miniMapMask.sizeDelta.y);

		if(Input.GetKeyDown("m"))
		{
			isMiniActive = !isMiniActive;
			miniMapCanvas.SetActive(isMiniActive);
		}


	}

	public void Teleport()
	{
		//Debug.Log("minimap center: " + miniMapCenter);
		//Debug.Log("Mouse Position: " + Input.mousePosition);
		relativeClickPostion.x = (Input.mousePosition.x - miniMapCenter.x) / (0.5f*mapDim);
		relativeClickPostion.y = (Input.mousePosition.y - miniMapCenter.y) / (0.5f*mapDim);
		//Debug.Log("relative click: " + relativeClickPostion);
		rayStartPostion = miniMapCam.transform.position + (relativeClickPostion.x*miniMapCam.transform.right + relativeClickPostion.y*miniMapCam.transform.up) * miniMapCam.orthographicSize;
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(rayStartPostion,Vector3.down,out hit, Mathf.Infinity);
		
		avatar.transform.position = hit.point;
	}

	void SetCompassTransform()
	{
		compassPlane.transform.localScale = new Vector3 (miniMapCam.orthographicSize/20, 1, miniMapCam.orthographicSize/20);
		compassPlane.transform.localPosition = new Vector3 (.75f*miniMapCam.orthographicSize, .75f*miniMapCam.orthographicSize, 0.4f);
	}

	void SetMiniMapCam()
	{
		mapDim = (int)(Screen.height * mapProportionOfScreen);
		miniMapMask.sizeDelta = new Vector2 (mapDim, mapDim);
		miniMapRender.sizeDelta = new Vector2 (mapDim, mapDim);
		miniMapCam.orthographicSize = maxOrthographicSize - maxOrthographicSize * zoomLevel / 100;
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

}
