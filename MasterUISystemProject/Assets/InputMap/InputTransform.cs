
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;


/// <summary>
/// Manages a list of input transformations.  A data value can be
/// modified by applying transforms in the sequence in order.
/// </summary>
public class TransformSequence : List<InputTransform>
{
    /// <summary>
    /// Apply the TransformationSequence to a given value and return the result.
    /// </summary>
    /// <param name="value">Input value to be transformed</param>
    /// <returns>The resultant value after the transformation</returns>
    public object Apply(object value)
    {
        /// Apply each InputTransform in order
        foreach (InputTransform t in this)
            value = t.Apply(value);

        // Return the result
        return (value);
    }
}

/// <summary>
/// This class represents a transformation that can be applied to an input value.
/// Transformation are encoded and decoded as strings that contain transformation
/// name and a list of arguments.  Transformations are defined in the Transformation
/// class which is used to find the actual method to call during the transformation
/// process.
/// </summary>
/// 
[XmlType("Transform")]
public class InputTransform
{
    /// <summary>
    /// The string representation of the transformation.
    /// </summary>
    [XmlText]
    public string Trans
    {
        get { return (Get()); }
        set { Set(value); }
    }

    /// <summary>
    /// Parsed method name of the transformation.
    /// </summary>
    private string MethodName { get; set; }

    /// <summary>
    /// List of argument values that will be passed to the transformation method.
    /// </summary>
    private object[] Args { get; set; }

    /// <summary>
    /// List of argument types that will be passed to the transformation method.
    /// </summary>
    private Type[] ArgTypes { get; set; }


    /// <summary>
    /// Default constructor
    /// </summary>
    public InputTransform()
    {
    }

    /// <summary>
    /// Constructor that initializes with a transfomation string
    /// </summary>
    /// <param name="val">Transformation string</param>
    public InputTransform(string val)
    {
        Set(val);
    }

    /// <summary>
    /// Set the transfomation with a new transformation string.
    /// This will intialize private data members needed to call the method
    /// associated with this transformation.
    /// </summary>
    /// <param name="newMethod">Transformation method string</param>
    private void Set(string newMethod)
    {
        /// Split the transformation method into the method and argument strings
        string[] s = newMethod.Split(' ');

        /// Save the method name.  Create arrays for argument values and types.
        MethodName = s[0];
        Args = new object[s.Length];
        ArgTypes = new Type[s.Length];

        /// Convert each argument string to a value and type
        for (int i = 1; i < s.Length; i++)
        {
            int intVal;
            float floatVal;

            /// try to convert argument to numeric (first int, then float).  If 
            /// conversion fails, leave argument as a string.
            if (Int32.TryParse(s[i], out intVal))
                Args[i] = intVal;
            else if (Single.TryParse(s[i], out floatVal))
                Args[i] = floatVal;
            else
                Args[i] = s[i];

            /// Save off the type of the argument.
            ArgTypes[i] = Args[i].GetType();
        }
    }

    /// <summary>
    /// Get the InputTransformation as a string.
    /// </summary>
    /// <returns>Representation of transofmation as string</returns>
    private string Get()
    {
        string retVal = null;

        /// Create a string with the method name and each argument
        if (Args != null && Args.Length > 0)
        {
            retVal = MethodName;
            for (int i = 1; i < Args.Length; i++)
                retVal += " " + Args[i].ToString();
        }

        return (retVal);
    }

    /// <summary>
    /// Apply the transformation to an input object.  This method uses the transfomation
    /// src object, method name, and argument list to search for an compatible method in 
    /// the Transformations class.  If one exists, the method is executed on the src
    /// and the result is returned.
    /// </summary>
    /// <param name="src">Source object on which to apply the transformation</param>
    /// <returns>The resultant object of the transformation</returns>
    public object Apply(object src)
    {
        object retVal = null;

        /// Use the src to fill in the first argument value and type to the arguments lists
        Args[0] = src;
        ArgTypes[0] = src.GetType();

        /// Look for a method that matches the name and argument list.
        MethodInfo method = typeof(Transformations).GetMethod(MethodName, ArgTypes);

        /// if the method was found, call it and save the return value
        if (method != null)
            retVal = method.Invoke(null, Args);
        else
        {
            /// Otherwise, throw and exception
            string eMessage = "Unknown Transformation: " + MethodName + "(";
            eMessage += ArgTypes[0];
            for (int i = 1; i < ArgTypes.Length; i++)
                eMessage += ", " + ArgTypes[i];
            eMessage += ")";

            throw new Exception(eMessage);
        }

        /// return the result
        return (retVal);
    }

    /// <summary>
    /// Comprehensive class containing all methods that can be used as Transformations
    /// on input data.
    /// </summary>
    public static class Transformations
    {
        /// <summary>
        /// Logical Not operator.
        /// </summary>
        /// <param name="src">source boolean</param>
        /// <returns></returns>
        public static bool Not(bool src)
        {
            return (!src);
        }

        /// <summary>
        /// Converts boolean to float.
        /// false = 0
        /// true = 1
        /// </summary>
        /// <param name="src">source boolean</param>
        /// <returns></returns>
        public static float AsFloat(bool src)
        {
            return (src ? 1f : 0f);
        }

