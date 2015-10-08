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

	public abstract void ApplySettings ();

	public abstract void SetValues (object[] values);

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

	public override void SetValues(object[] values)
	{
		rectPos = (Vector2)values[0];
		mapPortionOfScreen = (float)values[1];
		orthoCamRadiusFeet = (float)values[2];
		gameobjectName = (string)values[3];
	}

	public MiniMapSettings()
	{
	}

}