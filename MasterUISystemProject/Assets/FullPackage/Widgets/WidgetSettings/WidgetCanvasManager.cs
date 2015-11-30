using UnityEngine;
using System.Collections;

public class WidgetCanvasManager : MonoBehaviour {

    public GameObject toggleSettingsMenu;
    public GameObject dropCharacter;
    public GameObject chooseWidgetPanel;
    public GameObject displaySettingsPanel;
    public GameObject errorWindow;

    private bool menuOpen = false;

    void Start()
    {
        CloseAll();
    }

    public void ToggleMenu()
    {
        if (menuOpen)
            CloseAll();
        else
            OpenMenu();
    }

    private void CloseAll()
    {
        toggleSettingsMenu.SetActive(false);
        dropCharacter.SetActive(false);
        chooseWidgetPanel.SetActive(false);
        displaySettingsPanel.SetActive(false);
        errorWindow.SetActive(false);
        menuOpen = false;
    }

    private void OpenMenu()
    {
        toggleSettingsMenu.SetActive(true);
        dropCharacter.SetActive(true);
        menuOpen = true;
    }
}
