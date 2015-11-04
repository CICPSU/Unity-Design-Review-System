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
    /// The GameObject that represents the origin of the display system.
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
    public static String PreferenceExtension = ".pref";

    /// <summary>
    /// Unity awake function.  Initialize values for the DIRE system,
    /// </summary>
	void Awake()
	{
        /// See if the DIRE instance has been set.  Only one may be registered.
		if ( DIRE.Instance != null )
			MessageHandler.Message("Debug", "Multiple instances of DIRE script");
		else
		{
            /// register the DIRE instance.  Set so that it will remain when scenes are loaded.
			DIRE.Instance = this;
			DontDestroyOnLoad(this);

			string programDir = "/DIRE";
			string settingsDir = "/Settings";
			
			List<string> searchPaths = new List<string>();
			
			//searchPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + programDir + settingsDir);
		//	searchPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + programDir + settingsDir);
		//	searchPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + programDir + settingsDir);
		//	searchPaths.Add(Directory.GetCurrentDirectory() + "/.." + settingsDir);
			searchPaths.Add(Application.dataPath + "/.." + settingsDir);
			searchPaths.Add(@"C:\DIRE\");
			SetupSearch = new FileSearch();
			foreach( string path in searchPaths )
				SetupSearch.Add( Path.GetFullPath( path ) );

			MessageHandler.Message("Debug", "Unified Search Path: " + Environment.NewLine + SetupSearch, null);

			/// Load display settings from definition file
			DisplaySettings settings = this.GetComponent<DisplaySetup>().LoadDisplayDefinition();
			
			/// Initialize DIRE display system.
			this.GetComponent<DisplaySetup>().InitializeDisplay(settings);
			if (!( settings == null || settings.screens.Count <= 0 ))
				this.GetComponent<DisplaySetup>().calculateGeometricCenter (settings);
			
			//InitializeInputs();
			trackingActive = GetComponent<ARTtrack>().checkTracking();
			this.GetComponent<ARTtrack>().SetTracking(trackingActive);
            InitializePreferences();
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

    /// <summary>
    /// Load all preference files found
    /// </summary>
    void InitializePreferences()
    {
        // XmlIO.Save(new Preferences(), "C:\\users\\mew9\\Desktop\\Default.pref");

		//XmlIO.Save(new Preferences(), "c:/users/kal5544/desktop/minimapintegration/settings/Default.pref");

        // Get list of preference file
        IEnumerable<string> prefFiles =
            (from f in DIRE.SetupSearch.FindFile("*" + DIRE.PreferenceExtension)
             select f);

        // try to load each file.
        foreach (string path in prefFiles)
            Preferences.Load(path);
    }

    /// <summary>
    /// Unity per frame update method.  Exit application if Escape is pressed.
    /// </summary>
    void Update()
    {
        // Exit if escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
            System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
