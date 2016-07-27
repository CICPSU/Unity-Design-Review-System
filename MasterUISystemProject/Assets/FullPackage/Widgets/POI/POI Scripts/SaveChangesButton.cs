using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SaveChangesButton : MonoBehaviour {

	private GameObject activeButton = null;

	public void SaveChanges()
	{
		activeButton = POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager> ().activeButton;
		POI newPoint = new POI ();

		newPoint.sceneFlag = Application.loadedLevelName;
		newPoint.buttonName = POI_ReferenceHub.Instance.poiInfoFields [0].text;
		newPoint.position = new Vector3(float.Parse (POI_ReferenceHub.Instance.poiInfoFields[1].text),float.Parse(POI_ReferenceHub.Instance.poiInfoFields[2].text),float.Parse(POI_ReferenceHub.Instance.poiInfoFields[3].text));
		newPoint.rotation = new Vector3(0,float.Parse(POI_ReferenceHub.Instance.poiInfoFields[4].text),0);

        //update the Point in POIHandler 
        //activeButton.GetComponent<POIInfoRef>().poiInfo.Point.UpdateByValue(newPoint);
        activeButton.GetComponent<POIInfoRef>().poiInfo.Point = newPoint;
		activeButton.GetComponentInChildren<Text> ().text = newPoint.buttonName;

        POIMenuStateManager.CloseBookmarkEditWindow();


    }

}
