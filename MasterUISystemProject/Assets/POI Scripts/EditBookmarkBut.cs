using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EditBookmarkBut : MonoBehaviour {


	public void clicked(){

		if(!POI_ReferenceHub.Instance.POIEditWindow.gameObject.activeSelf)
			POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(true);
		
		// Here we get the POI from the POIInfo script that is attached to the activeButton.
		// We then use this info to populate the edit menu fields.
		POI clickedPOI = POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager>().activeButton.GetComponent<POIInfoRef>().poiInfo.Point;
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("XPosField").FindChild("Text").GetComponent<Text>().text = clickedPOI.position.x.ToString();
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("YPosField").FindChild("Text").GetComponent<Text>().text = clickedPOI.position.y.ToString();
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("ZPosField").FindChild("Text").GetComponent<Text>().text = clickedPOI.position.z.ToString();
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("YRotField").FindChild("Text").GetComponent<Text>().text = clickedPOI.rotation.y.ToString();
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("NameField").FindChild("Text").GetComponent<Text>().text = clickedPOI.buttonName;

		//disable the Add bookmark button
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(false);
	}
}
