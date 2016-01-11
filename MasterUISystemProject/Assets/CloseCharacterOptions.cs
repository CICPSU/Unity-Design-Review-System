using UnityEngine;
using System.Collections;

public class CloseCharacterOptions : MonoBehaviour {

    public CharacterDropper charDropperScript;

    public void OnMouseUp()
    {
        charDropperScript.CloseCharacterOptions();
    }
}
