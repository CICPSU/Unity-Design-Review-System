using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//attach to the NameField Button
//called when the Namefield Button is selected and deselected
public class LocationNameChanged : MonoBehaviour {
	private string nameString = "";

	//called by the event when name input field is selected
	public void selected(){
		nameString = gameObject.GetComponentInChildren<Text>().text;
	}

	//called by the event when the name input field is deselected
	public void deselected(){
		if(nameString != gameObject.GetComponentInChildren<Text>().text){ //user has typed in name
			POI_ReferenceHub.Instance.HintText.GetComponent<HintManager>().ChangeHint(2);
		}
	}
}
