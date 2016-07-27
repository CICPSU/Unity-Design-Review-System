using UnityEngine;
using System.Collections;

public class WidgetTransitions : MonoBehaviour {

    public static WidgetTransitions Instance;

    public RectTransform widgetRoot;
    public RectTransform widgetConfig;

    public int rootDirection = 2;
    public int configDirection = -2;

	// Use this for initialization
	void Start ()
    {
        if (Instance == null)
            Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetKeyUp(KeyCode.R) && iTween.Count(widgetRoot.gameObject) == 0)
            SlideWidgetRoot();

        if (Input.GetKeyUp(KeyCode.C) && iTween.Count(widgetConfig.gameObject) == 0)
            SlideWidgetConfig();
            */
        if (Input.GetKeyUp(TP_InputManager.instance.toggleInterface) && iTween.Count(widgetRoot.gameObject) == 0 && iTween.Count(widgetConfig.gameObject) == 0)
            ClearScreen();

	}

    /// <summary>
    /// This function is used to slide the widget root off of the screen.
    /// </summary>
    public void SlideWidgetRoot()
    {
        // if the widgetroot has an itween running on it, finish that motion before triggering the next motion
        if (iTween.Count(widgetRoot.gameObject) != 0)
        {
            iTween.Stop(widgetRoot.gameObject);

            if (rootDirection == -2)
                widgetRoot.anchoredPosition = new Vector3(widgetRoot.anchoredPosition.x, Screen.height * 2, 0);
            else
                widgetRoot.anchoredPosition = new Vector3(widgetRoot.anchoredPosition.x, 0, 0);
        }

        // start the itween to move the widgetroot
        iTween.MoveBy(widgetRoot.gameObject, iTween.Hash("y", Screen.height * rootDirection, "easeType", "easeInOutExpo", "time", .75f));
        rootDirection *= -1;
        
    }

    /// <summary>
    /// This function is used to slide the widget config off the screen.
    /// </summary>
    public void SlideWidgetConfig()
    {
        // if the widgetconfig has an itween running on it, finish that motion before triggering the next motion
        if (iTween.Count(widgetConfig.gameObject) != 0)
        {
            iTween.Stop(widgetConfig.gameObject);

            if (configDirection == -2)
                widgetConfig.anchoredPosition = new Vector3(widgetConfig.anchoredPosition.x, 0, 0);
            else
                widgetConfig.anchoredPosition = new Vector3(widgetConfig.anchoredPosition.x, -2 * Screen.height, 0);
        }

        // start the itween to move the widgetconfig
        iTween.MoveBy(widgetConfig.gameObject, iTween.Hash("y", Screen.height * configDirection, "easeType", "easeInOutExpo", "time", .75f));
        configDirection *= -1;
        
    }

    /// <summary>
    /// This function slides both the widgetroot and widgetconfig off the screen.
    /// </summary>
    public void ClearScreen()
    {
        SlideWidgetRoot();
        SlideWidgetConfig();
    }
}
