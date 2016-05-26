using UnityEngine;
using System.Collections;

public static class UIUtilities {

    private static int gridIncrement = 50;

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
        Vector2 result = Vector2.zero;
        // if the x position cuts off the menu at the right of the screen, line the right side of the menu up with the right of the screen.
        if (rectObject.anchoredPosition.x > Screen.width - rectObject.sizeDelta.x)
            result += new Vector2((int)((Screen.width - rectObject.sizeDelta.x) / gridIncrement) * gridIncrement, 0);
        // if the x position is off the left of the screen, line the left side of the menu up with the left of the screen
        else if (rectObject.anchoredPosition.x < 0)
            result += new Vector2(0, 0);
        // if the menu is on the screen, snap to the grid
        else
            result += new Vector2((int)(rectObject.anchoredPosition.x / gridIncrement) * gridIncrement, 0);
        
        // if the y position is off the top of the screen, line the top of the menu up at the top of the screen
        if (rectObject.anchoredPosition.y > 0)
            result += new Vector2(0, 0);
        // if the y position cuts the menu off at the bottom of the screen, line the bottom of the menu up on the bottom of the screen
        else if (rectObject.anchoredPosition.y - rectObject.sizeDelta.y < -Screen.height)
            result += new Vector2(0, (int)((-Screen.height + rectObject.sizeDelta.y)/gridIncrement)*gridIncrement);
        // if the menu is on the screen, snap to the grid
        else
            result += new Vector2(0, (int)(rectObject.anchoredPosition.y / gridIncrement) * gridIncrement);

        rectObject.anchoredPosition = result;

    }
}
