using UnityEngine;
using System;
using System.Collections;


/// <summary>
/// Class containing accessors to system preferences
/// </summary>
public class Preferences
{
    /// <summary>
    /// Get/set the position of the display origin in the virtual world
    /// </summary>
	public Vector3 Position
	{
		get { return DIRE.Instance.DisplayOrigin.transform.position; }	
		set { DIRE.Instance.DisplayOrigin.transform.position = value; }
	}

    /// <summary>
    /// Get/set the orientation of the display origin in the virtual world
    /// </summary>
    public Vector3 Orientation
    {
        get { return DIRE.Instance.DisplayOrigin.transform.rotation.eulerAngles; }
        set { DIRE.Instance.DisplayOrigin.transform.rotation = Quaternion.Euler(value); }
    }

	public String DistanceUnits
	{
		get { return( LinearUnits.DefaultUnits ); }
		set { LinearUnits.DefaultUnits = value; }
	}

	public float MouseTranslationSpeed
	{
		get { return DIRE.Instance.DisplayOrigin.GetComponent<MouseNavigator>().translationSpeed; }
		set { DIRE.Instance.DisplayOrigin.GetComponent<MouseNavigator>().translationSpeed = value; }
	}
	/*
	public float[] PGTranslationSpeeds
	{
		get { return DIRE.Instance.DisplayOrigin.GetComponent<PointAndGo>().TranslationSpeeds; }
		set { DIRE.Instance.DisplayOrigin.GetComponent<PointAndGo>().TranslationSpeeds = value; }
	}

	public bool MiniMapEnabled
	{
		get { return DIRE.Instance.Minimap.GetComponent<MiniMapManager>().isMiniActive; }
		set { DIRE.Instance.Minimap.GetComponent<MiniMapManager>().SetMiniMapActive(value); }
	}

	public Vector2 MinimapPosition
	{
		get { return DIRE.Instance.Minimap.GetComponent<RectTransform>().anchoredPosition; }
		set { DIRE.Instance.Minimap.GetComponent<MiniMapManager>().SetMiniMapPosition(value); }
	}

	public int MiniMapPixelSize
	{
		get { return DIRE.Instance.Minimap.GetComponent<MiniMapManager>().pixelDimension; }
		set { DIRE.Instance.Minimap.GetComponent<MiniMapManager>().SetSize(value); }
	}

	public float MiniMapCameraRadius
	{
		get { return DIRE.Instance.Minimap.GetComponent<MiniMapManager>().orthoCamRadiusFeet; }
		set { DIRE.Instance.Minimap.GetComponent<MiniMapManager>().SetZoom(value); }
	}

	public bool ShowMiniMapControls
	{
		get { return DIRE.Instance.Minimap.GetComponent<MiniMapManager>().isControlActive; }
		set { DIRE.Instance.Minimap.GetComponent<MiniMapManager>().SetMiniMapControls(value); }
	}
*/
    /// <summary>
    /// Load preferences from a file.
    /// As preferences directly access data members in the DIRE scene and system,
    /// the mere act of loading the file will set the values in the system.
    /// </summary>
    /// <param name="path">Path to preferences file</param>
    /// <returns></returns>
    public static void Load(string path)
    {
        try
        {
            XmlIO.Load(path, typeof(Preferences));
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error loading: " + path + Environment.NewLine + ex);
        }
    }
}