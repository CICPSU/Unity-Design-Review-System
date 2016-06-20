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
        BoxCollider2D boxCollider2d = rectObject.GetComponent<BoxCollider2D>();

        // if the bottom left corner is off the screen
        if (!screenRect.Contains(objectCorners[0]))
        {
            // if both the left side corners are off the screen
            if (!screenRect.Contains(objectCorners[1]))
            {
                rectObject.Translate(-objectCorners[1].x, 0, 0);
            }

            // if both of the bottom corners are off the screen
            if (!screenRect.Contains(objectCorners[3]))
            {
                rectObject.Translate(0, -objectCorners[3].y, 0);
            }
        }

        // if the top right corner is off the screen
        if (!screenRect.Contains(objectCorners[2]))
        {
            // if both the right side corners are off the screen
            if (!screenRect.Contains(objectCorners[3]))
            {
                rectObject.Translate(-objectCorners[3].x + Screen.width, 0, 0);
            }

            // if both of the top corners are off the screen
            if (!screenRect.Contains(objectCorners[1]))
            {
                rectObject.Translate(0, -objectCorners[1].y + Screen.height, 0);
            }
        }


    }
}
