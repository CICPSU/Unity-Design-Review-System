using UnityEngine;
using System.Collections;

public class CharacterDropper : MonoBehaviour {

    public GameObject characterToDrop;

    private bool dropModeOn;

    public void ToggleMode()
    {

    }

    void Update()
    {
        /// here is where we will do the raycast and show a temporary character where it will be dropped
        /// we will have a reference to the temporary character and update its position to wherever the raycast from the mouse is pointing
        /// when "dropping" we will just stop updating the position
        /// need to make sure the temporary character is deleted/removed whenever the dropcharacter button is disabled (this script)
        if (dropModeOn)
        {
            
        }
    }
}
