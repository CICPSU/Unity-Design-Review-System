using UnityEngine;
using System.Collections;

//this script is attached to the root object of Lynn
public class TP_InputManager : MonoBehaviour {
	public static TP_InputManager instance;
	public string forward = "w";
	public string backward = "s";
	public string leftward = "q";
	public string rightward = "e";
	public string elevate = "up";
	public string descend = "down";
	public string gravity = "g";
	public string rotateLeft = "a";
	public string rotateRight = "d";
	public float rotateKeySensitivity = .8f;
	public string increaseSpeed = "=";
	public string decreaseSpeed = "-";
	public string toggleInterface = "c";
	
	
	// Use this for initialization
	void Start () {
	 instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
