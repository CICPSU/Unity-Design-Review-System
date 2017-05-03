using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuickAccessBarManager : MonoBehaviour {

    public GameObject toggleSettingsMenu;
    public GameObject dropCharacterButton;
    public GameObject errorWindow;
    public GameObject widgetRoot;
	public GameObject backgroundBlueBar;
	public GameObject quickAccessBar;
	public GameObject characterDropTool;
	public Text settingButText;
    public Image settingsButtonImage;

    public RectTransform keybindingsPanel;
    public RectTransform avatarPanel;
    public RectTransform widgetPanel;
    
    public TP_Controller tpControlRef;
    public ToggleMenuButtonManager toggleMenuButtonManagerRef;

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
		foreach(Transform child in quickAccessBar.transform){
			child.gameObject.SetActive(false);
		}

        SettingsMenusRefs.Instance.SettingsContentPanel.anchoredPosition = Vector2.zero; //vector2.zero is at the center of the screen
        SettingsMenusRefs.Instance.SettingsContentPanel.gameObject.SetActive(false);
        keybindingsPanel.anchoredPosition = new Vector2(0, -25);
        keybindingsPanel.gameObject.SetActive(false);
        avatarPanel.anchoredPosition = new Vector2(0, -25);
        avatarPanel.gameObject.SetActive(false);
        widgetPanel.anchoredPosition = new Vector2(0, -25);
        widgetPanel.gameObject.SetActive(false);
        SettingsMenusRefs.Instance.SettingsButtonMenu.anchoredPosition = new Vector2(0, -25);
        SettingsMenusRefs.Instance.SettingsButtonMenu.gameObject.SetActive(false);

        characterDropTool.GetComponent<Widget>().Active = false;
        menuButtonsOpen = false;
        tpControlRef.allowPlayerInput = true;
    }

    public void ToggleMenuButtons()
    {
        //check if UIs under the menu is playing animation, if yes, skip
        foreach (Transform child in quickAccessBar.transform) {
            if (iTween.Count(child.gameObject) != 0)
                return;
        }

        //check if widgets are playing 
        foreach (GameObject gO in openWidgets) {
            if (iTween.Count(gO) != 0) {
                return;
            }
        }

        //no animation is playing in bottom menus.
        if (menuButtonsOpen)
            CloseMenu();
        else
            OpenMenu();
             
    }
    
    private void CloseMenu()
    {
        if (ActiveWidgetManager.currentActive == ActiveWidgetManager.ActiveWidget.WidgetConfig)
        {
            WidgetTransitions.Instance.SlideWidgetRoot();

            GetComponent<SettingsManager>().LoadSettingsFiles();

            //this closes all menu buttons along with any other panels that were opened
            GetComponent<ToggleGroup>().SetAllTogglesOff();

            foreach (Transform child in quickAccessBar.transform)
            {
                child.gameObject.SetActive(false);
            }
            errorWindow.SetActive(false);
            menuButtonsOpen = false;
            tpControlRef.allowPlayerInput = true;
            toggleMenuButtonManagerRef.clickedState = false;
            ActiveWidgetManager.DeactivateWidget(ActiveWidgetManager.ActiveWidget.WidgetConfig);
        }
    }

    private void OpenMenu()
    {
        if (ActiveWidgetManager.ActivateWidget(ActiveWidgetManager.ActiveWidget.WidgetConfig))
        {
            WidgetTransitions.Instance.SlideWidgetRoot();
            //all of the menu buttons need to be opened here
            foreach (Transform child in quickAccessBar.transform)
            {
                child.gameObject.SetActive(true);
                iTween.MoveFrom(child.gameObject, iTween.Hash(iT.MoveBy.x, Screen.width, iT.MoveBy.easetype, "easeOutCubic", iT.MoveBy.time, .6)); // time is different to control button arrive time
            }

            menuButtonsOpen = true;
            tpControlRef.allowPlayerInput = false;
            toggleMenuButtonManagerRef.clickedState = true;
            toggleMenuButtonManagerRef.clickTime = Time.time;
        }
    }
    
    public void ToggleSettingsSelectMenu()
    {
        if (!settingsMenusOpen)
        {
            SettingsMenusRefs.Instance.SettingsContentPanel.gameObject.SetActive(true);
            SettingsMenusRefs.Instance.SettingsButtonMenu.gameObject.SetActive(true);
            settingsButtonImage.color = Color.red;
			settingButText.text = "Close";
			settingsMenusOpen = true;
        }
        else
        {
            SettingsMenusRefs.Instance.SettingsContentPanel.gameObject.SetActive(false);
            settingsButtonImage.color = Color.white;
			settingButText.text = "Settings";
			settingsMenusOpen = false;
        }
    }

    public void OpenMenu(RectTransform menuToOpen)
    {
        menuToOpen.gameObject.SetActive(true);
        SettingsMenusRefs.Instance.SettingsButtonMenu.gameObject.SetActive(false);
    }

    public void CloseMenu(RectTransform menuToClose)
    {
        menuToClose.gameObject.SetActive(false);
        SettingsMenusRefs.Instance.SettingsButtonMenu.gameObject.SetActive(true);
    }

}
