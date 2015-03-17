using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;


//this is the entry class of the DIRE 
public class DIRE : MonoBehaviour 
{

	public static DIRE Instance { get; set; }
	public static string WindowName { get { return "DIRE"; } }

	//public GameObject Controller = null;
	public GameObject DisplayOrigin = null;
	public GameObject Head = null;
	public GameObject Hand = null;
	public DisplaySettings settings;
	public Vector3 displayGeometricCenter;
	public bool trackingActive = false;

	private DisplaySetup displaySetupOnDisplayOrigin;
	private DisplaySystemHandler displaySystemHander;

	void Awake()
	{
		if ( DIRE.Instance != null )
			Debug.LogError("Multiple instances of DIRE script");

		else
		{
			DIRE.Instance = this;
			DontDestroyOnLoad(this);
		}

		//grabbing reference scripts
		displaySetupOnDisplayOrigin = this.GetComponent<DisplaySetup>() as DisplaySetup;
		displaySystemHander = this.GetComponent<DisplaySystemHandler>() as DisplaySystemHandler;

		//reads display configuration into settings
		settings = displaySetupOnDisplayOrigin.LoadDisplayDefinition();

		//check existence of the configuration file. 
		//missing of configuration file results in errors when calculating display geometric center 
		if(settings.screens.Count > 0){
			//align geometric center with "Virtual Cam for TP_camera" gameobject
			displayGeometricCenter = displaySystemHander.calculateGeometricCenter();
			displaySystemHander.offsetDisplayOriginByGeometricCenter();

			//generate walls and eyes based on settings
			displaySetupOnDisplayOrigin.InitializeDisplay(settings);
			
			//****!!!Must be checked before other tracking scripts that reference DIRE.Instance.trackingActive
			//detect if receiving tracking data, if not, set up the cam to regular cam
			trackingActive = GetComponent<ARTtrack>().checkTracking();

			if(!trackingActive){
				Debug.Log("tracking inactive!");
				displaySystemHander.offsetHeadToGeometricCenter();
			}
		}else
		{
			Debug.LogWarning("Warning: no display configuration file!");
		}

	}

}
