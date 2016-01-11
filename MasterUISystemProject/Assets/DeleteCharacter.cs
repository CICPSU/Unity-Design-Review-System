using UnityEngine;
using System.Collections;

public class DeleteCharacter : MonoBehaviour {

    public CharacterDropper charDropperScript;

    public void OnMouseUp()
    {
        charDropperScript.DeleteCharacter();
    }
}
