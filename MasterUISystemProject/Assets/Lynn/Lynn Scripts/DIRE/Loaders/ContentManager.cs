//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.IO;
//
//
///*ContentManager:
// * 
// * This class is responsible for loading the content into DIRE.  Scene files have to be saved into Streamed Scene Asset Bundles (process described in the DIREHowToGuide) to be loaded by DIRE.
// *
// */
//
//public class ContentManager : MonoBehaviour
//{
//
//	// isDownloading:	Bool to track when DIRE is downloading content.  We maintain this state so that we can display information about the download process.
//	// showNoContentMessage:	Bool.  true when the DIREBase scene is loaded.  This only happens when no content parameter was passed on the command line.
//	private bool isDownloading = false;
//	private bool showNoContentMessage = true;
//
//
//	void Awake()
//    {
//		if ( ArgumentProcessor.CmdLine.ContainsKey("content") )
//		{
//			foreach( string content in ArgumentProcessor.CmdLine["content"] )
//			{
//				showNoContentMessage = false;
//				List <string> paths = FileSearch.ContentSearch.FindFile( content + ".unity3d" );
//
//				if ( paths.Count() <= 0 )
//					Debug.Log("Unable to find: " + content );
//				else
//					StartCoroutine( LoadContent( paths.Last() ) );
//			}
//		}
//	}
//
//	/* LoadContent(string path): Function to handle loading content files.
//	 * First we use the WWW class to download the content file.  We can use the file:/// prefix to specify a location on the local machine.
//	 * We set the isDownloading variable to track when the file is done downloading.
//	 * Then we place the asset bundle from the content file into a new variable.
//	 * We call the LoadAll() function.  This laods all of the contents of the asset bundle into memory.
//	 * We call LoadLevel using the name of the asset bundle (which is the same as the name of the scene in the bundle, check the DIREHowToGuide)
//	 */
//	public IEnumerator LoadContent(string path)
//    {
//		if ( !Path.IsPathRooted(path) )
//			path = Path.GetFullPath( Directory.GetCurrentDirectory() + "/" + path );
//
//		// load the SSAB into a www object
//		WWW www = new WWW("file:///" + path);
//
//		isDownloading = true;
//
//		// wait for the www object to finish loading
//		yield return www;
//
//		isDownloading = false;
//
//		// get the assetBundle from the www object
//		AssetBundle bundle = www.assetBundle;
//		
//		bundle.LoadAll ();
//
//		Application.LoadLevel(path.Substring(path.LastIndexOfAny(new char[]{"\\".ToCharArray()[0], "/".ToCharArray()[0]}) + 1, path.Length - path.LastIndexOfAny(new char[]{"\\".ToCharArray()[0], "/".ToCharArray()[0]}) - 9));
//
//	}// LoadContent
//
//	/* OnLevelWasLoaded(int level): Function that is called when a level is loaded.
//	 * We set the position of the character to either the position of the "Origin" object or to a point above the world origin.
//	 * We also reset the velocity and rotation of the character to make sure none of the effects of the previous scene carry over to the newly loaded one.
//	 */
//	void OnLevelWasLoaded(int level)
//    {
//		if(GameObject.Find("Origin") != null)
//        {
//			gameObject.transform.position = GameObject.Find("Origin").transform.position;
//		}// find if
//		else
//        {
//			gameObject.transform.position = new Vector3(0,15,0);
//		}
//
//		gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
//		gameObject.transform.eulerAngles = Vector3.zero;
//	}// OnLevelWasLoaded
//
//	/* Update(): standard Unity function called every frame.
//	 * We use this function to turn off gravity if the application is loading a new level to prevent the user from falling while the level loads.
//	 */
//	void Update()
//    {
//		if (Application.isLoadingLevel)
//        {
//			gameObject.transform.GetComponent<Rigidbody>().useGravity = false;
//		}// loading level
//		else
//        {
//			gameObject.transform.GetComponent<Rigidbody>().useGravity = true;
//		}//
//	}// Update
//
//	/* OnGUI(): standard Unity function called every frame.
//	 * We use this to display messages when the application is loading a level.
//	 */
//	void OnGUI()
//    {
//		if(Application.isLoadingLevel)
//        {
//			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 200),
//                "LOADING LEVEL!  PLEASE WAIT!");
//		}// loading level
//		else if (isDownloading)
//        {
//			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 200), 
//                "DOWNLOADING CONTENT!  PLEASE WAIT!");
//		}// downloading
//		if(showNoContentMessage)
//        {
//			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2 - 300, 200, 200), 
//                "NO CONTENT PARAMETER WAS PASSED ON THE COMMAND LINE.");
//		}// no content message
//	}// onGUI
//}// ContentManager
//
