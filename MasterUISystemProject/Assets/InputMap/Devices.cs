
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Abstract base class for devices that are compatible with the InputMap.  
/// 
/// Devices must implement the GetRawValue() method to return device specific data.  
/// Devices are individual inputs from a more complex physical device.  For instance,
/// a standard gamepad controller would have several axis inputs (from joysticks) and
/// several button inputs (from buttons).  If the gamepad had some sort of 3 or 6 DOF 
/// tracking capability it may also provide tracing data.
/// 
/// Devices are assinged a name, device specific parameters, and a transformation 
/// sequence.
/// 
/// The name should be assigned as "function" identifier that the input controls.  
/// For instance, the "fire" function might be assigned to button on a gamepad 
/// controller. 
/// 
/// Devices can contain a list of transformations.  The tranformations are executed in
/// order and can convert data from a device into another format.  Available transformations
/// are defined in the transformation class.
/// 
/// Device specifications can be loaded as XML files.  New devices must be included
/// in this class with an XmlInclude attribute.
/// </summary>

[XmlType("Device")]
public abstract class Device
{
    /// <summary>
    /// Name of the device function.  This can be anything the designer chooses.  Function
    /// names are used to retrieve device values within scripts.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Transformation sequence.  Used to transform raw device data into another format.
    /// Useful for translation, rotation, scale, inversion, type conversion, etc.
    /// </summary>
    public TransformSequence Sequence = new TransformSequence();

    /// <summary>
    /// Default Device constructor.
    /// </summary>
    public Device()
    {
        Name = "Unnamed Device";
    }

    /// <summary>
    /// Configuration method for device.  This method is called after the device has
    /// been instantited and is used to initialize and activate the device, if 
    /// needed.
    /// </summary>
    public virtual void Configure() { }

    /// <summary>
    /// Abstract method to return raw data from the device.  Data format is dependent 
    /// on the device itself.
    /// </summary>
    /// <returns>Raw (untransformed) device data</returns>
    public abstract object GetRawValue();

    /// <summary>
    /// Get the device data (value) after it has been modified by the transformation
    /// sequence.  This is the interface that will most often be used.
    /// </summary>
    /// <returns></returns>
    public object GetValue()
    {
        return (Sequence.Apply(GetRawValue()));
    }
}

/// <summary>
/// A device accessed through the VRPN protocol.
/// </summary>
[XmlType("VRPN")]
public abstract class VRPN : Device
{
    /// <summary>
    /// Device id as defined on the VRPN server.
    /// EG. "Tracker0", "Controller", etc.
    /// </summary>
    public string Device { get; set; }

    /// <summary>
    /// IP name or address of the server location.  Use localhost for the local host.
    /// EG.  localhost, 192.168.1.2, device.domain.suffix
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    /// The index number of the particular data type requested.  For instance, if 
    /// requesting tracker data, 2 would give the tracker at index 2.  Indexes are
    /// zero relative.
    /// </summary>
    public int Index { get; set; }

	public abstract ClusterInputType InputType {get;}

    /// <summary>
    /// A private ID string for the ClusterInput class to access the VRPN device.
    /// </summary>
	public string id = Guid.NewGuid().ToString();

    /// <summary>
    /// Public constructor
    /// </summary>
    public VRPN()
    {
        /// Initialize default values
		Name = GetType().Name;
        Device = "DeviceName";
        Server = "localhost";
        Index = 0;
    }

    /// <summary>
    /// Return description of device as string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
		return GetType().Name + "(" + Device + "@" + Server + ", " + Index + ")";
    }

    /// <summary>
    /// Configure the VRPN device with the Unity ClusterInput service.
    /// </summary>
    public override void Configure()
    {
        /// Addes the VRPN device to the Unity Cluster Input class.
		ClusterInput.AddInput(id.ToString(), Device, Server, Index, InputType);
    }
}

/// <summary>
/// Interface to a VRPN button through the ClusterInput class.
/// The device retains state on a per frame basis for detecting presses and releases.
/// It can also return the value of the button itself.
/// </summary>
[XmlType("VRPNButton")]
public class VRPNButton : VRPN
{
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

	/// <summary>
	/// Convert the device definition to a string
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	/// <filterpriority>2</filterpriority>
	public override string ToString()
	{
		return GetType().Name + "(" + Device + "@" + Server + ", " + Index + ", " + Trigger +")";
	}

	/// <summary>
	/// The ClusterInputType of device.
	/// </summary>
	/// <value>The type of the input.</value>
	public override ClusterInputType InputType 
	{
		get 
		{
			return( ClusterInputType.Button );
		}
	}

	/// <summary>
	/// Abstract method to return raw data from the device. 
	/// Returns the trigger based raw device value.
	/// </summary>
	/// <returns>Raw (untransformed) device data</returns>
	public override object GetRawValue()
	{
		/// Get current device state
		bool curVal = ClusterInput.GetButton(id);
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
}

/// <summary>
/// A VRPN (ClusterInput) device.
/// </summary>
[XmlType("VRPNAxis")]
public class VRPNAxis : VRPN
{

