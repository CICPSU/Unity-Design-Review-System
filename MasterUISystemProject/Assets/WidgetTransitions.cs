using UnityEngine;
using System.Collections;

public class WidgetTransitions : MonoBehaviour {

    public static WidgetTransitions Instance;

    public RectTransform widgetRoot;
    public RectTransform widgetConfig;
    public GameObject widget3D;

    private int rootDirection = 1;
    private int configDirection = -1;

	// Use this for initialization
	void Start () {
        if (Instance == null)
            Instance = this;
        widget3D.transform.position = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.R) && iTween.Count(widgetRoot.gameObject) == 0)
            SlideWidgetRoot();

        if (Input.GetKeyUp(KeyCode.C) && iTween.Count(widgetConfig.gameObject) == 0)
            SlideWidgetConfig();

        if (Input.GetKeyUp(KeyCode.B) && iTween.Count(widgetRoot.gameObject) == 0 && iTween.Count(widgetConfig.gameObject) == 0)
            ClearScreen();

	}

    public void SlideWidgetRoot()
    {
        if (iTween.Count(widgetRoot.gameObject) != 0)
        {
            iTween.Stop(widgetRoot.gameObject);

            if (rootDirection == -1)
                widgetRoot.anchoredPosition = new Vector3(widgetRoot.anchoredPosition.x, 0, 0);
            else
                widgetRoot.anchoredPosition = new Vector3(widgetRoot.anchoredPosition.x, -Screen.height, 0);
        }

        iTween.MoveBy(widgetRoot.gameObject, iTween.Hash("y", Screen.height * rootDirection, "easeType", "easeInOutExpo", "time", .5f));
        rootDirection *= -1;
        
    }

    public void SlideWidgetConfig()
    {
        if (iTween.Count(widgetConfig.gameObject) != 0)
        {
            iTween.Stop(widgetConfig.gameObject);

            if (rootDirection == -1)
                widgetConfig.anchoredPosition = new Vector3(widgetConfig.anchoredPosition.x, 0, 0);
            else
                widgetConfig.anchoredPosition = new Vector3(widgetConfig.anchoredPosition.x, -Screen.height, 0);
        }


        iTween.MoveBy(widgetConfig.gameObject, iTween.Hash("y", Screen.height * configDirection, "easeType", "easeInOutExpo", "time", .5f));
        configDirection *= -1;
        
    }

    public void ClearScreen()
    {
        SlideWidgetRoot();
        SlideWidgetConfig();
    }
}
