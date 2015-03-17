using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EditButton : MonoBehaviour {

	public void EditClicked(){

		POIMenuStateManager.EditModeState = true;
		//show the add/delete window
	 	POI_ReferenceHub.Instance.AddDeleteWindow.gameObject.SetActive (true);
		Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
		deleteBut.GetComponent<Button>().enabled = false; //disable delete button
		Transform deleteButText = deleteBut.FindChild("Text") as Transform;
		deleteButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);

		Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
		editBut.GetComponent<Button>().enabled = false; //disable edit button
		Transform editButText = editBut.FindChild("Text") as Transform;
		editButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);

		// show the hint text
		POI_ReferenceHub.Instance.HintText.gameObject.SetActive (true);
		//change the color of the POImenu
		POI_ReferenceHub.Instance.POIMenu.gameObject.GetComponent<Image>().color = Color.black;

		//show the apply and cancel button
		POI_ReferenceHub.Instance.ApplyBut.gameObject.SetActive(true);
		POI_ReferenceHub.Instance.CancelBut.gameObject.SetActive(true);

		//pause the game
		Time.timeScale = 0;
		Time.fixedDeltaTime = 0.02f * Time.timeScale; // fixed update is 50fps, which is 0.02s when time scale is 1

		// set the hint to the first hint
		POI_ReferenceHub.Instance.HintText.GetComponent<HintManager> ().ChangeHint (0);

		//hide the edit button
		gameObject.SetActive(false);
	}
}
