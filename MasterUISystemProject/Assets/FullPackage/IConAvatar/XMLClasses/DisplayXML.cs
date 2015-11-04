using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

/// <summary>
/// Class containing physical location information about a wall (position, orientation,
/// width, and height).  Units are in display system coordinates (usually tracker 
/// coordinates).
/// </summary>
public class Location
{
    /// <summary>
    /// Position of wall
    /// </summary>
    [XmlElement("Position")]
    public Vector3 Position = Vector3.zero;

    /// <summary>
    /// Orientation of wall in euler angles
    /// </summary>
    [XmlElement("Orientation")]
    public Vector3 Orientation = Vector3.zero;

    /// <summary>
    /// Width of wall
    /// </summary>
    [XmlElement("Width")]
    public float Width = 1.0f;

    /// <summary>
    /// Height of wall
    /// </summary>
    [XmlElement("Height")]
    public float Height = 1.0f;

    /// <summary>
    /// Default constructor
    /// </summary>
	public Location()
    {
	}
}

/// <summary>
/// Information on the displays viewport in screen coordinates (pixels).
/// </summary>
public class Viewport
{
    /// <summary>
    /// X position of display
    /// </summary>
	[XmlElement("X")]
    public int X = 0;

    /// <summary>
    /// Y position of display
    /// </summary>
	[XmlElement("Y")]
    public int Y = 0;

    /// <summary>
    /// Width of display
    /// </summary>
    [XmlElement("Width")]
    public int Width = Screen.currentResolution.width;

    /// <summary>
    /// Height of display
    /// </summary>
	[XmlElement("Height")]
	public int Height = Screen.currentResolution.height;

    /// <summary>
    /// Default constructor
    /// </summary>
	public Viewport()
    {
	}
}

/* ScreenInfo:
 * This class holds the corners and the viewport info for a single screen.
 */


/// <summary>
/// The ScreenInfo class contains all the information necessary to map virtual screen 
/// coordinates to a physical screen position.
/// </summary>
public class ScreenInfo
{
    /// <summary>
    /// Name of the screen.  Usually names like "Front", "Left", "Right", "Floor", etc.
    /// </summary>
    [XmlElement("Name")]
    public String Name = "Screen";

    /// <summary>
    /// Location of the physical screen space.  Units are usually in tracker coordinates.
    /// </summary>
	[XmlElement("Location")]
	public Location Location = new Location();

    /// <summary>
    /// Viewport location of display in screen space (eg. frame buffer) coordinates (pixels).
    /// </summary>
	[XmlElement("Viewport")]
	public Viewport Viewport = new Viewport();

    /// <summary>
    /// Swap eyes if necessary.  Deprecated.
    /// </summary>
	public bool SwapEyes = false;
		
    /// <summary>
    /// Default constructor.
    /// </summary>
	public ScreenInfo()
    {
	}
}

/// <summary>
/// Complete description of a multi-screen display setup.  Each screen has a physical
/// position, orientation, and size.  And, it has a virtual position and size in screen
/// space.
/// 
/// Current versions of Unity do not support multiple independent windows.  As a display
/// setup can have multiple screens.  A window large enough to encompass all screen areas
/// is created.  Individual Unity cameras are created in that window to draw the defined 
/// view.
/// </summary>
[XmlRoot("DisplaySettings")]
public class DisplaySettings
{
    /// <summary>
    /// List of all screens
    /// </summary>
    [XmlArray("Screens")]
    public List<ScreenInfo> screens;

    /// <summary>
    /// X position of window needed to encompass all screens
    /// </summary>
    [XmlIgnore]
    public int X
    {
        get
        {
			if ( screens.Count <= 0 )
				return( 0 );
			else
				return (screens.Min( s => s.Viewport.X));
		}
    }

    /// <summary>
    /// Y position of window needed to encompass all screens
    /// </summary>
    [XmlIgnore]
    public int Y
    {
        get
        {
			if ( screens.Count <= 0 )
				return( 0 );
			else
				return (screens.Min(s => s.Viewport.Y));
        }
    }

    /// <summary>
    /// Widith of window needed to encompass all screens
    /// </summary>
    [XmlIgnore]
    public int Width
    {
        get
        {
			if ( screens.Count <= 0 )
				return( 0 );
			else
			{
	            int max = screens.Max(s => s.Viewport.X + s.Viewport.Width);
	            return ( max - X );
			}
		}
    }

    /// <summary>
    /// Height of window needed to encompass all screens
    /// </summary>
    [XmlIgnore]
    public int Height
    {
        get
        {
			if ( screens.Count <= 0 )
				return( 0 );
			else
			{
	            int max = screens.Max(s => s.Viewport.Y + s.Viewport.Height);
	            return (max - Y);
			}
        }
    }

    /// <summary>
    /// Default constructor
    /// </summary>
	public DisplaySettings()
    {
        screens = new List<ScreenInfo>();
	}
}