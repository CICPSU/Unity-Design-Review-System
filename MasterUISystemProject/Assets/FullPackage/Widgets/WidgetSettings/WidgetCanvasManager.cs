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

    public TP_Camera tpCamRef;

    public bool menuButtonsOpen = false;

    private List<GameObject> openWidgets = new List<GameObject>();

    void Start()
    {
        CloseAll();
    }

    void Update()
    {
       
    }

    public void ToggleMenuButtons()
    {
        if (menuButtonsOpen)
            CloseAll();
        else
            OpenMenu();

    }

    private void CloseAll()
    {
        //this re-enables any widgets that were open and loads the settings files to make sure they are up to date
        foreach (GameObject gO in openWidgets)
            gO.SetActive(true);

        widgetRoot.GetComponent<WidgetSettingsManager>().LoadSettingsFiles();

        //this closes all menu buttons along with any other panels that were opened
        GetComponent<ToggleGroup>().SetAllTogglesOff();
        toggleSettingsMenu.SetActive(false);
        dropCharacterButton.SetActive(false);
        errorWindow.SetActive(false);
        menuButtonsOpen = false;
        tpCamRef.allowPlayerInput = true;
        Time.timeScale = 1f;
    }
    
    private void OpenMenu()
    {
        //this goes through and deactivates all the open widgets
        //the ones that were open are stored in a list so that they can be reopened when the menu is closed
        for (int i = 0; i < widgetRoot.transform.childCount; i++)
        {
            if (widgetRoot.transform.GetChild(i).gameObject.activeSelf)
            {
                widgetRoot.transform.GetChild(i).gameObject.SetActive(false);
                openWidgets.Add(widgetRoot.transform.GetChild(i).gameObject);
            }
        }

        //all of the menu buttons need to be opened here
        toggleSettingsMenu.SetActive(true);
        dropCharacterButton.SetActive(true);
        menuButtonsOpen = true;
        tpCamRef.allowPlayerInput = false;
        Time.timeScale = .01f;
    }
}

public abstract class WidgetMenu : MonoBehaviour
{
    public WidgetMenu()
    {

    }

    public abstract void ToggleMenu();

}