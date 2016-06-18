using UnityEngine;
using System.Collections;

public static class IConUtilities {

    public static Vector3 SetPopUpPanel(RectTransform panel)
    {
        Vector3 returnPos = Input.mousePosition;

        if(returnPos.x + panel.sizeDelta.x > Screen.width)
            returnPos -= new Vector3( panel.sizeDelta.x - (Screen.width - returnPos.x),0,0);

        if (returnPos.y + panel.sizeDelta.y > Screen.height)
            returnPos -= new Vector3( 0, panel.sizeDelta.y - (Screen.height - returnPos.y),0);

        return returnPos;
    }
}
