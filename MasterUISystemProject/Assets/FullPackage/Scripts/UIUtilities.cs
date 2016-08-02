using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UIUtilities {

	/// <summary>
	/// Sets the pop up panel. make sure that the panels are within screen
	/// </summary>
	/// <returns>The pop up panel.</returns>
	/// <param name="panel">Panel.</param>
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
