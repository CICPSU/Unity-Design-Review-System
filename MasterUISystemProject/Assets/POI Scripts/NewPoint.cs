using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewPoint : MonoBehaviour {

	public void clicked(){
		POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(true);
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(true);
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("SaveChanges").gameObject.SetActive(false);
		POI_ReferenceHub.Instance.HintText.GetComponent<HintManager> ().ChangeHint (1);

		//grey out delete point button
		Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
		deleteBut.GetComponent<Button>().enabled = false; //disable delete button
		Transform deleteButText = deleteBut.FindChild("Text") as Transform;
		deleteButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);
	}
}
