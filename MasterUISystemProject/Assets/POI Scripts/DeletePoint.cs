using UnityEngine;
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
		
	}
}
