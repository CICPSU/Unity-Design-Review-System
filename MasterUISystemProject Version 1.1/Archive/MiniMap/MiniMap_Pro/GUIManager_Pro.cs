using UnityEngine;
using System.Collections;

public class GUIManager_Pro : MonoBehaviour {

	public RenderTexture MiniMapTexture;
	public Material MiniMapMaterial;

	public Texture compass;


	public float leftOffset = 410;
	public float topOffset = 10;
	public float mapWidth = 400;
	public float mapHeight = 400;

	public float leftOffsetCompass = 110;
	public float topOffsetCompass = 10;
	public float compassWidth = 100;
	public float compassHeight = 100;
	public float mapProportionOfScreen = 0.2f;
	public float compassProportionOfScreen = 0.06f;

	public float maxOrthographicSize = 5;

	public Camera miniMapCam;

	public GameObject avatar;

	private int zoomLevel = 10;
	private Vector2 relativeClickPostion = Vector2.zero;
	private Vector2 miniMapCenter = Vector2.zero;
	private Vector3 rayStartPostion = Vector2.zero;

	
	void OnGUI(){
		miniMapCenter = new Vector2 (Screen.width - Screen.width * 0.5f * mapProportionOfScreen, Screen.height - topOffset - 0.5f * mapHeight);

		if (Event.current.type == EventType.Repaint) {
			mapWidth = Screen.width * mapProportionOfScreen - 10;
			mapHeight = Screen.width * mapProportionOfScreen - 10;
			compassWidth = Screen.width * compassProportionOfScreen - 10;
			compassHeight = Screen.width * compassProportionOfScreen - 10;
			Graphics.DrawTexture (new Rect (Screen.width - Screen.width * mapProportionOfScreen, topOffset, mapWidth, mapHeight), MiniMapTexture, MiniMapMaterial);
			Graphics.DrawTexture (new Rect (Screen.width - Screen.width * compassProportionOfScreen, topOffsetCompass, compassWidth, compassHeight), compass);
		}// if

		if (GUI.Button (new Rect (Screen.width - Screen.width * mapProportionOfScreen, topOffset + mapHeight, mapWidth * 0.5f, 30), "Decrease Zoom")) {
			zoomLevel -= 10;		
		}
		if (GUI.Button (new Rect (Screen.width - Screen.width * 0.5f * mapProportionOfScreen, topOffset + mapHeight, mapWidth * 0.5f, 30), "Increase Zoom")) {
			zoomLevel += 10;		
		}
		if (zoomLevel > 100) {
			zoomLevel =  100;
		}
		GUI.Label (new Rect (Screen.width - Screen.width * mapProportionOfScreen, topOffset + mapHeight + 30, mapWidth, 30), "Zoom Level: " + zoomLevel);

		// This is where we will reset the zoom of the ortho camera that is used to capture the minimap.
		float newOrthoSize = maxOrthographicSize - maxOrthographicSize * zoomLevel / 100;
		if (newOrthoSize <= 0) {
			newOrthoSize = .01f;
		}
		miniMapCam.orthographicSize = newOrthoSize;

		if(Input.GetMouseButtonDown(0) && (Input.mousePosition.x > Screen.width - Screen.width * mapProportionOfScreen) && (Input.mousePosition.y > Screen.height - topOffset - mapHeight)){
			Debug.Log("minimap center: " + miniMapCenter);
			Debug.Log("Mouse Position: " + Input.mousePosition);
			relativeClickPostion.x = (Input.mousePosition.x - miniMapCenter.x) / mapWidth;
			relativeClickPostion.y = (Input.mousePosition.y - miniMapCenter.y) / mapHeight;
			Debug.Log("relative click: " + relativeClickPostion);
			rayStartPostion = miniMapCam.transform.position + (relativeClickPostion.x*transform.right + relativeClickPostion.y*transform.up) * miniMapCam.orthographicSize*2;
			RaycastHit hit = new RaycastHit();
			Physics.Raycast(rayStartPostion,Vector3.down,out hit, Mathf.Infinity);

			avatar.transform.position = hit.point;

		}
	}
}
