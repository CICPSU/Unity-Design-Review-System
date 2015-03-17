using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;

/* DisplaySetup:
 * 
 * This class handles the loading of display settings files and creating cameras. The display settings file that was passed on the command line is deserialized
 * and used to instantiate a number of cameras.  The info in the display settings file allows us to capture the view that the user would see when looking through
 * the display into the virtual world.
 * 
 * Projection matrix calculations and modifications will occur in this script in the Update function once that code is working.  The projection matrix will be based off
 * of data collected from a tracker and the screen data from the display settings file.  I received code from the Unity Beta forums on 8/7/2014 that appears to work very well.
 * I will need to test this code and then integrate it into the current camera system.  It is located in a project on my desktop in a folder called TestCluster - Simple.
 */

public class DisplaySetup : MonoBehaviour 
{
	// baseCam: camera. The camera used to instantiate all of the cameras from the display settings file.  Included as a prefab in the DIRE project.
	public Camera baseCam;

	//the position of the geometric center, which is passed to ARTtrack.cs. Head will be positioned here when no tracking data
	public static Vector3 displayGeometricCenter;

	// Functions imported from the user32.dll to set the application window position.
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern IntPtr FindWindow(System.String className, System.String windowName);

	/* SetPosition: sets the application window position.
	 * Only need to pass the x and y position of the window.
	 * Currently sets the window position to (0,0) everytime.  Will be updated to set the window position using infor from the DisplaySettings object once
	 * those member have been added to the class and config files.
	*/
	public static void SetPosition(int x, int y, int resX, int resY) 
    {
		SetWindowPos(FindWindow(null, DIRE.WindowName), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
	}
	#endif

	/* old version
	 * public static void SetPosition(int x, int y, int resX = 0, int resY = 0) {
	 *	SetWindowPos(FindWindow(null, "DIRE"), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
	 *}
	 */

	

	public DisplaySettings LoadDisplayDefinition()
	{
		// default display name
		string displayToLoad = "IConDisplay";

		// Look for display file specified in command line[command: -display filename]
		if (ArgumentProcessor.CmdLine.ContainsKey("display") && 
		    ArgumentProcessor.CmdLine["display"].Count > 0 )
			displayToLoad = ArgumentProcessor.CmdLine["display"][0];

		//displaySettings class is inside DisplayXML.cs, it stores all the info for the display system(screen pos, size, orientation)
		//updated by xml configuration file. 
		DisplaySettings settings = null;

		try
		{
			Debug.Log("Searching for " + displayToLoad);
			List<string> displayPaths = FileSearch.DisplaySearch.FindFile(displayToLoad + ".xml");

			if ( displayPaths.Count <= 0 )
				throw new Exception("Cannot find " + displayToLoad );

			string loadPath = displayPaths.Last();

			Debug.Log("Loading: " + loadPath );
			//XmlIO.cs is a wrapper class that serialize and deserialize the xml.
			//has load and save call, and two other calls.
			settings = XmlIO.Load( loadPath, typeof(DisplaySettings) ) as DisplaySettings;
		}
		catch ( Exception ex )
		{
			Debug.LogError( ex.Message );
			Debug.LogWarning( "Unable to load display: " + displayToLoad );
			Debug.LogWarning( "Using default display" );

			settings = DisplaySettings.DefaultDisplay; //default display is an empty displaysettings class(see displayXML.cs), it does not containa any screen information.
		}

		XmlIO.Save( settings, "_Display.xml" );

		return( settings );
	}

	/* 
	 * Used to deserialize the display settings .xml file and use that info to instantiate cameras.
	 * This function also destroys all non-DIRE cameras in the scene.
	 * SetPosition is used to set the position of the application window.
	 */
    public void InitializeDisplay(DisplaySettings settings)
    {
		if ( settings.screens.Count <= 0 )
		{
			Debug.LogWarning( "No screens defined, reverting to default display mode" );
			return;
		}

		Camera[] camsToDestroy = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];

		Debug.Log( "Setting Window Position: " + 
		           "(" + settings.X + ", " + settings.Y + ") " + 
		           settings.Width + "x" + settings.Height );

		//Screen.SetResolution( settings.Width, settings.Height, false );
		//SetPosition( settings.X, settings.Y, settings.Width, settings.Height );

		GameObject wallsParent = DIRE.Instance.DisplayOrigin;
        Debug.Log("Wall Parent: " + wallsParent);

		bool audioFound = false;
		// Instantiate a camera for every screen from the display settings file.  
		for( int i=0; i<settings.screens.Count; i++ )
        {
            ScreenInfo sInfo = settings.screens[i];

            // Create the wall
            GameObject wallObject = new GameObject();
            wallObject.name = sInfo.Name + " Wall";

			wallObject.transform.parent = wallsParent.transform;
			wallObject.transform.localPosition = sInfo.Location.Position;
            wallObject.transform.localEulerAngles = sInfo.Location.Orientation;

            wallObject.AddComponent(typeof(WallDefinition));
            WallDefinition wallDef = wallObject.GetComponent(typeof(WallDefinition)) as WallDefinition;
            wallDef.Width = sInfo.Location.Width;
            wallDef.Height = sInfo.Location.Height;

			Camera camera = Instantiate(baseCam) as Camera;
            camera.name = sInfo.Name + " Camera";
            camera.transform.parent = DIRE.Instance.Head.transform;
			camera.transform.localEulerAngles = Vector3.zero;
			camera.transform.localPosition = Vector3.zero;

			camera.rect = new Rect( (float) (sInfo.Viewport.X - settings.X ) / settings.Width,
			                        (float) (sInfo.Viewport.Y - settings.Y ) / settings.Height,
			                        (float) (sInfo.Viewport.Width) / settings.Width,
			                        (float) (sInfo.Viewport.Height) / settings.Height);

			if ( camera.GetComponent( typeof( FixedDisplay )) == null )
				camera.gameObject.AddComponent( typeof( FixedDisplay ) );

			FixedDisplay fdScript = camera.GetComponent(typeof(FixedDisplay)) as FixedDisplay;
            fdScript.WallObject = wallObject;

			if ( camera.GetComponent( typeof( AudioListener ) ) != null )
			{
				if ( audioFound )
					Destroy( camera.GetComponent( typeof( AudioListener ) ) );
				audioFound = true;
			}

		}

		//deletes all other camera when a display configuration is loaded
		// This foreach destroys all of the non-DIRE cameras in the scene. 
        // This is performed after the new cameras are set up to prevent a blank screen while the new cameras are set up.
		foreach(Camera obj in camsToDestroy){
			if(obj.tag == "Untagged"){
				Debug.Log("Camera " + obj.name + " deleted, as it's not tagged");
				Destroy(obj.gameObject);
			}
		}

		//**
	}// LoadDisplays
	


}// DisplayLoader