using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

public class Location
{
    [XmlElement("Position")]
    public Vector3 Position = Vector3.zero;

    [XmlElement("Orientation")]
    public Vector3 Orientation = Vector3.zero;

    [XmlElement("Width")]
    public float Width = 1.0f;

    [XmlElement("Height")]
    public float Height = 1.0f;

	public Location()
    {
	}
}

/* Viewport:
 * This class holds the viewport information, specifically the x and y position of the viewport and its width and height.
 */

public class Viewport
{
	[XmlElement("X")]
    public int X = 0;

	[XmlElement("Y")]
    public int Y = 0;

    [XmlElement("Width")]
    public int Width = Screen.currentResolution.width;

	[XmlElement("Height")]
	public int Height = Screen.currentResolution.height;

	public Viewport()
    {
	}
}

/* ScreenInfo:
 * This class holds the corners and the viewport info for a single screen.
 */

public class ScreenInfo
{
    [XmlElement("Name")]
    public String Name = "Screen";

	[XmlElement("Location")]
	public Location Location = new Location();

	[XmlElement("Viewport")]
	public Viewport Viewport = new Viewport();

	public ScreenInfo()
    {
	}
}

/* DisplaySettings:
 * This class holds the information for a overall display setup.  It has a list of ScreenInfo objects that give the information for each screen in the display system, the total width
 * and the total height of the display system.
 */
[XmlRoot("DisplaySettings")]
public class DisplaySettings
{
    [XmlArray("Screens")]
    public List<ScreenInfo> screens;

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

	//pixel width of display system
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

    [XmlIgnore]
    public static DisplaySettings DefaultDisplay { get; set; }

    static DisplaySettings()
    {
        DefaultDisplay = new DisplaySettings();
        /*
        Default.screens = new List<ScreenInfo>();

        ScreenInfo screen = new ScreenInfo();
        screen.Name = "Front";
	
        screen.Location = new Location();
        screen.Location.Position = Vector3.zero;
        screen.Location.Orientation = Vector3.zero;
        screen.Location.Width = 1;
        screen.Location.Height = 1;

        screen.Viewport = new Viewport();
        screen.Viewport.X = 0;
        screen.Viewport.Y = 0;
        screen.Viewport.Width = Screen.currentResolution.width;
        screen.Viewport.Height = Screen.currentResolution.height;

        Default.screens.Add(screen);
        */
    }

	public DisplaySettings()
    {
        screens = new List<ScreenInfo>();
	}
}