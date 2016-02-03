using UnityEngine;
using System.Collections;

public class FlystickTest : MonoBehaviour {

    public GameObject flystickTracker;
    private TrackerData data;
    public float axisZero;
    public float axisOne;

    public bool buttonZero;
    public bool buttonOne;
    public bool buttonTwo;
    public bool buttonThree;
    public bool buttonFour;
    public bool buttonFive;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		data = (TrackerData)InputManager.map.GetValue("Tracker");
        flystickTracker.transform.position = data.Position;
        flystickTracker.transform.rotation = data.Rotation;
        axisZero = (float)InputManager.map.GetValue("Axis0");
        axisOne = (float)InputManager.map.GetValue("Axis1");

        buttonZero = (bool)InputManager.map.GetValue("Button0");
        buttonOne = (bool)InputManager.map.GetValue("Button1");
        buttonTwo = (bool)InputManager.map.GetValue("Button2");
        buttonThree = (bool)InputManager.map.GetValue("Button3");
        buttonFour = (bool)InputManager.map.GetValue("Button4");
        buttonFive = (bool)InputManager.map.GetValue("Button5");

    }
}
