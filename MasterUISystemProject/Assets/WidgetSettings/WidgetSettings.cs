using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;


/// <summary>
/// This class will end up having members for every kind of configurable field we would need.
/// Fields can be removed from the xml file for each widget if they aren't being used.
/// </summary>
public abstract class WidgetSettings {


	public string gameobjectName;
	/*
	public bool configRectTransform = false;
	public Vector2 rectPos = Vector2.zero;
	public Vector2 rectSize = Vector2.zero;

	public bool config3DTransform = false;
	public Vector3 transformPos = Vector3.zero;
	public Vector3 transformRot = Vector3.zero;
	public Vector3 transformScale = Vector3.one;
	*/
	public abstract void ApplySettings ();
	/*
	{

		GameObject gO = GameObject.Find (gameobjectName);

		if(configRectTransform)
		{
			RectTransform rectTrans = gO.GetComponent<RectTransform> ();
			rectTrans.anchoredPosition = rectPos;
			rectTrans.sizeDelta = rectSize;
		}

		if(config3DTransform)
		{
			Transform trans = gO.transform;
			trans.position = transformPos;
			trans.eulerAngles = transformRot;
			trans.localScale = transformScale;
		}

	}
*/

	public WidgetSettings()
	{
		gameobjectName = "name";
	}
}

public class MiniMapSettings : WidgetSettings {

	public Vector2 rectPos = Vector2.zero;

	public float mapPortionOfScreen;
	public float orthoCamRadiusFeet;
	
	public override void ApplySettings()
	{
		GameObject gO = GameObject.Find (gameobjectName);

		RectTransform rectTrans = GameObject.Find("MiniMapRoot").GetComponent<RectTransform> ();
		rectTrans.anchoredPosition = rectPos;

		gO.GetComponent<MiniMapManager> ().mapProportionOfScreen = mapPortionOfScreen;
		gO.GetComponent<MiniMapManager> ().orthoCamRadiusFeet = orthoCamRadiusFeet;
	}

	public MiniMapSettings()
	{
	}

}