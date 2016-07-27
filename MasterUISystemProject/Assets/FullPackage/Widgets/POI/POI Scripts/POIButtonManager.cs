using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;

public class POIButtonManager : MonoBehaviour {

	public static POIButtonManager instance{get; set;}
    // variable to hold the initial button and poi data when the application starts
    // will have an option in the poi menu to restore the original values
    public static POIHandler originalHandler = new POIHandler();
    public RectTransform POIList;
    public GameObject buttonPrefab;
    public int NumOfButtons = 0;
	public GameObject markerRoot; //empty obj, root of all markers. assigned through inspector

    public GameObject markerMouseDown = null;

	// Use this for initialization
	void Start () {

		if(instance ==null){
			instance = this;
		}else{
			Debug.LogError("More than one instance of POIButtonManager!");
		}

		if(markerRoot == null || buttonPrefab == null || POIList == null){
			Debug.LogError("reference broke! check POIButtonManager on the point of interest tool object");
		}

		Debug.Log("loading POIs from: " + POI_GlobalVariables.XMLpath);

		// if we are running in the editor, and we find a poi xml file, we want to merge that file with the poi's that were created in the editor
		// we then generate buttons for all of the pois that were in either the xml file or in the editor
		// if we don't find an xml file, we save off the buttons that were in the editor/built into the project
		if (File.Exists (POI_GlobalVariables.XMLpath)) 
		{
			if(Application.isEditor)
			{
				mergeEditorButsIntoXML(grabButtonsFromEditor(), POI_GlobalVariables.XMLpath);
				GenerateButsMarkers(originalHandler);
			}
			else
			{
				LoadAndGenerateButs();
			}
		} 
		else 
		{
			SaveButsToXML();
		}
	}// start
    
	//whenever a new level is loaded, we regenerate the buttons from the xml file so that only the buttons for the current scene are visible
	public void OnLevelWasLoaded(int level)
	{
		if(File.Exists(POI_GlobalVariables.XMLpath))
		   LoadAndGenerateButs();
		else
		   SaveButsToXML();
	}

	//load the xml from specified path into the handler
	public void loadButsFromXML(string XMLpath, ref POIHandler handler){
		if (File.Exists (XMLpath)) {
			//load the POIHandler.xml, the saved button files
			handler = XmlIO.Load (XMLpath, typeof(POIHandler)) as POIHandler;
		} else {
			Debug.Log ("saved buttons not found! need to generate saved button files based on current project.");
		}
	}

	//generate and setup a button marker pair
	public void GenerateButMarkerPair(POI point){
		GameObject marker;
		marker = generateMarker(point);
		CreateNewButton(point, marker);
	}
	public void ClearButsMarkers()
	{
		//clear current buttons in the menu
		foreach (Transform child in POIList.transform)
			Destroy(child.gameObject);
		
		//clear current markers in marker root
		foreach(Transform child in markerRoot.transform)
			Destroy(child.gameObject);
		
		NumOfButtons = 0;
	}
	
    //this is the combination of function loadButsFromXML and GenerateButsMarkers
	public void LoadAndGenerateButs(){
		if (File.Exists(POI_GlobalVariables.XMLpath))
		{
			ClearButsMarkers();

			//load the POIHandler.xml, the saved button files
			originalHandler = XmlIO.Load(POI_GlobalVariables.XMLpath, typeof(POIHandler)) as POIHandler;
			
			//generate new buttons
			foreach(POI point in originalHandler.projectPOIs){
				GenerateButMarkerPair(point);
				POIList.localPosition = Vector3.zero;
			}
		}
		else{
			Debug.Log("saved buttons not found! need to generate saved button files based on current project.");
		}
	}

	//delete all existing scene buttons and markers and generate from the xml file
	private void GenerateButsMarkers(POIHandler handler){
		ClearButsMarkers ();

		//generate new buttons from the xml file
		foreach(POI point in handler.projectPOIs){
			GenerateButMarkerPair(point);
			POIList.localPosition = Vector3.zero;
		}
	}


	//the only function that generates marker
	//instantiate an instance of marker and return the marker object
	public GameObject generateMarker(POI point){
		//load marker prefab by name
		string path = "POIPanel/" + point.markerModelPrefab;
		Object prefab = Resources.Load(path);
		if(prefab == null){
			Debug.LogError("marker of name " + point.markerModelPrefab + " not found!");
		}

		//configure marker transform, assign point to POIInfo component, assign marker name to button name
		//each marker model is a child of a marker parent. this way we can easily switch the model representation of the marker
		GameObject marker = new GameObject(point.buttonName);//an empty game object whose child is marker model
		marker.AddComponent<POIInfo>();
		marker.transform.position = point.position;
		marker.transform.eulerAngles = point.rotation;
		marker.transform.parent = markerRoot.transform;
		marker.GetComponent<POIInfo>().Point = point;

		GameObject markerModel = Instantiate(prefab, point.position, Quaternion.Euler(point.rotation)) as GameObject;
		markerModel.transform.parent = marker.transform;

		if (point.sceneFlag != SceneManager.GetActiveScene().name)
			markerModel.SetActive (false);

		point.marker = marker;
		return marker;
	}

