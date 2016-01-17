using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WidgetCanvasManager : MonoBehaviour {

    public GameObject toggleSettingsMenu;
    public GameObject dropCharacter;
    public GameObject chooseWidgetPanel;
    public GameObject displaySettingsPanel;
    public GameObject errorWindow;
    public GameObject toggleMenuButton;

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
        //toggleMenuButton.GetComponent<Animator>().SetBool("Open", false);
        //toggleMenuButton.GetComponent<Animator>().SetTrigger("Normal");
        toggleSettingsMenu.SetActive(false);
        dropCharacter.SetActive(false);
        chooseWidgetPanel.SetActive(false);
        displaySettingsPanel.SetActive(false);
        errorWindow.SetActive(false);
        menuOpen = false;
    }

    private void OpenMenu()
    {
        //toggleMenuButton.GetComponent<Animator>().SetBool("Open", true);
        toggleSettingsMenu.SetActive(true);
        dropCharacter.SetActive(true);
        menuOpen = true;
    }
}
