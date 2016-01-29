using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WidgetCanvasManager : MonoBehaviour {

    public GameObject toggleSettingsMenu;
    public GameObject dropCharacterButton;
    public GameObject chooseWidgetPanel;
    public GameObject displaySettingsPanel;
    public GameObject errorWindow;

    public WidgetMenu openMenu = null;

    public bool menuOpen = false;

    void Start()
    {
        CloseAll();
    }

    public void OpenMenu(WidgetMenu menuToOpen)
    {
        if (openMenu == null)
        {
            menuToOpen.ToggleMenu();
            openMenu = menuToOpen;
        }
        else if(openMenu == menuToOpen)
        {
            openMenu.ToggleMenu();
            openMenu = null;
        }
        else
        {
            openMenu.ToggleMenu();
            menuToOpen.ToggleMenu();
            openMenu = menuToOpen;
        }

    }

    private void ToggleMenu()
    {
        if (menuOpen)
            CloseAll();
        else
            OpenMenu();
    }

    private void CloseAll()
    {
        toggleSettingsMenu.SetActive(false);
        dropCharacterButton.SetActive(false);
        chooseWidgetPanel.SetActive(false);
        displaySettingsPanel.SetActive(false);
        errorWindow.SetActive(false);
        menuOpen = false;
    }

    private void OpenMenu()
    {
        toggleSettingsMenu.SetActive(true);
        dropCharacterButton.SetActive(true);
        menuOpen = true;
    }
}

public abstract class WidgetMenu : MonoBehaviour
{
    public WidgetMenu()
    {

    }

    public abstract void ToggleMenu();

}