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
    
    public static void PlaceMenuObject(RectTransform rectObject, Vector3[] corners)
    {
        Rect screenRect = new Rect(0,0,Screen.width,Screen.height);

        // need to handle case where all 4 corners are off the screen

        // if the bottom left corner is off the screen
        if (!screenRect.Contains(corners[0]))
        {
            // if both the left side corners are off the screen
            if (!screenRect.Contains(corners[1]))
            {
                rectObject.Translate(-corners[1].x, 0, 0);
            }

            // if both of the bottom corners are off the screen
            if (!screenRect.Contains(corners[3]))
            {
                rectObject.Translate(0, -corners[3].y, 0);
            }
        }

        // if the top right corner is off the screen
        if (!screenRect.Contains(corners[2]))
        {
            // if both the right side corners are off the screen
            if (!screenRect.Contains(corners[3]))
            {
                rectObject.Translate(-corners[3].x + Screen.width, 0, 0);
            }

            // if both of the top corners are off the screen
            if (!screenRect.Contains(corners[1]))
            {
                rectObject.Translate(0, -corners[1].y + Screen.height, 0);
            }
        }


    }
}
