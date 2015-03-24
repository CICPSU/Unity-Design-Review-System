using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//attached to the custom bookmark but
public class NewPoint : MonoBehaviour {

	public void clicked(){
		POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(true);
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(true);
		POI_ReferenceHub.Instance.POIEditWindow.FindChild("SaveChanges").gameObject.SetActive(false);

		//grey out delete point button
		Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
		deleteBut.GetComponent<Button>().enabled = false; //disable delete button
		Transform deleteButText = deleteBut.FindChild("Text") as Transform;
		deleteButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);

		//clear the old value from input fields
		foreach(InputField input in POI_ReferenceHub.Instance.poiInfoFields){
			input.transform.FindChild("Text").GetComponent<Text>().text = "";
			input.text = "";
		}
	}
}
