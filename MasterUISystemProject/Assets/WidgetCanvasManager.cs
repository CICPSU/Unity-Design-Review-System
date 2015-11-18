using UnityEngine;
using System.Collections;

public class WidgetCanvasManager : MonoBehaviour {

    public GameObject toggleSettingsMenu;
    public GameObject dropCharacter;
    public GameObject chooseWidgetPanel;
    public GameObject displaySettingsPanel;
    public GameObject errorWindow;
    public GameObject openMenu;
    public GameObject closeMenu;

    public void CloseAll()
    {
        toggleSettingsMenu.SetActive(false);
        dropCharacter.SetActive(false);
        chooseWidgetPanel.SetActive(false);
        displaySettingsPanel.SetActive(false);
        errorWindow.SetActive(false);
        closeMenu.SetActive(false);
        openMenu.SetActive(true);
    }

    public void OpenMenu()
    {
        toggleSettingsMenu.SetActive(true);
        dropCharacter.SetActive(true);
        closeMenu.SetActive(true);
        openMenu.SetActive(false);
    }
}
