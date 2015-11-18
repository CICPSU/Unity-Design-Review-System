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
	public abstract void ApplySettings ();

	public abstract void SetValues (object[] values);

	public abstract object[] GetValues ();

	public WidgetSettings ()
	{
	}
}

public class AvatarSettings : WidgetSettings
{

    public bool trackingEnabled;
    public float avatarForwardSpeed;
    public float avatarBackwardSpeed;
    public float avatarStrafeSpeed;

    public override void ApplySettings()
    {
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

public class MiniMapSettings : WidgetSettings {

	public Vector2 rectPos = Vector2.zero;

	public float mapPortionOfScreen;
	public float orthoCamRadiusFeet;
	
	public override void ApplySettings()
	{
		if (GameObject.FindObjectOfType<MiniMapManager> () == null)
			return;
		GameObject gO = GameObject.FindObjectOfType<MiniMapManager>().gameObject;

        gO.transform.GetChild(0).gameObject.SetActive(enabled);
        gO.transform.GetChild(1).gameObject.SetActive(enabled);
        if (!enabled)
            return;
        RectTransform rectTrans = gO.GetComponentInChildren<RectTransform> ();
		rectTrans.anchoredPosition = rectPos;

		gO.GetComponent<MiniMapManager> ().mapProportionOfScreen = mapPortionOfScreen;
		gO.GetComponent<MiniMapManager> ().orthoCamRadiusFeet = orthoCamRadiusFeet;
		gO.GetComponent<MiniMapManager> ().SetMiniMapCam ();
        
		//gO.SetActive (enabled);
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

public class POISettings : WidgetSettings {

	public override void ApplySettings(){
        GameObject gO = GameObject.FindObjectOfType<POI_ReferenceHub>().gameObject;

        gO.transform.GetChild(0).gameObject.SetActive(enabled);
        gO.transform.GetChild(1).gameObject.SetActive(enabled);
    }

	public override void SetValues(object[] values){
        enabled = (bool)values[0];
	}

	public override object[] GetValues()
	{
		return new object[]{enabled};
	}

}