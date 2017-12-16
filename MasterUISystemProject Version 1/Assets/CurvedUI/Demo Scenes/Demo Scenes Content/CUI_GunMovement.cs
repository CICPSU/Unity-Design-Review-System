using UnityEngine;
using System.Collections;

public class CUI_GunMovement : MonoBehaviour {

	[SerializeField] Transform pivot;
	[SerializeField]  float sensitivity = 0.1f;
	Vector3 lastMouse;

	// Use this for initialization
	void Start () {
		lastMouse = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 mouseDelta = Input.mousePosition - lastMouse;
		lastMouse = Input.mousePosition;
		pivot.localEulerAngles += new Vector3 (-mouseDelta.y,  mouseDelta.x, 0) * sensitivity;

	}
}
