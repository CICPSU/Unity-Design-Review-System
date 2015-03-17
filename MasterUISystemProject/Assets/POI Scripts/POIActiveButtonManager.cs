using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class POIActiveButtonManager : MonoBehaviour {

	public Color defaultColor = Color.white;
	public Color activeColor = Color.red;

	public InputField xPosField, yPosField, zPosField, yRotField, nameField;

	public GameObject activeButton = null;

	private GameObject avatar;

	void Start()
	{
		POIMenuStateManager.MenuStateChange += resetActiveButton;
	}

	// This function is called whenver a button is clicked.  
	// The EventTrigger and Listener were set up when the button was instantiated in POIButtonManager.cs.
	public void POIClicked(GameObject clicked)
	{
		if(POIMenuStateManager.EditModeState)
		{
		
			// If there is already an active button, we will need to change its color back to the default before we set the new active button.
			if(activeButton != null)
			{
				activeButton.GetComponent<Image>().color = defaultColor;
			}

			if(activeButton == clicked)
			{
				resetActiveButton();
			}
			else
			{
				activeButton = clicked;
				clicked.GetComponent<Button>().image.color = activeColor;

				Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
				deleteBut.GetComponent<Button>().enabled = true; //enable delete button
				Transform deleteButText = deleteBut.FindChild("Text") as Transform;
				deleteButText.GetComponent<Text>().color = Color.black;

				Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
				editBut.GetComponent<Button>().enabled = true; //enable edit button
				Transform editButText = editBut.FindChild("Text") as Transform;
				editButText.GetComponent<Text>().color = Color.black;
			}
		}
		else
		{
			// teleport to the poi
			avatar = GameObject.FindWithTag("Player");
			//Debug.Log("found gameobject with player tag: " + avatar.name);
			avatar.transform.position = clicked.GetComponent<POIInfoRef>().poiInfo.position;
			avatar.transform.eulerAngles = clicked.GetComponent<POIInfoRef>().poiInfo.rotation;
		}
	}// poiclicked

	public void resetActiveButton()
	{
		if(activeButton != null)
		{
			activeButton.GetComponent<Image>().color = defaultColor;
			activeButton = null;
		}
	}

}
