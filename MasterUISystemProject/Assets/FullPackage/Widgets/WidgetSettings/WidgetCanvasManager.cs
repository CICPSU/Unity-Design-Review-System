using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WidgetCanvasManager : MonoBehaviour {

    public GameObject toggleSettingsMenu;
    public GameObject dropCharacterButton;
    public GameObject chooseWidgetPanel;
    public GameObject displaySettingsPanel;
    public GameObject errorWindow;
    public GameObject widgetRoot;
	public GameObject backgroundBlueBar;
	public GameObject settingBar;
	public GameObject characterDropTool;
    public float timeScale = 1f;


    public TP_Camera tpCamRef;
    public TP_Controller tpControlRef;

    public bool menuButtonsOpen = false;

    private List<GameObject> openWidgets = new List<GameObject>();


    void Start()
    {
        IntializeUI();
    }

    void Update()
    {
       
    }

    private void IntializeUI()
    {
        widgetRoot.GetComponent<WidgetSettingsManager>().LoadSettingsFiles();
        //this closes all menu buttons along with any other panels that were opened
        GetComponent<ToggleGroup>().SetAllTogglesOff();
		foreach(Transform child in settingBar.transform){
			child.gameObject.SetActive(false);
		}

		characterDropTool.GetComponent<Widget>().Active = false;
        menuButtonsOpen = false;
        //tpCamRef.allowPlayerInput = true;
        tpControlRef.allowPlayerInput = true;
    }

    public void ToggleMenuButtons()
    {
        //check if UIs under the menu is playing animation, if yes, skip
		foreach(Transform child in settingBar.transform){
			if(iTween.Count(child.gameObject) != 0)
				return;
		}

		//check if widgets are playing 
		foreach(GameObject gO in openWidgets){
			if(iTween.Count(gO) !=0){
				return;
			}
		}

		//no animation is playing in bottom menus.
            if (menuButtonsOpen)
                CloseAll();
            else
                OpenMenu();

    }

    private void CloseAll()
    {
        //this re-enables any widgets that were open and loads the settings files to make sure they are up to date
        foreach (GameObject gO in openWidgets) {
            iTween.MoveBy(gO, new Vector3(0, -Screen.height, 0), 0.8f);
        }

        widgetRoot.GetComponent<WidgetSettingsManager>().LoadSettingsFiles();

        //this closes all menu buttons along with any other panels that were opened
        GetComponent<ToggleGroup>().SetAllTogglesOff();

		foreach(Transform child in settingBar.transform){
			child.gameObject.SetActive(false);
		}
        errorWindow.SetActive(false);
        menuButtonsOpen = false;
        //tpCamRef.allowPlayerInput = true;
        tpControlRef.allowPlayerInput = true;
	
    }

    private void OpenMenu()
    {
        //this goes through and slides up all the active widgets
        //the ones that were open are stored in a list so that they can be reopened when the menu is closed
		openWidgets.Clear(); //clear previous section
		for (int i = 0; i < widgetRoot.transform.childCount; i++)
        {
			if (widgetRoot.transform.GetChild(i).gameObject.GetComponent<Widget>().Active && widgetRoot.transform.GetChild(i).gameObject.activeSelf)
            {
                iTween.MoveBy(widgetRoot.transform.GetChild(i).gameObject, new Vector3(0, Screen.height, 0), 0.8f);
                openWidgets.Add(widgetRoot.transform.GetChild(i).gameObject);
            }
        }

        //all of the menu buttons need to be opened here
		foreach(Transform child in settingBar.transform){
			child.gameObject.SetActive(true);
			iTween.MoveFrom(child.gameObject, iTween.Hash(iT.MoveBy.x, Screen.width, iT.MoveBy.easetype, "easeOutCubic", iT.MoveBy.time, .6)); // time is different to control button arrive time
		}
			
        menuButtonsOpen = true;
        //tpCamRef.allowPlayerInput = false;
        tpControlRef.allowPlayerInput = false;
    }
}

public abstract class WidgetMenu : MonoBehaviour
{
    public WidgetMenu()
    {

    }

    public abstract void ToggleMenu();

}
