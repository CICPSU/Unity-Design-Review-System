using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class AddPointButton : MonoBehaviour {

	public void onClicked(){
		//create new Point object
		List<string> sFlag = POI_ReferenceHub.Instance.SceneFlagList.parent.GetComponent<SceneFlagSelector>().GetSceneFlags(); //!!!! need to be implemented later when sceneflag is set
		if(sFlag ==null || sFlag.Count == 0){
			sFlag.Add(Application.loadedLevelName);
		}
		//validate input
		//validate button name
		if(checkButtonNameExist()){
			POI_ReferenceHub.Instance.NameExistedWarning.gameObject.SetActive(true);
		}
		
		if(!validateInput()){
			POI_ReferenceHub.Instance.InvalidInputWarning.gameObject.SetActive(true);
		}

		Vector3 pos = new Vector3 (float.Parse(POI_ReferenceHub.Instance.poiInfoFields [0].value), float.Parse(POI_ReferenceHub.Instance.poiInfoFields [1].value), float.Parse(POI_ReferenceHub.Instance.poiInfoFields [2].value));
		Vector3 rot = new Vector3 (0, float.Parse(POI_ReferenceHub.Instance.poiInfoFields [3].value), 0);
		POI point = new POI (sFlag, POI_ReferenceHub.Instance.poiInfoFields [4].value, pos, rot, POI_GlobalVariables.defaultMarker);
		//generate button and marker pair
		POIButtonManager.instance.GenerateButMarkerPair (point);
		//add the point into the orginalHandler
		POIButtonManager.originalHandler.AddPoint(point);



	}

	bool checkButtonNameExist(){
		string butName = POI_ReferenceHub.Instance.poiInfoFields [4].value;
		foreach (POI point in POIButtonManager.originalHandler.projectPOIs){
			if(point.buttonName == butName){
				return true; //button name existed
			}
		}
		return false; //button name does not exist
	}

	//return false when input not valid
	bool validateInput(){
		for(int i =0; i < 4; i++){ //traverse the poi input fields, 0-4 are pos and rot input fields
			 InputField field = POI_ReferenceHub.Instance.poiInfoFields[i];
			string value = field.value;
			float result; //dummy var for filling in tryparse below
			if(!float.TryParse(value, out result)){
				return false;
			}
		}

		return true;
	}
}
