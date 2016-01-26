using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeScalePrint : MonoBehaviour {

    public Text timeScaleText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (Time.timeScale == 0)
                Time.timeScale = 1f;
            else if (Time.timeScale < .1f)
                Time.timeScale = 0f;
            else
                Time.timeScale -= .1f;

           
        }
        timeScaleText.text = "TimeScale = " + Time.timeScale;
	}
}
