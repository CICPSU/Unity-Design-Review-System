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
    public float timeScale = 1f;


    public TP_Camera tpCamRef;
    public TP_Controller tpControlRef;

    public bool menuButtonsOpen = false;

    private List<GameObject> openWidgets = new List<GameObject>();

    private float currentTime; //used to ensure the the gear icon is only clicked once within 0.5 second

    void Start()
    {
        currentTime = Time.time;
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

        toggleSettingsMenu.SetActive(false);
        dropCharacterButton.SetActive(false);
        errorWindow.SetActive(false);
        menuButtonsOpen = false;
        //tpCamRef.allowPlayerInput = true;
        tpControlRef.allowPlayerInput = true;
    }

    public void ToggleMenuButtons()
    {
        if (Time.time - currentTime >= 0.8f) { //make sure two clicks are at least 0.8s apart
            if (menuButtonsOpen)
                CloseAll();
            else
                OpenMenu();

            currentTime = Time.time;
        }

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

        toggleSettingsMenu.SetActive(false);
        dropCharacterButton.SetActive(false);
        errorWindow.SetActive(false);
        menuButtonsOpen = false;
        //tpCamRef.allowPlayerInput = true;
        tpControlRef.allowPlayerInput = true;
        
    }

    private void OpenMenu()
    {
        //this goes through and deactivates all the open widgets
        //the ones that were open are stored in a list so that they can be reopened when the menu is closed
        for (int i = 0; i < widgetRoot.transform.childCount; i++)
        {
            if (widgetRoot.transform.GetChild(i).gameObject.activeSelf)
            {
                iTween.MoveBy(widgetRoot.transform.GetChild(i).gameObject, new Vector3(0, Screen.height, 0), 0.8f);
                openWidgets.Add(widgetRoot.transform.GetChild(i).gameObject);
            }
        }

        //all of the menu buttons need to be opened here
        toggleSettingsMenu.SetActive(true);
        dropCharacterButton.SetActive(true);

        iTween.MoveFrom(dropCharacterButton, iTween.Hash(iT.MoveBy.x, Screen.width, iT.MoveBy.easetype, "easeOutCubic", iT.MoveBy.time, .4)); // time is different to control button arrive time
        iTween.MoveFrom(toggleSettingsMenu, iTween.Hash(iT.MoveFrom.x, Screen.width, iT.MoveFrom.easetype, "easeOutCubic", iT.MoveFrom.time, .5));
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
