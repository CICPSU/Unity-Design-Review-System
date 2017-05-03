using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// This script manages the changes to the POIMenu when going between Edit mode and Normal mode.
/// The MenuStateChange delegate allows otehr scripts to add functions that will be called when the state of the POIMenu changes.
/// </summary>
public class POIMenuStateManager : MonoBehaviour {

	private static bool editModeState = false;

	public delegate void stateChangeDelegate();

	public static event stateChangeDelegate MenuStateChange;

    // we initially add the reset function to the MenuStateChange delegate
    // other functions can be added to the delegate elsewhere
	void Start(){
		MenuStateChange += ChangeState;
	}

    public void ClosePOIWidget()
    {
        ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.Bookmark);
        EditModeState = false;
        SettingsManager.Instance.wc_Settings.bm_Enabled = false;
        SettingsManager.Instance.SaveWidgetControlSettings();
        gameObject.SetActive(false);
    }

    public static void OpenAddDeleteWindow()
    {
        //show the add/delete window
        POI_ReferenceHub.Instance.AddDeleteWindow.gameObject.SetActive(true);
        POI_ReferenceHub.Instance.AddDeleteWindow.anchoredPosition3D = new Vector3(145, 0, 0);

        Vector3[] corners = new Vector3[4];
        POI_ReferenceHub.Instance.AddDeleteWindow.GetWorldCorners(corners);
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!screenRect.Contains(corners[2]))
            POI_ReferenceHub.Instance.AddDeleteWindow.anchoredPosition3D = new Vector3(-220, 0, 0);


        Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
        deleteBut.GetComponent<Button>().enabled = false; //disable delete button
        Transform deleteButText = deleteBut.FindChild("Text") as Transform;
        deleteButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);

        Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
        editBut.GetComponent<Button>().enabled = false; //disable edit button
        Transform editButText = editBut.FindChild("Text") as Transform;
        editButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);

    }

    // buttons in the gui wont reference static functions
    // this is a non-static function to trigger opening the bookmark edit window from a button
    public void B_OpenBookmarkEditWindow()
    {
        OpenBookmarkEditWindow();
    }

    // this function is used to open the bookmark edit window
    // if there is an activeButton, we open the window to edit the selected poi
    // if there is not an activeButton, we open the window to create a custom bookmark
    public static void OpenBookmarkEditWindow()
    {
        if (!POI_ReferenceHub.Instance.POIEditWindow.gameObject.activeSelf)
            POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(true);

        // Here we get the POI from the POIInfo script that is attached to the activeButton.
        // We then use this info to populate the edit menu fields.
        if (POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager>().activeButton != null && POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager>().activeButton.GetComponent<POIInfoRef>().poiInfo.Point != null)
        {
            POI_ReferenceHub.Instance.FillPOIInfoFields(POI_ReferenceHub.Instance.POIMenu.GetComponent<POIActiveButtonManager>().activeButton.GetComponent<POIInfoRef>().poiInfo.Point);
            //disable the Add bookmark button, enable save changes
            POI_ReferenceHub.Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(false);
            POI_ReferenceHub.Instance.POIEditWindow.FindChild("SaveChanges").gameObject.SetActive(true);

            //grey out edit bookmark
            Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
            editBut.GetComponent<Button>().enabled = false; //disable edit button
            Transform editButText = editBut.FindChild("Text") as Transform;
            editButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);
        }
        // if there is no activeButton, we open the edit bookmark window with default values to create a custom bookmark
        else
        {
            POI_ReferenceHub.Instance.FillPOIInfoFields(new POI("", "Name", Vector3.zero, Vector3.zero, ""));

            //enable the Add bookmark button, disable save changes
            POI_ReferenceHub.Instance.POIEditWindow.FindChild("AddBookmark").gameObject.SetActive(true);
            POI_ReferenceHub.Instance.POIEditWindow.FindChild("SaveChanges").gameObject.SetActive(false);

            //grey out delete point button
            Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
            deleteBut.GetComponent<Button>().enabled = false; //disable delete button
            Transform deleteButText = deleteBut.FindChild("Text") as Transform;
            deleteButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);

            //clear the old value from input fields
            foreach (InputField input in POI_ReferenceHub.Instance.poiInfoFields)
            {
                input.transform.FindChild("Text").GetComponent<Text>().text = "";
                input.text = "";
            }
        }


    }

    // buttons in the gui wont reference static functions
    // this is a non-static function to trigger closing the bookmark edit window from a button
    public void B_CloseBookmarkEditWindow()
    {
        CloseBookmarkEditWindow();
    }

    // this function is used to close the bookmark edit window
    public static void CloseBookmarkEditWindow()
    {
        //enable edit bookmark
        Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
        editBut.GetComponent<Button>().enabled = true; //enable edit button
        Transform editButText = editBut.FindChild("Text") as Transform;
        editButText.GetComponent<Text>().color = new Color(50f / 255, 50f / 255, 50f / 255);

        POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);
    }

    // this is the single function that can be called to switch the Edit mode.
    // the edit button and cancel button both call this
    public void ToggleEditModeState()
    {
        EditModeState = !EditModeState;
    }

	public static bool EditModeState{
		get{

			return editModeState; //default of bool is false, no need to initialize

		}
        // the setter for this property triggered the MenuStateChange delegate
		set{
			if(editModeState != value){
				MenuStateChange();
			}

			editModeState = value;
		}
	}

    private void ChangeState()
    {
        if (ActiveWidgetManager.currentActive == ActiveWidgetManager.ActiveWidget.None || ActiveWidgetManager.currentActive == ActiveWidgetManager.ActiveWidget.Bookmark)
        {
            editModeState = !editModeState;
            // this means we are switching out of Edit mode
            if (!editModeState)
            {
                //restoring the original window
                POI_ReferenceHub.Instance.POIMenu.gameObject.GetComponent<Image>().color = Color.white;

                POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);
                POI_ReferenceHub.Instance.AddDeleteWindow.gameObject.SetActive(false);
                POI_ReferenceHub.Instance.BookmarkCurrentLocationWindow.gameObject.SetActive(false);

                POIButtonManager.instance.LoadAndGenerateButs();

                ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.Bookmark);

            }
            // this means we are switching into Edit mode
            else
            {
                if (ActiveWidgetManager.ActivateWidget(ActiveWidgetManager.ActiveWidget.Bookmark))
                {
                    OpenAddDeleteWindow();

                    //change the color of the POImenu
                    POI_ReferenceHub.Instance.POIMenu.gameObject.GetComponent<Image>().color = Color.black;
                }

            }
        }
    }
}
