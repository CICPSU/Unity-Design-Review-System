using UnityEngine;
using UnityEngine.VR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;


/// <summary>
/// This class initializes a display setup at runtime from an XML file.  Display settings include the physical 
/// position and orientation of one or more display walls as well as their screen space in the display frame.
/// </summary>

public class DisplaySetup : MonoBehaviour 
{
	/// <summary>
    /// A reference to the Camera prefab that will be used to instantiate a per display surface camera.
    /// </summary>
    public Camera baseCam;

	// Functions imported from the user32.dll to set the application window position.
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR

	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
	
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern IntPtr FindWindow(System.String className, System.String windowName);

    /// <summary>
    /// Sets the window position and size of the Unity application
    /// </summary>
    /// <param name="x">X position of window in pixels</param>
    /// <param name="y">Y position of window in pixels</param>
    /// <param name="resX">Window width in pixels</param>
    /// <param name="resY">Window heigh in pixels</param>
	public static void SetPosition(int x, int y, int resX, int resY) 
    {
		/// Weird to have to do two things.
		/// Looks like Unity won't know that you've resized the screen unless you call
		/// its SetResolution method.  But, you can't set the Window position, so you
		/// need to use the Windows call.
		Screen.SetResolution( resX, resY, false );
		SetWindowPos( FindWindow(null, DIRE.WindowName), 0, 
                      x, y, resX, resY, 0x0040);
	}

	#endif

	/// <summary>
	/// Determines if player running in stereo mode
	/// </summary>
	public static bool IsStereoMode
	{
		get
		{
			return ( VRSettings.enabled && 
			         VRSettings.loadedDevice == VRDeviceType.Stereo );
		}
	}

    /// <summary>
    /// Standard Unity start method.  Initialize the display system from definition
    /// file
    /// </summary>
	void Start()
    {
        /// Load display settings from definition file
		DisplaySettings settings = LoadDisplayDefinition();

        /// Initialize DIRE display system.
        InitializeDisplay(settings);
	}

    /// <summary>
    /// Load display definition file and return as DisplaySettings.
    /// The display name is specified on the command line with the -display DisplayName
    /// argument.  If no display is specified, the system searches for "DefaultDisplay.xml"
    /// The display search paths are TBD
    /// </summary>
    /// <returns>Contents of display definition as DisplaySettings</returns>
    /// 
	private DisplaySettings LoadDisplayDefinition()
	{
		/// default display name
		string displayToLoad = "Default" + DIRE.DisplayExtension;

		/// Look for display file specification.  Reset the display name for loading if one is
        /// given.
		if ( DIRE.DisplayOption != null )
            displayToLoad = DIRE.DisplayOption;

        /// Initialize return value to null
		DisplaySettings settings = null;

        /// Catch errors
		try
		{
            /// Use FileSearch to display path list for the requested display file.
			MessageHandler.Message("Debug", "Searching for " + displayToLoad);

			List<string> displayPaths = DIRE.SetupSearch.FindFile(displayToLoad);

            /// Couldn't find the file, throw an exception
			if ( displayPaths.Count <= 0 )
				throw new Exception("Cannot find: " + displayToLoad );

            /// load the file on the last path found.  (Highest order of precedence.)
			string loadPath = displayPaths.Last();

			MessageHandler.Message("Debug", "Loading: " + loadPath);
			settings = XmlIO.Load( loadPath, typeof(DisplaySettings) ) as DisplaySettings;
		}
		catch ( Exception ex )
		{
            /// Print debug if an exception is thrown.
			MessageHandler.Message("Debug", ex.Message, null, Color.red, null);
			MessageHandler.Message("Debug", "Unable to load display: " + displayToLoad);
			MessageHandler.Message("Debug", "Using internal defaults");
		}

		return( settings );
	}

	/// <summary>
	/// Create eye object.
	/// </summary>
	/// <param name="offset">Offset distance for eye.  This should be in the same units as camera and wall parameters.</param>
	/// <param name="name">Name to give the eye object.</param>
	/// <returns></returns>
	GameObject CreateEyeObject(Transform parent, Vector3 offset, string name)
	{
		GameObject eyeObject;
		
		// Create and return a new GameObject representing an eye.  Attach to this
		// transform and set its TRS appropriately.
		
		eyeObject = new GameObject(name); ;
		eyeObject.transform.parent = parent;
		eyeObject.transform.localPosition = offset;
		eyeObject.transform.localRotation = Quaternion.identity;
		eyeObject.transform.localScale = Vector3.one;
		
		return (eyeObject);
	}

