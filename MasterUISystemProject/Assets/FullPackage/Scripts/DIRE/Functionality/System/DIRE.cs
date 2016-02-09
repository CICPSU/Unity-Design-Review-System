using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


/// <summary>
/// DIRE system class.  This class contains interfaces and settings reflective of the entire DIRE system.
/// </summary>
public class DIRE : MonoBehaviour 
{
    /// <summary>
    /// A reference to the Camera prefab that will be used to instantiate a per display surface camera.
    /// </summary>
    public Camera baseCam;

    /// <summary>
    /// Reference to the camera that was in the scene originally.  This camera is used when a display file isn't loaded.
    /// </summary>
    public GameObject defaultCam;

    /// <summary>
    /// The GameObject that represents the origin of the tracking system
    /// </summary>
    public GameObject DisplayOrigin = null;

	/// <summary>
	/// The GameObject that represents the root of the MiniMap in the heirarchy.
	/// </summary>
	//public GameObject Minimap = null;

    /// <summary>
    /// The GameObject that represents the head of virtual player.  It may have a tracking system 
    /// attached to it.
    /// </summary>
    public GameObject Head = null;

    /// <summary>
    /// The GameObject that represents the hand (right) of the virtual player.  It may have a tracking
    /// system attached to it.
    /// </summary>
    public GameObject Hand = null;

	public Vector3 displayGeometricCenter;
	public bool trackingActive = false;

    /// <summary>
    /// This is the system wide instance of DIRE.  Scripts should access this for DIRE values.
    /// This reference is initialized by the first DIRE component on the DIRE GameObject.
    /// </summary>
	public static DIRE Instance { get; set; }

    /// <summary>
    /// Name of the window of the DIRE application.  
    /// NOTE: THIS IS NOT ALWAYS ACCURATE AND WE NEED A BETTER WAY TO FIND THE WINDOW NAME.
    /// </summary>
	public static string WindowName 
    {
        get 
        { 
            return "DIRE"; 
        }
    }
	
    public static string DisplayOption
    {
        get
        {
            string retVal = null;

            if ( ArgumentProcessor.CmdLine.ContainsKey("display") &&
                 ArgumentProcessor.CmdLine["display"].Count > 0 )
                retVal = ArgumentProcessor.CmdLine["display"][0] + 
                         DIRE.DisplayExtension;
            
            return( retVal );
        }
    }

    public static List<string> ContentOption
    {
        get
        {
            if (ArgumentProcessor.CmdLine.ContainsKey("content"))
                return (ArgumentProcessor.CmdLine["content"]);
            else
                return (new List<string>());
        }
    }

    public static FileSearch SetupSearch;
    public static String DisplayExtension = ".display";
    public static String InputExtension = ".input";

    public Canvas[] worldCanvasArray;

