using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;


public abstract class TrackDDevice : Device
{
}

[XmlType("TrackDTracker")]
public class TrackDTracker : TrackDDevice
{
	public int Key { get; set; }
	public int Sensor { get; set; }

	private TrackDNet.TrackdTracker device = null;

	public override void Configure()
	{
		device = new TrackDNet.TrackdTracker( Key );
		device.connect();
	}

	public override object GetRawValue()
	{
		Vector3 position = new Vector3( device.X(Sensor), 
		                                device.Y(Sensor), 
		                                device.Z(Sensor) );
		Vector3 eulerAngles = new Vector3( device.H(Sensor), 
		                                   device.P(Sensor), 
		                                   device.R(Sensor) );
		TrackerData retVal = new TrackerData( position,
 		                                      Quaternion.Euler( eulerAngles ) );
		return( retVal );
	}

	public override string ToString()
	{
		return GetType().Name + "(" + Key + ":" + Sensor +")";
	}
}

[XmlType("TrackDAxis")]
public class TrackDAxis : TrackDDevice
{
	public int Key { get; set; }
	public int Index { get; set; }

	private TrackDNet.TrackdController device = null;

	public override void Configure ()
	{
		device = new TrackDNet.TrackdController( Key );
		device.connect();
	}

	public override object GetRawValue()
	{
		return( (float) device.GetValuator( Index ) );
	}

	public override string ToString()
	{
		return GetType().Name + "(" + Key + ":" + Index +")";
	}
}

[XmlType("TrackDButton")]
public class TrackDButton : TrackDDevice
{
	public int Key { get; set; }
	public int Index { get; set; }

	private TrackDNet.TrackdController device = null;

	/// <summary>
	/// Trigger toggle state that represents "true" on the device.
	/// </summary>
	public ToggleState Trigger { get; set; }
	
	/// <summary>
	/// The value of button on the last frame.
	/// </summary>
	bool lastValue = false;
	
	/// <summary>
	/// The frame number of the last frame.
	/// </summary>
	int lastFrame = 0;
	
	/// <summary>
	/// Thr return value of the button on the last frame.
	/// </summary>
	bool lastRetVal = false;

	public override void Configure ()
	{
		device = new TrackDNet.TrackdController( Key );
		device.connect();
	}
	
	public override object GetRawValue()
	{
		/// Get current device state
		bool curVal = device.GetButton(Index);
		int  curFrame = Time.frameCount;
		bool retVal = false;
		
		/// If the value has already been retreived this frame, the return
		/// the result.
		if ( curFrame == lastFrame )
			retVal = lastRetVal;
		else
		{
			/// Otherwise get the value of the button and save state.
			if ( Trigger == ToggleState.Value )
				retVal = curVal;
			else if ( Trigger == ToggleState.Press )
				retVal = curVal && !lastValue;
			else if ( Trigger == ToggleState.Release )
				retVal = !curVal && lastValue;
			else
				retVal = false;
			
			lastValue = curVal;
			lastFrame = curFrame;
			lastRetVal = retVal;
		}
		
		/// return the button value.
		return( retVal );
	}

	public override string ToString()
	{
		return GetType().Name + "(" + Key + ":" + Index +")";
	}
}
