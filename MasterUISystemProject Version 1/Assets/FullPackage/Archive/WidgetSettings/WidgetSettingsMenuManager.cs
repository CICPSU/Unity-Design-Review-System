using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WidgetSettingsMenuManager : MonoBehaviour {

    public Image buttonImage;
    public GameObject chooseWidgetPanel;
    public GameObject displaySettingsPanel;
    public WidgetSettingsManager settingsManager;

    private bool shouldOpen = false;

    public void ToggleMenu()
    {
        shouldOpen = !shouldOpen;
        if (shouldOpen)
        {
            chooseWidgetPanel.SetActive(true);
            buttonImage.color = Color.red;
            settingsManager.GenerateSettingsFileButtons();
        }
        else
        {
            chooseWidgetPanel.SetActive(false);
            displaySettingsPanel.SetActive(false);
            buttonImage.color = Color.white;
        }
    }
}
