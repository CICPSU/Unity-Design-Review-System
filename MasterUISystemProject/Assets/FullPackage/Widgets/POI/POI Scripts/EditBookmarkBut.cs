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
		POI_ReferenceHub.Instance.poiInfoFields [0].text = clickedPOI.buttonName;
		POI_ReferenceHub.Instance.poiInfoFields [1].text = clickedPOI.position.x.ToString();
		POI_ReferenceHub.Instance.poiInfoFields [2].text = clickedPOI.position.y.ToString();
		POI_ReferenceHub.Instance.poiInfoFields [3].text = clickedPOI.position.z.ToString();
		POI_ReferenceHub.Instance.poiInfoFields [4].text = clickedPOI.rotation.y.ToString();
		
		//disable the Add bookmark button, enable save changes
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(false);
		POI_ReferenceHub.Instance.POIEditWindow.FindChild ("SaveChanges").gameObject.SetActive (true);

		//grey out edit bookmark
		Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
		editBut.GetComponent<Button>().enabled = false; //disable edit button
		Transform editButText = editBut.FindChild("Text") as Transform;
		editButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);
	}
}