	/// <summary>
	/// Gets the ClusterInputType of the device.
	/// </summary>
	/// <value>The type of the input.</value>
	public override ClusterInputType InputType 
	{
		get 
		{
			return( ClusterInputType.Axis );
		}
	}

	/// <summary>
	/// Return the value of the axis.
	/// </summary>
	/// <returns>Axis value</returns>
	public override object GetRawValue()
	{
		return( ClusterInput.GetAxis(id) );
	}
}

/// <summary>
/// VRPN tracker device (ClusterInput).
/// </summary>
[XmlType("VRPNTracker")]
public class VRPNTracker : VRPN
{
	/// <summary>
	/// Gets the ClusterInputType of the device.
	/// </summary>
	/// <value>The type of the input.</value>
	public override ClusterInputType InputType 
	{
		get 
		{
			return( ClusterInputType.Tracker );
		}
	}

	/// <summary>
	/// Return the tracker data.
	/// </summary>
	/// <returns>Raw (untransformed) device data</returns>
	public override object GetRawValue()
	{
		return( new TrackerData(ClusterInput.GetTrackerPosition(id),
		                        ClusterInput.GetTrackerRotation(id)) );
	}
}

/// <summary>
/// Six Degree of Freedom (DOF) data from VRPN tracker devices.  Contains position
/// and orientation information (when available).
/// </summary>
public class TrackerData
{
    /// <summary>
    /// Cartesian position data.  Coordinate system, units and ranges are defined 
    /// by the individual VRPN device
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Orientation data as defined by the individual VRPN device.
    /// </summary>
    public Quaternion Rotation { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public TrackerData()
    {
    }

    /// <summary>
    /// Constructor with initial values.
    /// </summary>
    /// <param name="initialPosition">Initial position data</param>
    /// <param name="initialRotation">Initial rotation data</param>
    public TrackerData(Vector3 initialPosition, Quaternion initialRotation)
    {
        Position = initialPosition;
        Rotation = initialRotation;
    }

    /// <summary>
    /// Convert data to string format.
    /// </summary>
    /// <returns>String representation of device.</returns>
    public override string ToString()
    {
        return (Position.ToString() + ":" + Rotation.eulerAngles.ToString());
    }
}

/// <summary>
/// Static tracking device.  Creates an input device that returns a preset
/// position and orientation as tracker data.
/// </summary>
[XmlType("StaticTracker")]
public class StaticTracker : Device
{
    /// <summary>
    /// Device position as defined by usage and contents.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Device rotation as defined by usage and contents.
    /// </summary>
    public Vector3 Rotation { get; set; }
    
    /// <summary>
    /// Return the device as a string.
    /// </summary>
    /// <returns>String representation of device.</returns>
    public override string ToString()
    {
        return "StaticTracker(" + Name + ")";
    }

    /// <summary>
    /// Get device raw data as TrackerData.
    /// </summary>
    /// <returns>Return position and orientation as TrackerData</returns>
    public override object GetRawValue()
    {
        return (new TrackerData(Position, Quaternion.Euler(Rotation)));
    }
}

/// <summary>
/// Trigger states for toggle (boolean) devices.
/// States include...
///     Value - current value of boolean device.
///     Press - true when transitioning from false to true (leading edge)
///     Release - true when transitioning from true to false (trailing edge)
/// </summary>
public enum ToggleState
{
    Value, Press, Release
}

/// <summary>
/// Keyboard key device.
/// </summary>
[XmlType("Key")]
public class Key : Device
{
    /// <summary>
    /// Key device key code as defined by Unity.KeyCode class.
    /// </summary>
    public KeyCode KeyCode { get; set; }

    /// <summary>
    /// Trigger toggle state that represents "true" on the device.
    /// </summary>
    public ToggleState Trigger { get; set; }

    /// <summary>
    /// Public constructor
    /// </summary>
    public Key()
    {
        /// Initialize default values.
        Name = "Key Device";
        KeyCode = UnityEngine.KeyCode.Space;
        Trigger = ToggleState.Press;
    }

    /// <summary>
    /// Convert device to string.
    /// </summary>
    /// <returns>String representing device</returns>
    public override string ToString()
    {
        return "Key(" + KeyCode + ", " + Trigger + ")";
    }

    /// <summary>
    /// Get the raw value of the device.  
    /// Value is dependent on triggering ToggleState.
    /// </summary>
    /// <returns>Device value (key state)</returns>
    public override object GetRawValue()
    {
        /// initialize default value
        bool value = false;

        /// get value based on ToggleState requested.
        if (Trigger == ToggleState.Value)
            value = Input.GetKey(KeyCode);
        else if (Trigger == ToggleState.Press)
            value = Input.GetKeyDown(KeyCode);
        else if (Trigger == ToggleState.Release)
            value = Input.GetKeyUp(KeyCode);

        /// return value
        return (value);
    }
}