        /// <summary>
        /// Scales a single float by scale
        /// </summary>
        /// <param name="src">source float</param>
        /// <param name="scale">scale value</param>
        /// <returns></returns>
        public static float Scale(float src, float scale)
        {
            return (src * scale);
        }

        /// <summary>
        /// Creates a "Dead Zone" around a source float.
        /// Any values withing range value of zero will be mapped to zero.
        /// Generally meant for joystick axis data that doesn't quite return to zero.
        /// </summary>
        /// <param name="src">source float</param>
        /// <param name="range">dead range from zero</param>
        /// <returns></returns>
        public static float DeadZone(float src, float range)
        {
            float retVal = src;

            if (src >= -Math.Abs(range) && src <= Math.Abs(range))
                retVal = 0;

            return (retVal);
        }

        /// <summary>
        /// Overwrites tracker position data.
        /// </summary>
        /// <param name="src">source tracker data</param>
        /// <param name="x">new x value</param>
        /// <param name="y">new y value</param>
        /// <param name="z">new z value</param>
        /// <returns></returns>
        public static TrackerData SetPosition(TrackerData src, float x, float y, float z)
        {
            src.Position = new Vector3(x, y, z);
            return (src);
        }

        /// <summary>
        /// Overwrites tracker rotation data
        /// </summary>
        /// <param name="src">source tracker data</param>
        /// <param name="x">new Euler x rotation</param>
        /// <param name="y">new Euler y rotation</param>
        /// <param name="z">new Ruler Z rotation</param>
        /// <returns></returns>
        public static TrackerData SetRotation(TrackerData src, float x, float y, float z)
        {
            src.Rotation = Quaternion.Euler(x, y, z);
            return (src);
        }

        /// <summary>
        /// Remaps position data to an alternate coordinate system.  See
        /// MapAxes for details.
        /// </summary>
        /// <param name="src">Input 3D position</param>
        /// <param name="x">New X Axis</param>
        /// <param name="y">New Y Axis</param>
        /// <param name="z">New Z Axis</param>
        /// <returns>Remapped position data</returns>
        private static Vector3 Remap(Vector3 src, int x, int y, int z)
        {
            float [] vals = { src.x, src.y, src.z };
            int [] idx = { x, y, z };
            float [] newVals = new float[3];

            for (int i = 0; i < 3; i++)
            {
                newVals[i] = vals[Math.Abs(idx[i]) - 1] * Math.Sign(idx[i]);
            }
            return (new Vector3(newVals[0], newVals[1], newVals[2]));
        }

        /// <summary>
        /// This method will remap coordinate system axes to another system.
        /// Axis numbers are X=1, Y=2, Z=3.  Positive numbers are positive axis 
        /// direction.  Negative are negative direction.
        /// 
        /// Example.
        /// The VRPN/Hydra data is returning: X:Left, Y:Back, Z:Down
        /// I need data (Unity) in X:Right, Y:Up, Z:Forward.
        /// So,  
        ///     X = -X: -1
        ///     Y = -Z: -3
        ///     Z = -Y: -2
        ///     
        /// I would call this function with -1, -3, -2
        /// 
        /// </summary>
        /// <param name="src">Tracker data to transform</param>
        /// <param name="x">New X axis</param>
        /// <param name="y">New Y axis</param>
        /// <param name="z">New Z axis</param>
        /// <returns></returns>
        public static TrackerData MapAxes(TrackerData src, int x, int y, int z)
        {
            src.Position = Remap(src.Position, x, y, z);

            // Not sure if this will work for rotation.

            float angle;
            Vector3 axis;

            src.Rotation.ToAngleAxis( out angle, out axis );
            axis = Remap(axis, x, y, z);
            src.Rotation = Quaternion.AngleAxis(angle, axis);

            return (src);
        }

        /// <summary>
        /// Scales the position values in TrackerData by a x,y,z scale value.
        /// </summary>
        /// <param name="src">Input TrackerData</param>
        /// <param name="sx">Scale in X</param>
        /// <param name="sy">Scale in Y</param>
        /// <param name="sz">Scale in Z</param>
        /// <returns></returns>
		public static TrackerData ScalePosition( TrackerData src, float sx, float sy, float sz )
		{
			src.Position = new Vector3(src.Position.x * sx,
			                           src.Position.y * sy,
			                           src.Position.z * sz);

			return(src);
		}

        /// <summary>
        /// Scales the rotation values in TrackerData by applying an x,y,z scale value
        /// to the rotations Euler angles.  The most useful application is reversing
        /// rotations by scaling by -1.  (Often used to convert between right handed
        /// and left handed coordinate systems.)
        /// </summary>
        /// <param name="src">Input TrackerData</param>
        /// <param name="x">X rotation scale</param>
        /// <param name="y">Y rotation scale</param>
        /// <param name="z">Z rotation scale</param>
        /// <returns></returns>
		public static TrackerData ScaleRotation(TrackerData src, float x, float y, float z)
		{
			src.Rotation = Quaternion.Euler(new Vector3 (src.Rotation.eulerAngles.x*x, src.Rotation.eulerAngles.y*y, src.Rotation.eulerAngles.z*z));

			return (src);
		}
    }
}