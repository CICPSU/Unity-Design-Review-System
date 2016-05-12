using UnityEngine;
using System.Collections;

public class WidgetTransitions : MonoBehaviour {

    public RectTransform widgetRoot;
    public RectTransform widgetConfig;

    private int rootDirection = 1;
    private int configDirection = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.R))
            SlideWidgetRoot();

        if (Input.GetKeyUp(KeyCode.C))
            SlideWidgetConfig();

        if (Input.GetKeyUp(KeyCode.B))
            ClearScreen();

	}

    public void SlideWidgetRoot()
    {
        iTween.MoveBy(widgetRoot.gameObject, iTween.Hash("y", Screen.height * rootDirection, "easeType", "easeInOutExpo", "time", .5f));
        rootDirection *= -1;
    }

    public void SlideWidgetConfig()
    {
        iTween.MoveBy(widgetConfig.gameObject, iTween.Hash("y", Screen.height * configDirection, "easeType", "easeInOutExpo", "time", .5f));
        configDirection *= -1;
    }

    public void ClearScreen()
    {
        SlideWidgetRoot();
        SlideWidgetConfig();
    }
}
