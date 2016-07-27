using UnityEngine;
using System.Collections;

public class ActiveWidgetManager : MonoBehaviour {
    
    public enum ActiveWidget { None, Sunlight, CharacterDrop, Bookmark, WidgetConfig}
    public static ActiveWidget currentActive = ActiveWidget.None;

    public void ActivateWidgetFromButton(ActiveWidget widgetToActivate)
    {
        ActivateWidget(widgetToActivate);
    }

    /// <summary>
    /// Widgets can call this function to activate.
    /// Returns true if 
    /// </summary>
    public static bool ActivateWidget(ActiveWidget widgetToActivate)
    {
        if (currentActive == ActiveWidget.None)
        {
            // these two if statements will make sure that the widgets that are not being activated are disabled
            if (widgetToActivate != ActiveWidget.Bookmark)
                SettingsManager.Instance.bm_GameObject.SetActive(false);
            if (widgetToActivate != ActiveWidget.Sunlight)
                SettingsManager.Instance.sl_GameObject.SetActive(false);

            // moves the widget config object off the screen
            if (WidgetTransitions.Instance != null
                && widgetToActivate != ActiveWidget.CharacterDrop
                && widgetToActivate != ActiveWidget.WidgetConfig)
                WidgetTransitions.Instance.SlideWidgetConfig();

            // since the minimap doesnt use an active state, it will always be disabled
            SettingsManager.Instance.mm_GameObject.SetActive(false);
            // set the reference for the currently active widget
            currentActive = widgetToActivate;
            return true;
        }

        // there was already an active widget, so return false
        return false;
    }

    public void DeactivateWidgetFromButton(ActiveWidget widgetToDeactivate)
    {
        DeactivateWidget(widgetToDeactivate);
    }

    /// <summary>
    /// Widgets can call this function to deactivate.
    /// </summary>
    public static void DeactivateWidget(ActiveWidget widgetToDeactivate)
    {
        if (widgetToDeactivate == currentActive)
        {
            if (SettingsManager.Instance != null)
            {
                if (!SettingsManager.Instance.bm_GameObject.activeSelf && SettingsManager.Instance.wc_Settings.bm_Enabled)
                    SettingsManager.Instance.bm_GameObject.SetActive(true);
                if (!SettingsManager.Instance.sl_GameObject.activeSelf && SettingsManager.Instance.wc_Settings.sl_Enabled)
                    SettingsManager.Instance.sl_GameObject.SetActive(true);
                if (!SettingsManager.Instance.mm_GameObject.activeSelf && SettingsManager.Instance.wc_Settings.mm_Enabled)
                    SettingsManager.Instance.mm_GameObject.SetActive(true);
            }

            if (WidgetTransitions.Instance != null
                && currentActive != ActiveWidget.CharacterDrop
                && currentActive != ActiveWidget.WidgetConfig)
                WidgetTransitions.Instance.SlideWidgetConfig();

            currentActive = ActiveWidget.None;
        }
    }
}