    /// <summary>
    /// Unity awake function.  Initialize values for the DIRE system,
    /// </summary>
	void Awake()
	{
        /// See if the DIRE instance has been set.  Only one may be registered.
		if (DIRE.Instance != null)
            Debug.Log("Multiple instances of DIRE script");
        else
        {
            /// register the DIRE instance.  Set so that it will remain when scenes are loaded.
			DIRE.Instance = this;
            //DontDestroyOnLoad(this);

            string programDir = "/DIRE";
            string settingsDir = "/Settings";

            List<string> searchPaths = new List<string>();

            //searchPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + programDir + settingsDir);
            //	searchPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + programDir + settingsDir);
            //	searchPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + programDir + settingsDir);
            //	searchPaths.Add(Directory.GetCurrentDirectory() + "/.." + settingsDir);
            searchPaths.Add(Application.dataPath + "/.." + settingsDir);
            searchPaths.Add(Application.dataPath + "/FullPackage/Settings");
            searchPaths.Add(@"C:\DIRE\");
            SetupSearch = new FileSearch();
            foreach (string path in searchPaths)
                SetupSearch.Add(Path.GetFullPath(path));

            Debug.Log("Unified Search Path: " + Environment.NewLine + SetupSearch);

            /// Load display settings from definition file
            DisplaySettings settings = LoadDisplayDefinition();

            /// Initialize DIRE display system.
            InitializeDisplay(settings);
            if (!(settings == null || settings.screens.Count <= 0))
            {
                calculateGeometricCenter(settings);
                if (defaultCam != null)
                    defaultCam.SetActive(false);
            }

            //InitializeInputs();
            GetComponent<ARTtrack>().InitializeTracking();
            GetComponent<ARTtrack>().SetTracking(GetComponent<ARTtrack>().CheckTracking());
        }
	}
    /*
	void InitializeInputs()
	{
		// Get list of input files.  Only keep one of each file name.
		IEnumerable<string> inputFiles = (from f in DIRE.SetupSearch.FindFile("*" + DIRE.InputExtension)
		                                  select Path.GetFileName(f)).Distinct();
		
		// try to load each file.
		foreach (string file in inputFiles)
		{
			try
			{
				// Get the path to the file to be loaded.  The last in the 
				// list has the most precedence.  Load through InputManager
				InputManager.LoadMap ( DIRE.SetupSearch.FindFile(file).Last() );
			}
			catch (Exception ex)
			{
				// something went wrong.  Log the error.
				Debug.LogWarning("Error loading: " + file + Environment.NewLine + ex);
			}
		}
	}

*/

    private Camera FindMouseCamera()
    {
        List<Camera> camList = (from cam in GameObject.FindObjectsOfType<Camera>() where cam.targetTexture == null select cam).ToList();
        foreach (Camera cam in camList)
        {
            if (Input.mousePosition.x > cam.pixelRect.xMin && Input.mousePosition.x < cam.pixelRect.xMax
                && Input.mousePosition.y > cam.pixelRect.yMin && Input.mousePosition.y < cam.pixelRect.yMax)
            {
                return cam;
            }
        }
        return null;
    }

    /// <summary>
    /// Unity per frame update method.  Exit application if Escape is pressed.
    /// </summary>
    void Update()
    {
        /*
        Camera mouseCam = FindMouseCamera();
        foreach(Canvas worldCanvas in worldCanvasArray)
        {
            worldCanvas.worldCamera = mouseCam;
        }
        */
        // Exit if escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
            System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    /// <summary>
    /// Load display definition file and return as DisplaySettings.
    /// The display name is specified on the command line with the -display DisplayName
    /// argument.  If no display is specified, the system searches for "DefaultDisplay.xml"
    /// The display search paths are TBD
    /// </summary>
    /// <returns>Contents of display definition as DisplaySettings</returns>
    /// 
    public DisplaySettings LoadDisplayDefinition()
    {
        /// default display name
        string displayToLoad = "Default" + DIRE.DisplayExtension;

        /// Look for display file specification.  Reset the display name for loading if one is
        /// given.
        if (DIRE.DisplayOption != null)
            displayToLoad = DIRE.DisplayOption;

        /// Initialize return value to null
		DisplaySettings settings = null;

        /// Catch errors
		try
        {
            /// Use FileSearch to display path list for the requested display file.
			Debug.Log("Searching for " + displayToLoad);

            List<string> displayPaths = DIRE.SetupSearch.FindFile(displayToLoad);

            /// Couldn't find the file, throw an exception
			if (displayPaths.Count <= 0)
                throw new Exception("Cannot find: " + displayToLoad);

            /// load the file on the last path found.  (Highest order of precedence.)
			string loadPath = displayPaths.Last();

            Debug.Log("Loading: " + loadPath);
            settings = XmlIO.Load(loadPath, typeof(DisplaySettings)) as DisplaySettings;
        }
        catch (Exception ex)
        {
            /// Print debug if an exception is thrown.
			Debug.Log(ex.Message);
            Debug.Log("Unable to load display: " + displayToLoad);
            Debug.Log("Using internal defaults");
        }

        return (settings);
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
    public void InitializeDisplay(DisplaySettings settings)
    {
        /// Abort if null settings were passed in.  Just use the cameras defined in the project.
		if (settings == null || settings.screens.Count <= 0)
        {
            Debug.LogWarning( "No screens defined, reverting to default display mode" );
            return;
        }

        /// Get a list of cameras that are in the scene by default.  These will be destroyed and 
        /// replaced by cameras defined in the display file.
		Camera[] camsToDestroy = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];

        /// Find the GameObject that will act as the local origin for Walls
        /// This assumes a fixed position display (projection screen, monitor, etc.) in which the 
        /// users head position will change with regard to the walls.  For something like a HMD,
        /// the head itself would be the display origin.
        GameObject wallsParent = DIRE.Instance.DisplayOrigin;
        Debug.Log("Wall parent: " + wallsParent);

        /// Create and initialize game objects to represent each eye.
        GameObject leftEyeObj = null;
        GameObject rightEyeObj = null;

        leftEyeObj = DIRE.Instance.Head;
        rightEyeObj = DIRE.Instance.Head;

        // Instantiate a camera for every screen from the display settings file.  
        for (int i = 0; i < settings.screens.Count; i++)
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
			camera.rect = new Rect((float)(sInfo.Viewport.X - settings.X) / settings.Width,
                                    (float)(settings.Y + settings.Height - sInfo.Viewport.Y - sInfo.Viewport.Height) / settings.Height,
                                    (float)(sInfo.Viewport.Width) / settings.Width,
                                    (float)(sInfo.Viewport.Height) / settings.Height);

            /// Create a FixedDisplay component if none exists
			if (camera.GetComponent(typeof(FixedDisplay)) == null)
                camera.gameObject.AddComponent(typeof(FixedDisplay));

            /// Associate the wall object the FixedDisplay component on the camera
			FixedDisplay fdScript = camera.GetComponent(typeof(FixedDisplay)) as FixedDisplay;
            //fdScript.leftEyeObj = (sInfo.SwapEyes ? rightEyeObj : leftEyeObj );
            //fdScript.rightEyeObj = (sInfo.SwapEyes ? leftEyeObj : rightEyeObj );
            fdScript.WallObject = wallObject;
        }
    }


    //!!!!!!!!!!!!!!!STUFF FROM DISPLAY SYSTEM HANDLER
    public int fieldOfView_oneScreen = 60;

    //called by DIRE, returns the geometric center of the display relative to display origin
    //provides generic methods to calculate the geometric center(the point that is equal distance to all screens) of a display system.
    public Vector3 calculateGeometricCenter(DisplaySettings settings)
    {
        List<Vector3> listOfIntersections = new List<Vector3>();
        List<Vector3> listOfScreenCenter = new List<Vector3>();

        //if there is only one screen, set the geometry center to a location such that the field of view is symetric and equals to fieldOfView_oneScreen
        if (settings.screens.Count == 1)
        {
            //vector(0,0, -1) is the normal of the screen before rotation. Quarternion * vector rotate the vector by the quaternion
            //screenCenterOne(center point of screen 1) and normal vector defines a line starting from the center point. 
            Vector3 normalVectorScreenOne = Quaternion.Euler(settings.screens[0].Location.Orientation) * new Vector3(0, 0, -1);
            Vector3 screenCenterOne = Quaternion.Euler(settings.screens[0].Location.Orientation) * (new Vector3(settings.screens[0].Location.Width * 0.5f, settings.screens[0].Location.Height * 0.5f, 0)) + settings.screens[0].Location.Position;
            float distanceFromScreenCenter = (settings.screens[0].Location.Width / 2) / Mathf.Tan(fieldOfView_oneScreen / 2);

            DIRE.Instance.displayGeometricCenter = normalVectorScreenOne * distanceFromScreenCenter + screenCenterOne;
            return normalVectorScreenOne * distanceFromScreenCenter + screenCenterOne;
        }

        //find all the intersections points of all screen pairs.
        for (int i = 0; i < settings.screens.Count; i++)
        {
            for (int j = i + 1; j < settings.screens.Count; j++)
            {
                //vector(0,0, -1) is the normal of the screen before rotation. Quarternion * vector rotate the vector by the quaternion
                //screenCenterOne(center point of screen 1) and normal vector defines a line starting from the center point. 
                Vector3 normalVectorScreenOne = Quaternion.Euler(settings.screens[i].Location.Orientation) * new Vector3(0, 0, -1);
                Vector3 screenCenterOne = Quaternion.Euler(settings.screens[i].Location.Orientation) * (new Vector3(settings.screens[i].Location.Width * 0.5f, settings.screens[i].Location.Height * 0.5f, 0)) + settings.screens[i].Location.Position;

                listOfScreenCenter.Add(screenCenterOne);

                Vector3 normalVectorScreenTwo = Quaternion.Euler(settings.screens[j].Location.Orientation) * new Vector3(0, 0, -1);
                Vector3 screenCenterTwo = Quaternion.Euler(settings.screens[j].Location.Orientation) * (new Vector3(settings.screens[j].Location.Width * 0.5f, settings.screens[j].Location.Height * 0.5f, 0)) + settings.screens[j].Location.Position;

                Vector3 intersectionPoint = calculateIntersectionOfTwoLines(normalVectorScreenOne, normalVectorScreenTwo, screenCenterOne, screenCenterTwo);

                //vector3(10000,0,0) means no intersection
                if (intersectionPoint != new Vector3(10000, 0, 0))
                {
                    listOfIntersections.Add(intersectionPoint);
                }
            }
        }

        //calculate the average of all the intersection of screen normals
        Vector3 averageOfIntersection = Vector3.zero;
        if (listOfIntersections.Count != 0)
        {
            foreach (Vector3 item in listOfIntersections)
            {
                averageOfIntersection += item;
            }
            averageOfIntersection /= listOfIntersections.Count;
        }
        else
        {
            //****!!!!!! WE ASSUME WHEN THE NORMALS OF MULTIPLE SCREENS DONT INTERSECT, THE SCREENS ARE PARALLEL AND FORM ONE BIG WALL
            //*****!!!IF YOUR SYSTEM HAS NO PARALLEL SCREENS AND THOSE SCREENS DON'T HAVE INTERSECTIONS, THIS WON'T WORK
            //calculates physical distance(in meter) corresponding to a pixel
            float metersByPixel = settings.screens[0].Location.Width / settings.screens[0].Viewport.Width;
            float totalScreenWidth = metersByPixel * settings.Width;
            Vector3 parallelScreenNormal = Quaternion.Euler(settings.screens[0].Location.Orientation) * new Vector3(0, 0, -1);

            Vector3 averageOfScreenCenters = Vector3.zero;
            foreach (Vector3 item in listOfScreenCenter)
            {
                averageOfScreenCenters += item;
            }
            averageOfScreenCenters /= listOfScreenCenter.Count;
            averageOfIntersection = averageOfScreenCenters + parallelScreenNormal * (totalScreenWidth / 2) / Mathf.Tan(fieldOfView_oneScreen / 2);
        }

        DIRE.Instance.displayGeometricCenter = averageOfIntersection;

        //return the average of the screen intersections
        return averageOfIntersection;
    }

    //returns the point of interesection of two lines, or (10000, 0, 0) if they don't intersect
    //positionOne and positionTwo are the center points of the screens
    Vector3 calculateIntersectionOfTwoLines(Vector3 normalOne, Vector3 normalTwo, Vector3 positionOne, Vector3 positionTwo)
    {
        //source code: http://stackoverflow.com/questions/2316490/the-algorithm-to-find-the-point-of-intersection-of-two-3d-line-segment
        Vector3 dC = positionTwo - positionOne;

        //if two normal vectors overlaps
        if (Vector3.Cross(normalOne, normalTwo) == Vector3.zero)
        {
            return (positionOne + (dC.magnitude / 2) * normalOne);
        }

        if (Vector3.Dot(dC, Vector3.Cross(normalOne, normalTwo)) == 0.0)
        {
            Vector3 abCross = Vector3.Cross(normalOne, normalTwo);
            float s = Vector3.Dot(Vector3.Cross(dC, normalTwo), Vector3.Cross(normalOne, normalTwo)) / (Mathf.Pow(abCross.x, 2) + Mathf.Pow(abCross.y, 2) + Mathf.Pow(abCross.z, 2));
            Vector3 intersectionPoint = positionOne + normalOne * s;
            return (intersectionPoint);
        }
        return (new Vector3(10000, 0, 0));
    }

    public void offsetDisplayOriginByGeometricCenter()
    {
        DIRE.Instance.DisplayOrigin.transform.localPosition = -DIRE.Instance.displayGeometricCenter;
    }

    public void offsetHeadToGeometricCenter()
    {
        DIRE.Instance.Head.transform.localPosition = DIRE.Instance.displayGeometricCenter;
    }
}
