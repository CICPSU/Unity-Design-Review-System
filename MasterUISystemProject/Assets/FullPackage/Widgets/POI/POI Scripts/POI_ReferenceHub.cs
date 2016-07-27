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
	public  RectTransform EditBut;
    public RectTransform ExitBut;
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
	
    

    public void FillPOIInfoFields(POI newPoint)
    {
        Instance.poiInfoFields[0].text = newPoint.buttonName;
        Instance.poiInfoFields[1].text = newPoint.position.x.ToString("F2");
        Instance.poiInfoFields[2].text = newPoint.position.y.ToString("F2");
        Instance.poiInfoFields[3].text = newPoint.position.z.ToString("F2");
        Instance.poiInfoFields[4].text = newPoint.rotation.y.ToString("F2");
    }

}