	// This function is used to generate a new button based on the POI that is passed into the function.
	//args: marker has to be set up first in the scene
	public void CreateNewButton(POI point, GameObject marker)
	{
		// here we instantiate the newButton from a prefab
		// we also get a reference to its RectTransform and set its parent to the POIList
		GameObject newButton = Instantiate(buttonPrefab) as GameObject;
		RectTransform buttonRectTransform = newButton.transform as RectTransform;
		buttonRectTransform.SetParent(POIList);

        // this sets the TooltipTrigger text value to the name of the button
        newButton.GetComponent<BoundTooltipTrigger>().text = point.buttonName;

		newButton.GetComponent<POIInfoRef>().poiInfo = marker.GetComponent<POIInfo>() as POIInfo;

		// update the text of the button to match the name of the POI 
		newButton.transform.GetComponentInChildren<Text>().text = point.buttonName;

		NumOfButtons++;
		
		// code to add a listener to the button OnClicked() event
		EventTrigger eTrigger = newButton.GetComponent<EventTrigger>();
		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		
		// The following line adds the POIClicked function as a listener to the EventTrigger on the button we instantiated.
		trigger.AddListener((eventData)=>POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager>().POIClicked (newButton));
		
		// The next line adds the entry we created to the Event Trigger of the instantiated button.
		// The entry consists of two parts, the listener we set up earlier, and the EventTriggerType.
		// The EventTriggerType tells the EventTrigger when to send out the message that the event has occured.
		// We use PointerClick so we know when the used has clicked on a button.
		EventTrigger.Entry entry = new EventTrigger.Entry(){callback = trigger, eventID = EventTriggerType.PointerClick};
		eTrigger.triggers.Add(entry);
	}

	//remove specified button by reference
	//and shift remaining buttons to reflect changes.
	public void RemoveButton(GameObject buttonToRemove)
	{
		// this section of code is to reposition the buttons below the one being deleted so that there are no gaps in the list
		// !!!! when we switch to using the Layout system, this will be unnecessary
		float yThreshhold = buttonToRemove.transform.position.y;
		for(int i = 0; i < NumOfButtons; i++)
		{
			if(buttonToRemove.transform.parent.GetChild(i).position.y < yThreshhold)
			{
				buttonToRemove.transform.parent.GetChild(i).position += new Vector3(0,37,0);
			}
		}
        
		// The destroy call is what actually removes the button
		DestroyImmediate(buttonToRemove);

		// numofbuttons is used for positioning and sizing the list and buttons
		// !!!! it will be unnecessary in the Layout system
		NumOfButtons--;
	}

	//save the buttons in the scene into the XML file and the orginalHandler
	public void SaveButsToXML()
    {
		Debug.Log("generating saved button files based on current project");
		originalHandler = new POIHandler();
		foreach (Transform child in POIList.transform)
		{
			POI pointToAdd = child.GetComponent<POIInfoRef>().poiInfo.Point;
			originalHandler.AddPoint(pointToAdd);
		}
		
		XmlIO.Save(originalHandler, POI_GlobalVariables.XMLpath);
	}

	//find all buttons in POIList and return them in a List
	private List<POI> grabButtonsFromEditor(){

		List<POI> butsInEditor = new List<POI>();
		foreach(RectTransform but in POIList){ 
			butsInEditor.Add(but.GetComponent<POIInfoRef>().poiInfo.Point);
		}
		return butsInEditor;
	}

	//merge the buttons in editor with the existing XML file
	private void mergeEditorButsIntoXML(List<POI> butsInEditor, string XMLpath){
		loadButsFromXML(XMLpath,ref originalHandler);
		for(int i =0; i < butsInEditor.Count; i++){
			bool match = false;
			foreach(POI point in originalHandler.projectPOIs){
				if(IsPointSame(point,butsInEditor[i])){
					match = true;
					break;
				}
			}
			if(!match){
				originalHandler.AddPoint (butsInEditor[i]);
			}
		}
	}

	//compare two POI classes by value
	//return true if two points are the same
	private bool IsPointSame(POI pointA, POI pointB){
		if(pointA.buttonName != pointB.buttonName){
			return false;
		}

		if(!pointA.sceneFlag.SequenceEqual(pointB.sceneFlag)){
			return false;
		}

		if(pointA.position != pointB.position){
			return false;
		}

		if(pointA.rotation != pointB.rotation){
			return false;
		}

		return true;
	}
	
	public void GeneratePairCurrentLocation()
	{
		POI newPOI = new POI (SceneManager.GetActiveScene().name, POI_ReferenceHub.Instance.BookmarkCurrentLocationNameField.GetComponent<InputField>().text, POI_ReferenceHub.Instance.Avatar.transform.position, POI_ReferenceHub.Instance.Avatar.transform.rotation.eulerAngles, POI_ReferenceHub.Instance.defaultMarkerPrefab.name);
		GenerateButMarkerPair (newPOI);
	}

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
}