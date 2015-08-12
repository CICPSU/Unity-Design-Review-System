using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

public class WidgetSettings {

	public string gameobjectName;	
	public float xPos;
	public float yPos;
	public float xSize;
	public float ySize;

	public virtual void ApplySettings()
	{
		GameObject gO = GameObject.Find (gameobjectName);
		RectTransform rectTrans = gO.GetComponent<RectTransform> ();
		Debug.Log (rectTrans);
		rectTrans.position = new Vector3 (xPos, yPos, 0);
		rectTrans.sizeDelta = new Vector2 (xSize, ySize);
	}

	public WidgetSettings()
	{
		gameobjectName = "name";
	}

}
