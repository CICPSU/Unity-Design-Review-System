using UnityEngine;
using System.Collections;

public class EditModeManager : MonoBehaviour {

    public static bool inEditMode = false;
    private static bool editHasRaycastLock = false;
    public static RectTransform widgetInEdit = null;

    public void EnterEditFromButton(RectTransform widgetToEnter)
    {
        EnterEditMode(widgetToEnter);
    }

    /// <summary>
    /// Widgets can call this function to enter into their edit mode.
    /// </summary>
    public static void EnterEditMode(RectTransform widgetToEnterEdit)
    {
        if (!inEditMode)
        {
            editHasRaycastLock = RaycastLock.GetLock(true);

            // these two if statements will make sure that the widgets that are not going into edit mode are disabled
            if (widgetToEnterEdit != SettingsManager.Instance.bm_GameObject.GetComponent<RectTransform>())
                SettingsManager.Instance.bm_GameObject.SetActive(false);
            if (widgetToEnterEdit != SettingsManager.Instance.sl_GameObject.GetComponent<RectTransform>())
                SettingsManager.Instance.sl_GameObject.SetActive(false);

            // moves the widget config object off the screen
            if(WidgetTransitions.Instance != null 
                && widgetToEnterEdit != SettingsManager.Instance.ci_Gameobject.GetComponent<RectTransform>() 
                && widgetToEnterEdit != SettingsManager.Instance.wc_Gameobject.GetComponent<RectTransform>())
                WidgetTransitions.Instance.SlideWidgetConfig();

            // since the minimap doesnt have an edit mode, it will always be disabled
            SettingsManager.Instance.mm_GameObject.SetActive(false);
            inEditMode = true;
            widgetInEdit = widgetToEnterEdit;
        }
    }

    public void ExitEditFromButton()
    {
        ExitEditMode();
    }

    /// <summary>
    /// Widgets can call this function to exit their edit mode.
    /// </summary>
    public static void ExitEditMode()
    {
        if (inEditMode)
        {
            if (SettingsManager.Instance != null)
            {
                if (!SettingsManager.Instance.bm_GameObject.activeSelf)
                    SettingsManager.Instance.bm_GameObject.SetActive(true);
                if (!SettingsManager.Instance.sl_GameObject.activeSelf)
                    SettingsManager.Instance.sl_GameObject.SetActive(true);
                if (!SettingsManager.Instance.mm_GameObject.activeSelf)
                    SettingsManager.Instance.mm_GameObject.SetActive(true);
            }
            if (WidgetTransitions.Instance != null 
                && widgetInEdit != SettingsManager.Instance.ci_Gameobject.GetComponent<RectTransform>() 
                && widgetInEdit != SettingsManager.Instance.wc_Gameobject.GetComponent<RectTransform>())
                WidgetTransitions.Instance.SlideWidgetConfig();
            inEditMode = false;

            widgetInEdit = null;

            if (editHasRaycastLock)
                RaycastLock.GiveLock();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
