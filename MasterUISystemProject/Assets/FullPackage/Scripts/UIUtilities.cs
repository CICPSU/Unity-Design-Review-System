using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UIUtilities {

    public static List<Collider2D> collider2Ds = new List<Collider2D>();

    public static Vector3 SetPopUpPanel(RectTransform panel)
    {
        Vector3 returnPos = Input.mousePosition;

        if(returnPos.x + panel.sizeDelta.x > Screen.width)
            returnPos -= new Vector3( panel.sizeDelta.x - (Screen.width - returnPos.x),0,0);

        if (returnPos.y + panel.sizeDelta.y > Screen.height)
            returnPos -= new Vector3( 0, panel.sizeDelta.y - (Screen.height - returnPos.y),0);

        return returnPos;
    }

    // this was written for use with the POIMenu object as an example and will be expanded to work with different pivot/anchor layouts
    public static void PlaceMenuObject(RectTransform rectObject)
    {
        Vector3[] objectCorners = new Vector3[4];
        rectObject.GetWorldCorners(objectCorners);
        Rect screenRect = new Rect(0,0,Screen.width,Screen.height);
        Debug.Log(objectCorners[0] + " " + objectCorners[1] + " " + objectCorners[2] + " " + objectCorners[3]);
        /*
        Vector2 result = Vector2.zero;
        // if the x position cuts off the menu at the right of the screen, line the right side of the menu up with the right of the screen.
        if (!screenRect.Contains())
            result += new Vector2(Screen.width - rectObject.sizeDelta.x, 0);
        // if the x position is off the left of the screen, line the left side of the menu up with the left of the screen
        else if (rectObject.anchoredPosition.x - rectObject.pivot.x * rectObject.sizeDelta.x < 0)
            result += new Vector2(0, 0);
        // if the menu is on the screen, keep its x position
        else
            result += new Vector2(rectObject.anchoredPosition.x, 0);
        
        // if the menu is cut off on the top of the screen, line the top of the menu up at the top of the screen
        if (rectObject.anchoredPosition.y + (1f - rectObject.pivot.y) * rectObject.sizeDelta.y > Screen.height)
            result += new Vector2(0, Screen.height - );
        // if the y position cuts the menu off at the bottom of the screen, line the bottom of the menu up on the bottom of the screen
        else if (rectObject.anchoredPosition.y  - rectObject.pivot.y * rectObject.sizeDelta.y < -Screen.height)
            result += new Vector2(0, );
        // if the menu is on the screen, snap to the grid
        else
            result += new Vector2(0, );

        rectObject.anchoredPosition = result;
        */
    }
}
