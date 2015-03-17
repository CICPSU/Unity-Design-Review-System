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
	public  RectTransform HintText;
	public RectTransform InvalidInputWarning;
	public RectTransform NameExistedWarning;
	public RectTransform SceneFlagList;
	public RectTransform BookmarkCurrentLocationNameField;

	public  List<InputField> poiInfoFields;
	public  UnityEngine.Object defaultMarkerPrefab; //stores the reference to the marker prefab for instantiation

	void Awake(){
		if (POI_ReferenceHub.Instance == null) {
			POI_ReferenceHub.Instance = this;
		} else {
			Debug.LogError("two instance of POI_ReferenceHub!");
		}
		//load the Pin prefab
		defaultMarkerPrefab = Resources.Load ("POIPanel/Pin");
	}
	
}
