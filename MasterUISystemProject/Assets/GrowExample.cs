using UnityEngine;
using System.Collections;

public class GrowExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.ScaleBy(gameObject, iTween.Hash("x", 0, "y", 0, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
