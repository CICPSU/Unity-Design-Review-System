using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Image>().CrossFadeAlpha(0, 2.5f, true);
        transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(0, 2.5f, true);
    }



	// Update is called once per frame
	void Update () {
	
	}
}
