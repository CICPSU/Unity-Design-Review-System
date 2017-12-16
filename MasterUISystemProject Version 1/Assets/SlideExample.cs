using UnityEngine;
using System.Collections;

public class SlideExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.MoveBy(gameObject, iTween.Hash("y", Screen.height/2 + 15, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
