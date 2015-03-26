using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

//this is attached to the Add bookmark button in POIEdit Window
public class AddPointButton : MonoBehaviour {
	POI point = new POI();

	public void onClicked(){
		//validate button name
		if(checkButtonNameExist(ref point)){
			POI_ReferenceHub.Instance.NameExistedWarning.gameObject.SetActive(true);
		}else if(!validateInput()){
			//validate input
			POI_ReferenceHub.Instance.InvalidInputWarning.gameObject.SetActive(true);
		}else{
			SaveChanges(false);
			this.transform.parent.gameObject.SetActive(false);
		}
	}

	//create a new location and save to handler, 
	//overwrite: true is overwrite existing, false will add a new point
	public void SaveChanges(bool overwrite){
		//create new Point object
		string sFlag = Application.loadedLevelName; 
		Vector3 pos = new Vector3 (float.Parse(POI_ReferenceHub.Instance.poiInfoFields [1].text), float.Parse(POI_ReferenceHub.Instance.poiInfoFields [2].text), float.Parse(POI_ReferenceHub.Instance.poiInfoFields [3].text));
		Vector3 rot = new Vector3 (0, float.Parse(POI_ReferenceHub.Instance.poiInfoFields [4].text), 0);
		POI newPoint = new POI (sFlag, POI_ReferenceHub.Instance.poiInfoFields [0].text, pos, rot, POI_GlobalVariables.defaultMarker);
		if(overwrite){
			point.UpdateByValue(newPoint);
			point.marker.transform.position = point.position;
		}else{
			//generate button and marker pair
			POIButtonManager.instance.GenerateButMarkerPair (newPoint);
			//add the point into the orginalHandler
			POIButtonManager.originalHandler.AddPoint(newPoint);
		}
	}

	// param: returns reference of duplicated point
	bool checkButtonNameExist(ref POI duplicatedPoint){
		string butName = POI_ReferenceHub.Instance.poiInfoFields [0].text;
		foreach (POI point in POIButtonManager.originalHandler.projectPOIs){
			if(point.buttonName == butName){
				duplicatedPoint = point;
				return true; //button name existed
			}
		}
		return false; //button name does not exist
	}

	//return false when input not valid
	bool validateInput(){
		for(int i =1; i <= 4; i++){ //traverse the poi input fields, 0-4 are pos and rot input fields
			 InputField field = POI_ReferenceHub.Instance.poiInfoFields[i];
			string value = field.text;
			float result; //dummy var for filling in tryparse below
			if(!float.TryParse(value, out result)){
				return false;
			}
		}

		return true;
	}
}
