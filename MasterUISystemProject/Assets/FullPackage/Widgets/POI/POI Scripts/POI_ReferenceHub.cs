using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class POI_ReferenceHub : MonoBehaviour{
	public static POI_ReferenceHub Instance;

	//references are all set through inspector
	public GameObject Avatar;
	public  Transform POIroot;
	public  RectTransform POIMenu;
	public  RectTransform POIEditWindow;
	public  RectTransform AddDeleteWindow;
	public RectTransform BookmarkCurrentLocationWindow;
	public  RectTransform ApplyBut;
	public  RectTransform CancelBut;
	public  RectTransform EditBut;
	public RectTransform InvalidInputWarning;
	public RectTransform NameExistedWarning;
	public RectTransform BookmarkCurrentLocationNameField;
	public GameObject markerInfoCanvas;
    public GameObject poiCanvas;
    public GameObject markerRoot;

	public  List<InputField> poiInfoFields; // the Name, position, and rotation fields in the POIedit window
	public  UnityEngine.Object defaultMarkerPrefab; //stores the reference to the marker prefab for instantiation

	void Awake(){
        if (Avatar == null)
        {
            Debug.Log("POI_ReferenceHub.Avatar was null! Searching for tag: Player");
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                Debug.Log("Found object tagged: Player");
                Avatar = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                Debug.Log("POI_ReferenceHub.Avatar was null and no object tagged: Player");
            }
        }
		if (POI_ReferenceHub.Instance == null) {
			POI_ReferenceHub.Instance = this;
		} else {
			Debug.LogError("two instance of POI_ReferenceHub!");
		}
		//load the Pin prefab
		defaultMarkerPrefab = Resources.Load ("POIPanel/Pin");
	}
	
    public void OpenEditWindow()
    {
        if (!Instance.POIEditWindow.gameObject.activeSelf)
            Instance.POIEditWindow.gameObject.SetActive(true);

        // Here we get the POI from the POIInfo script that is attached to the activeButton.
        // We then use this info to populate the edit menu fields.
        POI clickedPOI = Instance.POIMenu.GetComponent<POIActiveButtonManager>().activeButton.GetComponent<POIInfoRef>().poiInfo.Point;
        Instance.poiInfoFields[0].text = clickedPOI.buttonName;
        Instance.poiInfoFields[1].text = clickedPOI.position.x.ToString();
        Instance.poiInfoFields[2].text = clickedPOI.position.y.ToString();
        Instance.poiInfoFields[3].text = clickedPOI.position.z.ToString();
        Instance.poiInfoFields[4].text = clickedPOI.rotation.y.ToString();

        //disable the Add bookmark button, enable save changes
        Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(false);
        Instance.POIEditWindow.FindChild("SaveChanges").gameObject.SetActive(true);

        //grey out edit bookmark
        Transform editBut = Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
        editBut.GetComponent<Button>().enabled = false; //disable edit button
        Transform editButText = editBut.FindChild("Text") as Transform;
        editButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);
    }

    public void CloseEditWindow()
    {
        //enable edit bookmark
        Transform editBut = Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
        editBut.GetComponent<Button>().enabled = true; //enable edit button
        Transform editButText = editBut.FindChild("Text") as Transform;
        editButText.GetComponent<Text>().color = new Color(50f / 255, 50f / 255, 50f / 255);

        Instance.POIEditWindow.gameObject.SetActive(false);
    }

    public void FillPOIInfoFields(POI newPoint)
    {
        Instance.poiInfoFields[0].text = newPoint.buttonName;
        Instance.poiInfoFields[1].text = newPoint.position.x.ToString();
        Instance.poiInfoFields[2].text = newPoint.position.y.ToString();
        Instance.poiInfoFields[3].text = newPoint.position.z.ToString();
        Instance.poiInfoFields[4].text = newPoint.rotation.y.ToString();
    }

}
