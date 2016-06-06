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

	public List<MonoBehaviour> disableWhileMenuOpen = new List<MonoBehaviour> ();

	private static bool editModeState = false;

	private bool finshedSetupForModeChange = false;

	public delegate void stateChangeDelegate();

	public static event stateChangeDelegate MenuStateChange;

    // we initially add the reset function to the MenuStateChange delegate
    // other functions can be added to the delegate elsewhere
	void Start(){
		MenuStateChange += ChangeState;
	}

    public static void OpenAddDeleteWindow()
    {
        //show the add/delete window
        POI_ReferenceHub.Instance.AddDeleteWindow.gameObject.SetActive(true);
        Transform deleteBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("Delete") as Transform;
        deleteBut.GetComponent<Button>().enabled = false; //disable delete button
        Transform deleteButText = deleteBut.FindChild("Text") as Transform;
        deleteButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);

        Transform editBut = POI_ReferenceHub.Instance.AddDeleteWindow.FindChild("EditBookmark") as Transform;
        editBut.GetComponent<Button>().enabled = false; //disable edit button
        Transform editButText = editBut.FindChild("Text") as Transform;
        editButText.GetComponent<Text>().color = new Color(0.57f, 0.57f, 0.57f);

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

	void Update ()
	{
		if(!finshedSetupForModeChange){
            //this if/else statement changes the time scale while the POIMenu is in Edit mode.
			if (!editModeState){
                ControlUtilities.UnPause();
			}
			else{
                ControlUtilities.Pause();
			}

			//toggle all of the scripts in disableWhileMenuOpen
            //scripts can be added to this list so that they will not be active while the POIMenu is in Edit mode.
			foreach(MonoBehaviour mono in disableWhileMenuOpen)
			{
				mono.enabled = !editModeState;
			}

			finshedSetupForModeChange = true;
		}
	}

    private void ChangeState()
    {
        editModeState = !editModeState;
        // this means we are switching out of Edit mode
        if (!editModeState)
        {
            //restoring the original window
            POI_ReferenceHub.Instance.POIMenu.gameObject.GetComponent<Image>().color = Color.white;

            POI_ReferenceHub.Instance.POIEditWindow.gameObject.SetActive(false);
            POI_ReferenceHub.Instance.AddDeleteWindow.gameObject.SetActive(false);
            POI_ReferenceHub.Instance.CancelBut.gameObject.SetActive(false);
            POI_ReferenceHub.Instance.ApplyBut.gameObject.SetActive(false);
            POI_ReferenceHub.Instance.BookmarkCurrentLocationWindow.gameObject.SetActive(false);

            POI_ReferenceHub.Instance.EditBut.gameObject.SetActive(true);

            ControlUtilities.UnPause();
        }
        // this means we are switching into Edit mode
        else
        {
            OpenAddDeleteWindow();

            //change the color of the POImenu
            POI_ReferenceHub.Instance.POIMenu.gameObject.GetComponent<Image>().color = Color.black;

            //show the apply and cancel button
            POI_ReferenceHub.Instance.ApplyBut.gameObject.SetActive(true);
            POI_ReferenceHub.Instance.CancelBut.gameObject.SetActive(true);

            //pause the game
            ControlUtilities.Pause();

        }
    }
}
