using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WidgetSettingsBase : ScriptableObject{

	public GameObject widget;

	public float xPos;
	public float yPos;
	public float xSize;
	public float ySize;

	public virtual void Initialize()
	{
		RectTransform thisRectTrans = widget.GetComponent<RectTransform> ();
		thisRectTrans.position = new Vector3 (xPos,yPos,0);
		thisRectTrans.sizeDelta = new Vector2 (xSize, ySize);
	}
}
