using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class InputDebugger : MonoBehaviour
{
	/// <summary>
	/// Show input debug flag.
	/// </summary>
	public bool ShowInputDebug = false;

	public enum VRPNTimelineType
	{
		Off, FromClusterInput, FromDeviceClassRaw, FromDeviceClassTransformed
	}

	public VRPNTimelineType VRPNTimeline = VRPNTimelineType.Off;

	/// <summary>
	/// Input debug draw area
	/// </summary>
	private Rect DebugInputArea;


	private List<TimeLine> timelines = new List<TimeLine>();


	// Use this for initialization
	void Start () 
	{
		int width = 400;
		int height = 500;

		DebugInputArea = new Rect( Screen.width - width, 0, width, height );
	}
	
	// Update is called once per frame
	void Update () 
	{
		List <Device> vrpnDevices = (from device in InputManager.map.Devices
			                     where device is VRPN
				                 select device).ToList();

		if ( VRPNTimeline == VRPNTimelineType.Off || 
		     timelines.Count != vrpnDevices.Count() )
		{
			foreach( TimeLine iv in timelines )
				Destroy( iv.gameObject );
			timelines.Clear();
		}

		if ( VRPNTimeline != VRPNTimelineType.Off )
		{
			if ( timelines.Count < vrpnDevices.Count )
			{

				for( int i=0; i<vrpnDevices.Count; i++ )
				{
					GameObject plot = new GameObject();
					plot.transform.parent = transform;
					plot.name = vrpnDevices[i].ToString();

					TimeLine iv = plot.AddComponent<TimeLine>();
					timelines.Add( iv );

					plot.transform.localPosition = 
						new Vector3( 0, -i*.3f );                 
				}
			}

			for( int i=0; i<vrpnDevices.Count; i++ )
			{
				// Only VRPN devices are shown.
				VRPN device = vrpnDevices[i] as VRPN;

				float value = 0f;

				if ( VRPNTimeline == VRPNTimelineType.FromClusterInput )
				{
					if ( device.InputType == ClusterInputType.Axis )
						value = ClusterInput.GetAxis( device.id );

					if ( device.InputType == ClusterInputType.Button )
						value = ClusterInput.GetButton( device.id ) ? 1f : 0f;
				}
				else
				{
					object valueObject = null;

					if ( VRPNTimeline == VRPNTimelineType.FromDeviceClassRaw )
						valueObject = device.GetRawValue();

					if ( VRPNTimeline == VRPNTimelineType.FromDeviceClassTransformed )
						valueObject = device.GetValue();

					value = (device.GetRawValue() as float?).GetValueOrDefault();

					if ( valueObject is bool )
						value = ( valueObject as bool? ).GetValueOrDefault() ? 1.0f : 0f;
				}

				timelines[i].AddPt( value );
			}
		}
	}

	/// <summary>
	/// OnGUI draw method.  Draw debug if requested.
	/// </summary>
	void OnGUI()
	{
		/// Draw input debug
		if ( ShowInputDebug ) 
			DrawInputDebug();
	}
	
	/// <summary>
	/// Draw input debug if requested
	/// </summary>
	private void DrawInputDebug()
	{
		InputMap map = InputManager.map;
		

		if ( map == null )
			return;

		StringWriter writer = new StringWriter();
		
		writer.WriteLine("Devices");
		writer.WriteLine("-------");
		foreach (Device d in map.Devices)
		{
			writer.WriteLine(d.ToString() + ": " + d.GetRawValue() + "=>" + d.GetValue());
		}
		
		writer.WriteLine();
		writer.WriteLine("Functions");
		writer.WriteLine("---------");
		foreach (string s in map.Keys)
		{
			writer.Write(s + ": ");
			foreach (object val in map.GetDeviceValues(s))
				writer.Write(val.ToString() + " ");
			writer.WriteLine("=> " + map.GetValue(s));
		}
		GUI.Box(DebugInputArea, writer.ToString());
	}
}
