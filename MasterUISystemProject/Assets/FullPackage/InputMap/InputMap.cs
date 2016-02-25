
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Manages a collection of input Devices.  Includes interfaces for saving and loading 
/// device configurations to disk and getting device (function) values.
/// </summary>
[XmlType("InputMap")]
public class InputMap
{
    /// <summary>
    /// List of Devices in the input map.
    /// </summary>
    [XmlElement]
    public List<Device> Devices = new List<Device>();



    /// <summary>
    /// Get a list of all the function keys in the input map.
    /// </summary>
    [XmlIgnore]
    public IEnumerable<string> Keys
    {
        get
        {
            IEnumerable<string> keys = (from device in Devices
                                        select device.Name).Distinct();
            return (keys);
        }
    }

	/// <summary>
	/// Configure all devices in the map.  This should only be called once.
	/// </summary>
	public void Configure()
	{
		// Call device configure method.
		foreach (Device device in Devices)
			device.Configure();
	}

    /// <summary>
    /// Append a map to the this map.
    /// </summary>
    /// <param name="subMap"></param>
    public void Append(InputMap subMap)
    {
        Devices.AddRange(subMap.Devices);
    }

    /// <summary>
    /// Get all the devices that implement a requested function.
    /// </summary>
    /// <param name="key">Function ID desired</param>
    /// <returns>Device enumerator</returns>
    public IEnumerable<Device> GetDevices(string key)
    {
        return (from device in Devices
                where device.Name.Equals(key)
                select device);
    }

    /// <summary>
    /// Get all the values from devices that implement a requested function.
    /// </summary>
    /// <param name="key">Function ID desired</param>
    /// <returns>Device values enumerator</returns>
    public IEnumerable<object> GetDeviceValues(string key)
    {
        return (from device in GetDevices(key)
                select device.GetValue());
    }

    /// <summary>
    /// Get the consolidated value of an input function from all devices.
    /// 
    /// Functions may be duplicated across devices, allowing a funciton
    /// to be triggered in multiple ways.  So, in addition to the gamepad button, the "fire"
    /// function can be assigned to a keyboard key.  Scripts that use the "fire" value will
    /// then trigger from the keyboard or the gamepad.  Value combinations are limited to 
    /// boolean devices (keyboard, buttons) and float devices (joystick axis).  Booleans will
    /// be 0.0 or 1.0 when converted to floats.  Floats will be either false or true (if the 
    /// float is greater than zero).  If multiple float devices are combined, the float with
    /// the largest absolute value will be returned.
    /// </summary>
    /// <param name="key">Function ID desired</param>
    /// <returns>Consolidated function value.  If values cannot be consolidated, the
    /// first value in the map is returned.</returns>
    /// 
    public object GetValue(string key)
    {
        /// final return value
        object retVal = null; 
 
        /// values of individual devices
        object[] values = GetDeviceValues(key).ToArray();

        /// cycle through each value and combine it with the final return value
        for (int i = 0; i < values.Count(); i++)
        {
            object val = values[i];

            /// combine the individual value with the return value, or
            /// set the return value if null.
            
            if (retVal == null)
                retVal = val;
            else
            {
                /// combinations of float will always return a float
                /// by converting either the individual or return value
                if (retVal is bool && val is float)
                    retVal = Convert.ToSingle(retVal);
                
                if (retVal is float && val is bool)
                    val = Convert.ToSingle(val);

                /// Combine two floats by returning the value with the greatest magnitude
                /// This is similar to Unity's default behavior.
                if (retVal is float && val is float &&
                     Mathf.Abs((float)val) > Mathf.Abs((float)retVal))
                    retVal = val;

                /// Combine bools with OR operation
                if (retVal is bool && val is  bool)
                    retVal = (bool)retVal | (bool)val;
            }
        }

        return (retVal);
    }
}

