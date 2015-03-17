using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CancelChangesButton : MonoBehaviour {

	public void CancelClicked()
	{
		/*
		GameObject activeButton = POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager> ().activeButton;
		POI activePoint = activeButton.GetComponent<POIInfoRef> ().poiInfo.Point;
		POI_ReferenceHub.Instance.poiInfoFields[0].value = activePoint.position.x.ToString();
		POI_ReferenceHub.Instance.poiInfoFields[1].value = activePoint.position.y.ToString();
		POI_ReferenceHub.Instance.poiInfoFields[2].value = activePoint.position.z.ToString();
		POI_ReferenceHub.Instance.poiInfoFields[3].value = activePoint.rotation.y.ToString();
		POI_ReferenceHub.Instance.poiInfoFields[4].value = activePoint.buttonName;
		*/
		POI_ReferenceHub.Instance.poiInfoFields[0].value = "0";
		POI_ReferenceHub.Instance.poiInfoFields[1].value = "0";
		POI_ReferenceHub.Instance.poiInfoFields[2].value = "0";
		POI_ReferenceHub.Instance.poiInfoFields[3].value = "0";
		POI_ReferenceHub.Instance.poiInfoFields[4].value = "Name";

		POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive (false);
	}
}
