using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeletePoint : MonoBehaviour {

	public void RemovePointFromMenuAndHandler()
	{
		//get the reference to the active but
		GameObject activePOIButton = POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager>().activeButton;
		if(activePOIButton == null){
			Debug.LogError("Active button is null");
		}
		//remove the point from the original handler
		POIButtonManager.originalHandler.RemovePoint(activePOIButton.GetComponent<POIInfoRef>().poiInfo.Point);
		//remove the marker from the marker root
		Destroy( activePOIButton.GetComponent<POIInfoRef>().poiInfo.gameObject);
		//remove the button from the button manager
		POIButtonManager.instance.RemoveButton(activePOIButton);
		//close the edit window
		POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive (false);

		//grey out delete point button
		Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
		deleteBut.GetComponent<Button>().enabled = false; //disable delete button
		Transform deleteButText = deleteBut.FindChild("Text") as Transform;
		deleteButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);
		//grey out edit bookmark
		Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
		editBut.GetComponent<Button>().enabled = false; //disable edit button
		Transform editButText = editBut.FindChild("Text") as Transform;
		editButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);
		
	}
}
