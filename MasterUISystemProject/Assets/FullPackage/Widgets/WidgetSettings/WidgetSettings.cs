using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;


/// <summary>

/// </summary>
public abstract class WidgetSettings {

	//All implementation of the ApplySettings() should:
	//reference the widget through FindObjectsWithType([NAME OF WIDGET MANAGER])
	public bool enabled = true;
	public abstract void ApplySettings ();

	public abstract void SetValues (object[] values);

	public abstract object[] GetValues ();

	public WidgetSettings ()
	{
	}
}

public class MiniMapSettings : WidgetSettings {

	public Vector2 rectPos = Vector2.zero;

	public float mapPortionOfScreen;
	public float orthoCamRadiusFeet;
	
	public override void ApplySettings()
	{
		if (GameObject.FindObjectOfType<MiniMapManager> () == null)
			return;
		GameObject gO = GameObject.FindObjectOfType<MiniMapManager>().gameObject;

		RectTransform rectTrans = gO.GetComponentInChildren<RectTransform> ();
		rectTrans.anchoredPosition = rectPos;

		gO.GetComponent<MiniMapManager> ().mapProportionOfScreen = mapPortionOfScreen;
		gO.GetComponent<MiniMapManager> ().orthoCamRadiusFeet = orthoCamRadiusFeet;
		gO.GetComponent<MiniMapManager> ().SetMiniMapCam ();
		gO.SetActive (enabled);
	}

	public override void SetValues(object[] values)
	{
		rectPos = (Vector2)values[0];
		mapPortionOfScreen = (float)values[1];
		orthoCamRadiusFeet = (float)values[2];
		enabled = (bool)values[3];
	}

	public override object[] GetValues()
	{
		return new object[]{rectPos, mapPortionOfScreen, orthoCamRadiusFeet, enabled};
	}

	public MiniMapSettings()
	{
	}

}