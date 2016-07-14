using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System;

/// <summary>

/// </summary>
public abstract class WidgetSettings {

	//All implementation of the ApplySettings() should:
	//reference the widget through FindObjectsWithType([NAME OF WIDGET MANAGER])
	public bool enabled = true;
    public string enabledDescription = "Bool that is true when the widget should be active.";
	public abstract void ApplySettings ();

	public abstract void SetValues (object[] values);

	public abstract object[] GetValues ();

	public WidgetSettings ()
	{
	}
}
/*
public class AvatarSettings : WidgetSettings
{

    public bool trackingEnabled;
    public float avatarForwardSpeed;
    public float avatarBackwardSpeed;
    public float avatarStrafeSpeed;

    public override void ApplySettings()
    {
		if(DIRE.Instance.DisplayOrigin.activeSelf)
			DIRE.Instance.DisplayOrigin.GetComponent<ARTtrack>().SetTracking(trackingEnabled);
        TP_Motor tpMotorScript = GameObject.FindObjectOfType<TP_Motor>();
        tpMotorScript.ForwardSpeed = avatarForwardSpeed;
        tpMotorScript.BackwardSpeed = avatarBackwardSpeed;
        tpMotorScript.StrafingSpeed = avatarStrafeSpeed;

    }

    public override void SetValues(object[] values)
    {
		trackingEnabled = (bool)values [0];
		avatarForwardSpeed = (float)values [1];
		avatarBackwardSpeed = (float)values [2];
		avatarStrafeSpeed = (float)values [3];
    }

    public override object[] GetValues()
    {
		return (new object[]{trackingEnabled, avatarForwardSpeed, avatarBackwardSpeed, avatarStrafeSpeed, enabled});
    }

    public AvatarSettings()
    {
    }

}
*/
public class MiniMapSettings : WidgetSettings {

	public Vector2 rectPos = Vector2.zero;
    public string rectPosDescription;

	public float mapPortionOfScreen = 0.2f;
    public string mapPortionDescription = "";

	public float orthoCamRadiusFeet = 5;
    public string orthoCamRadiusDescription = "";

	public override void ApplySettings()
	{
		if (GameObject.FindObjectOfType<MiniMapManager> () == null)
			return;
		GameObject gO = GameObject.FindObjectOfType<MiniMapManager>().gameObject;

        gO.transform.GetChild(0).gameObject.SetActive(enabled);
        gO.transform.GetChild(1).gameObject.SetActive(enabled);
        if (!enabled)
            return;
        MiniMapManager miniMapManagerRef = gO.GetComponent<MiniMapManager>();
        RectTransform rectTrans = miniMapManagerRef.miniMapPanel.GetComponent<RectTransform> ();
		rectTrans.anchoredPosition = rectPos;

        miniMapManagerRef.mapProportionOfScreen = mapPortionOfScreen;
        miniMapManagerRef.orthoCamRadiusFeet = orthoCamRadiusFeet;
        miniMapManagerRef.SetMiniMapCam ();
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
		return new object[]{rectPos, rectPosDescription,
            mapPortionOfScreen, mapPortionDescription,
            orthoCamRadiusFeet, orthoCamRadiusDescription,
            enabled, enabledDescription};
	}

	public MiniMapSettings()
	{
	}
}

public class POISettings : WidgetSettings {

	public override void ApplySettings(){
        GameObject gO = GameObject.FindObjectOfType<POI_ReferenceHub>().gameObject;

        gO.GetComponent<POI_ReferenceHub>().poiCanvas.gameObject.SetActive(enabled);
        gO.GetComponent<POI_ReferenceHub>().markerRoot.gameObject.SetActive(enabled);
    }

	public override void SetValues(object[] values){
        enabled = (bool)values[0];
	}

	public override object[] GetValues()
	{
		return new object[]{enabled};
	}

    public POISettings()
    {
    }
}