    /// <summary>
    /// Initialize the display setup base on the contents of DisplaySettings.
    /// </summary>
    /// <param name="settings">Display settings defining each wall.</param>
    private void InitializeDisplay(DisplaySettings settings)
    {
        /// Abort if null settings were passed in.  Just use the cameras defined in the project.
		if ( settings == null || settings.screens.Count <= 0 )
		{
			//Debug.LogWarning( "No screens defined, reverting to default display mode" );
			MessageHandler.Message("Debug", "No screens defined, reverting to default display mode");
			return;
		}

        /// Get a list of cameras that are in the scene by default.  These will be destroyed and 
        /// replaced by cameras defined in the display file.
		Camera[] camsToDestroy = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];

        /// Set screen position and size
		MessageHandler.Message("Debug", "Screens: " + settings.screens.Count);
		MessageHandler.Message("Debug", "Setting Window Position: " + 
		                    "(" + settings.X + ", " + settings.Y + ") " + 
		                    settings.Width + "x" + settings.Height);
		SetPosition( settings.X, settings.Y, settings.Width, settings.Height );
	
        /// Find the GameObject that will act as the local origin for Walls
        /// This assumes a fixed position display (projection screen, monitor, etc.) in which the 
        /// users head position will change with regard to the walls.  For something like a HMD,
        /// the head itself would be the display origin.
		GameObject wallsParent = DIRE.Instance.DisplayOrigin;
 		MessageHandler.Message("Debug", "Wall Parent: " + wallsParent);

		/// Create and initialize game objects to represent each eye.
		GameObject leftEyeObj = null;
		GameObject rightEyeObj = null;

		if ( IsStereoMode )
		{
			Transform eyeParent = DIRE.Instance.Head.transform;
			float eyeOffset = baseCam.stereoSeparation / 2.0f;

			leftEyeObj = CreateEyeObject( eyeParent, new Vector3( -eyeOffset, 0, 0 ), "Left Eye" );
			rightEyeObj = CreateEyeObject( eyeParent, new Vector3( eyeOffset, 0, 0 ), "Right Eye" );
		}
		else
		{
			leftEyeObj = DIRE.Instance.Head;
			rightEyeObj = DIRE.Instance.Head;
		}

		// Instantiate a camera for every screen from the display settings file.  
		for( int i=0; i<settings.screens.Count; i++ )
        {
            ScreenInfo sInfo = settings.screens[i];

            /// Create a wall object with wall component for each screen.  Place wall on wall parent.
            GameObject wallObject = new GameObject();
            wallObject.name = sInfo.Name + " Wall";
			wallObject.transform.parent = wallsParent.transform;
            wallObject.transform.localPosition = sInfo.Location.Position;
            wallObject.transform.localEulerAngles = sInfo.Location.Orientation;

            wallObject.AddComponent(typeof(WallDefinition));
            WallDefinition wallDef = wallObject.GetComponent(typeof(WallDefinition)) as WallDefinition;
			wallDef.Width = sInfo.Location.Width;
            wallDef.Height = sInfo.Location.Height;

            /// Create a camera for each wall.  Place the camera on the users head.
			Camera camera = Instantiate(baseCam) as Camera;
            camera.name = sInfo.Name + " Camera";
            camera.transform.parent = DIRE.Instance.Head.transform;
			camera.transform.localPosition = Vector3.zero;
			camera.transform.localRotation = Quaternion.identity;
			camera.transform.localScale = Vector3.one;

            /// set camera display area in window
			camera.rect = new Rect( (float) (sInfo.Viewport.X - settings.X ) / settings.Width,
			                        (float) (settings.Y + settings.Height - sInfo.Viewport.Y - sInfo.Viewport.Height ) / settings.Height,
			                        (float) (sInfo.Viewport.Width) / settings.Width,
			                        (float) (sInfo.Viewport.Height) / settings.Height);

            /// Create a FixedDisplay component if none exists
			if ( camera.GetComponent( typeof( FixedDisplay )) == null )
				camera.gameObject.AddComponent( typeof( FixedDisplay ) );

            /// Associate the wall object the FixedDisplay component on the camera
			FixedDisplay fdScript = camera.GetComponent(typeof(FixedDisplay)) as FixedDisplay;
			fdScript.leftEyeObj = (sInfo.SwapEyes ? rightEyeObj : leftEyeObj );
			fdScript.rightEyeObj = (sInfo.SwapEyes ? leftEyeObj : rightEyeObj );
            fdScript.wallObject = wallObject;
		}

		// This foreach destroys all of the non-DIRE cameras in the scene. 
        // This is performed after the new cameras are set up to prevent a blank screen while the new cameras are set up.
		foreach(Camera obj in camsToDestroy)
		{
			if (obj.gameObject.tag != "DIRE")
				Destroy(obj.gameObject);
		}
	}
}