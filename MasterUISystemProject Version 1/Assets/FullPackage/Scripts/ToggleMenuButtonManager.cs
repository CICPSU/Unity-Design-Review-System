using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ToggleMenuButtonManager : MonoBehaviour {

    public Vector2 hiddenPosition = new Vector2(-25, -25);
    public RectTransform toggleMenuButton;
    public float transitionSeconds = 1f;

    public bool clickedState = false;
    public bool hoverState = false;

    private RectTransform thisRect;
    public float clickTime = 0;
    public float hoverTime = 0;
    public float neitherTime = 0;

    void Start()
    {
        thisRect = GetComponent<RectTransform>();
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.mousePosition.x > thisRect.position.x && Input.mousePosition.x < thisRect.position.x + thisRect.sizeDelta.x)
        {
            if (Input.mousePosition.y > thisRect.position.y && Input.mousePosition.y < thisRect.position.y + thisRect.sizeDelta.y)
            {
                hoverState = true;
                neitherTime = 0;
                if (hoverTime == 0)
                    hoverTime = Time.time;
                /*
                if (Input.GetMouseButtonUp(0))
                {
                    clickedState = !clickedState;
                    clickTime = Time.time;
                }
                */
            }
        }
        else
        {
            hoverState = false;
            hoverTime = 0f;
            if (neitherTime == 0)
                neitherTime = Time.time;
        }

        if (clickedState)
        {
            toggleMenuButton.anchoredPosition = new Vector2(Mathf.Lerp(toggleMenuButton.anchoredPosition.x, 0, (Time.time - clickTime) / transitionSeconds),
                                                            Mathf.Lerp(toggleMenuButton.anchoredPosition.y, 0, (Time.time - clickTime) / transitionSeconds));
        }
        else if (hoverState)
        {
            toggleMenuButton.anchoredPosition = new Vector2(Mathf.Lerp(toggleMenuButton.anchoredPosition.x, 0, (Time.time - hoverTime) / transitionSeconds),
                                                            Mathf.Lerp(toggleMenuButton.anchoredPosition.y, 0, (Time.time - hoverTime) / transitionSeconds));
        }
        else
        {
            toggleMenuButton.anchoredPosition = new Vector2(Mathf.Lerp(toggleMenuButton.anchoredPosition.x, hiddenPosition.x, (Time.time - neitherTime) / transitionSeconds),
                                                            Mathf.Lerp(toggleMenuButton.anchoredPosition.y, hiddenPosition.y, (Time.time - neitherTime) / transitionSeconds));
        }

	}
}
