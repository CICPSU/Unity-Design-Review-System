using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class POIActiveButtonManager : MonoBehaviour {

	public Color defaultColor = Color.white;
	public Color activeColor = Color.red;
	
	public GameObject activeButton = null;

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
		
			POI_ReferenceHub.Instance.BookmarkCurrentLocationWindow.gameObject.SetActive(false);
			// If there is already an active button, we will need to change its color back to the default before we set the new active button.
			if(activeButton != null)
			{
				activeButton.GetComponent<Image>().color = defaultColor;
			}

			if(activeButton == clicked)
			{
                // this will de-select the active button
				resetActiveButton();
			}
			else
			{
				activeButton = clicked;
				clicked.GetComponent<Button>().image.color = activeColor;

				Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
				deleteBut.GetComponent<Button>().enabled = true; //enable delete button
				Transform deleteButText = deleteBut.FindChild("Text") as Transform;
				deleteButText.GetComponent<Text>().color = new Color(50f / 255, 50f / 255, 50f / 255, 1);

				if(POI_ReferenceHub.Instance.POIEditWindow.gameObject.activeSelf)
				{
					//grey out edit bookmark
					Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
					editBut.GetComponent<Button>().enabled = false; //disable edit button
					Transform editButText = editBut.FindChild("Text") as Transform;
					editButText.GetComponent<Text>().color = new Color(0.57f,0.57f,0.57f);
				}
				else
				{
					Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
					editBut.GetComponent<Button>().enabled = true; //enable edit button
					Transform editButText = editBut.FindChild("Text") as Transform;
					editButText.GetComponent<Text>().color = new Color(50f/255,50f/255,50f/255,1);
				}

				// we also need to fill in the input fields in the edit window when a button is made active
				POI clickedPOI = activeButton.GetComponent<POIInfoRef>().poiInfo.Point;
                POI_ReferenceHub.Instance.FillPOIInfoFields(clickedPOI);
				POI_ReferenceHub.Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(false);
				POI_ReferenceHub.Instance.POIEditWindow.FindChild("SaveChanges").gameObject.SetActive(true);

			}
		}
		else
		{
			// teleport to the poi
            // if it is in a different scene, load that scene first
			if(clicked.GetComponent<POIInfoRef>().poiInfo.Point.sceneFlag != SceneManager.GetActiveScene().name)
				SceneManager.LoadScene(clicked.GetComponent<POIInfoRef>().poiInfo.Point.sceneFlag);

			POI_ReferenceHub.Instance.Avatar.transform.position = clicked.GetComponent<POIInfoRef>().poiInfo.Point.position;
			POI_ReferenceHub.Instance.Avatar.transform.eulerAngles = clicked.GetComponent<POIInfoRef>().poiInfo.Point.rotation;
		}
	}// poiclicked

    // this resets the active button and disables the Delete and EditBookmark buttons
	public void resetActiveButton()
	{
		if(activeButton != null)
		{
			activeButton.GetComponent<Image>().color = defaultColor;
			activeButton = null;
			POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);

			Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
			deleteBut.GetComponent<Button>().enabled = false; //disable delete button
			Transform deleteButText = deleteBut.FindChild("Text") as Transform;
			deleteButText.GetComponent<Text>().color = new Color(145f/255,145f/255,145f/255,1);
			
			Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
			editBut.GetComponent<Button>().enabled = false; //disable edit button
			Transform editButText = editBut.FindChild("Text") as Transform;
			editButText.GetComponent<Text>().color = new Color(145f/255,145f/255,145f/255,1);
		}
	}

}
