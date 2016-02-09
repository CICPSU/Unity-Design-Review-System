using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMapManager : MonoBehaviour {

	public GameObject avatar;
	public GameObject miniMapCam;
	public RectTransform compassImage;
	public Text zoomLabel;
	public GameObject miniMapPanel;
	public RectTransform miniMapRender;
	public RectTransform miniMapMask;
	public float mapProportionOfScreen = 0.2f;
	public bool isMiniActive = false;
	public float orthoCamRadiusFeet = 5;
	public int mapRadiusIncrementInFeet;

	private bool minimapRotate = false;
	private float feetToMetersFactor = 0.3048f;
	private Vector2 relativeClickPostion = Vector2.zero;
	private Vector2 miniMapCenter = Vector2.zero;
	private Vector3 rayStartPostion = Vector2.zero;
	private int mapDim;
	
	void Start()
	{
		avatar = GameObject.FindWithTag("Player");
		SetMiniMapCam ();
		zoomLabel.text = "Diameter: " + (2 * orthoCamRadiusFeet).ToString("F1")  + " ft";
	}

	void Update()
	{
		miniMapCam.transform.position = new Vector3 (avatar.transform.position.x, avatar.transform.position.y + 2.6f, avatar.transform.position.z);
		if (Input.GetKeyDown ("m")) 
		{
			isMiniActive = !isMiniActive;
			miniMapPanel.SetActive (isMiniActive);
		}
		if (Input.GetKeyDown ("r")) 
		{
			minimapRotate = !minimapRotate;
		}
	
		if (minimapRotate) 
		{
			miniMapCam.transform.eulerAngles = new Vector3 (90, avatar.transform.eulerAngles.y, 0);
			compassImage.transform.eulerAngles = new Vector3(0,0, avatar.transform.eulerAngles.y);
		} 
		else 
		{
			miniMapCam.transform.eulerAngles = new Vector3 (90, 0, 0);
			compassImage.transform.eulerAngles = Vector3.zero;
		}
	}

	public void SetRotate(bool rotateOn){
		minimapRotate = rotateOn;
	}

	public void Teleport()
	{
		relativeClickPostion.x = (Input.mousePosition.x - miniMapCenter.x) / (0.5f*mapDim);
		relativeClickPostion.y = (Input.mousePosition.y - miniMapCenter.y) / (0.5f*mapDim);
		rayStartPostion = miniMapCam.transform.position + (relativeClickPostion.x*miniMapCam.transform.right + relativeClickPostion.y*miniMapCam.transform.up) * miniMapCam.GetComponent<Camera>().orthographicSize;
		RaycastHit hit = new RaycastHit();
		if(	Physics.Raycast(rayStartPostion,Vector3.down,out hit, Mathf.Infinity))
			avatar.transform.position = hit.point;
	}
	
	public void SetMiniMapCam()
	{
		miniMapCenter = new Vector2 (miniMapMask.position.x - 0.5f*miniMapMask.sizeDelta.x, miniMapMask.position.y -0.5f*miniMapMask.sizeDelta.y);
		mapDim = (int)(Screen.height * mapProportionOfScreen);
		miniMapMask.sizeDelta = new Vector2 (mapDim, mapDim);
		miniMapRender.sizeDelta = new Vector2 (mapDim, mapDim);
		compassImage.sizeDelta = new Vector2 (mapDim + 10, mapDim + 10);
		compassImage.localPosition = new Vector3 (miniMapMask.localPosition.x - 0.5f*mapDim, miniMapMask.localPosition.y - 0.5f*mapDim, 0);
		miniMapCam.GetComponent<Camera>().orthographicSize = orthoCamRadiusFeet*feetToMetersFactor;
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
	}

	public void ChangeZoom(bool isIncrease)
	{
		if (isIncrease) 
			orthoCamRadiusFeet += mapRadiusIncrementInFeet*0.5f;
		else
			orthoCamRadiusFeet -= mapRadiusIncrementInFeet*0.5f;


		if (orthoCamRadiusFeet < 5)
			orthoCamRadiusFeet = 5;
		// This is where we will reset the zoom of the ortho camera that is used to capture the minimap.
		SetMiniMapCam ();
		
		// set zoom label
		zoomLabel.text = "Diameter: " + (2 * orthoCamRadiusFeet).ToString("F1")  + " ft";

	}

}
