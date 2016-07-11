using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SettingsBarManager : MonoBehaviour {

    public GameObject toggleSettingsMenu;
    public GameObject dropCharacterButton;
    public GameObject errorWindow;
    public GameObject widgetRoot;
	public GameObject backgroundBlueBar;
	public GameObject settingBar;
	public GameObject characterDropTool;
    public Image settingsButtonImage;

    
    public TP_Controller tpControlRef;

    public bool menuButtonsOpen = false;
    public bool settingsMenusOpen = false;

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
        // loads settings file when the application starts
        GetComponent<SettingsManager>().LoadSettingsFiles();

        //this closes all menu buttons along with any other panels that were opened
        GetComponent<ToggleGroup>().SetAllTogglesOff();
		foreach(Transform child in settingBar.transform){
			child.gameObject.SetActive(false);
		}

		characterDropTool.GetComponent<Widget>().Active = false;
        menuButtonsOpen = false;
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
        WidgetTransitions.Instance.SlideWidgetRoot();

        GetComponent<SettingsManager>().LoadSettingsFiles();

        //this closes all menu buttons along with any other panels that were opened
        GetComponent<ToggleGroup>().SetAllTogglesOff();

		foreach(Transform child in settingBar.transform){
			child.gameObject.SetActive(false);
		}
        errorWindow.SetActive(false);
        menuButtonsOpen = false;
        tpControlRef.allowPlayerInput = true;
	
    }

    private void OpenMenu()
    {
        WidgetTransitions.Instance.SlideWidgetRoot();
        //all of the menu buttons need to be opened here
		foreach(Transform child in settingBar.transform){
			child.gameObject.SetActive(true);
			iTween.MoveFrom(child.gameObject, iTween.Hash(iT.MoveBy.x, Screen.width, iT.MoveBy.easetype, "easeOutCubic", iT.MoveBy.time, .6)); // time is different to control button arrive time
		}
			
        menuButtonsOpen = true;
        tpControlRef.allowPlayerInput = false;
    }
    
    public void ToggleSettingsSelectMenu()
    {
        if (!settingsMenusOpen)
        {
            SettingsMenusRefs.Instance.SettingsContentPanel.anchoredPosition = Vector2.zero;
            SettingsMenusRefs.Instance.SettingsSelectMenu.anchoredPosition = new Vector2(0, -25);
            settingsButtonImage.color = Color.red;
            settingsMenusOpen = true;
        }
        else
        {
            SettingsMenusRefs.Instance.SettingsContentPanel.anchoredPosition = new Vector2(-1000, Screen.height * 2);
            settingsButtonImage.color = Color.white;
            settingsMenusOpen = false;
        }
    }

    public void OpenMenu(RectTransform menuToOpen)
    {
        menuToOpen.anchoredPosition = new Vector2(0, -25);
        SettingsMenusRefs.Instance.SettingsSelectMenu.anchoredPosition = new Vector2(-1000, Screen.height * 2);
    }

    public void CloseMenu(RectTransform menuToClose)
    {
        menuToClose.anchoredPosition = new Vector2(0, Screen.height * 2);
        SettingsMenusRefs.Instance.SettingsSelectMenu.anchoredPosition = new Vector2(0, -25);
    }

}
