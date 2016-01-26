using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WidgetSettingsMenuManager : WidgetMenu {

    public Image buttonImage;
    public GameObject chooseWidgetPanel;
    public WidgetSettingsManager settingsManager;
    public WidgetCanvasManager canvasManager;

    private bool shouldOpen = false;

    public override void ToggleMenu()
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
            buttonImage.color = Color.white;
        }
    }
}
