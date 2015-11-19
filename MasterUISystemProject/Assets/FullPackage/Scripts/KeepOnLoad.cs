using UnityEngine;
using System.Collections;

public class KeepOnLoad : MonoBehaviour {

	public bool firstCreated = false;

	// Use this for initialization
	void Awake () {
		firstCreated = (GameObject.FindGameObjectsWithTag ("KeepOnLoad").Length == 1);
		if (!firstCreated)
			Destroy (gameObject);
		gameObject.SetActive (firstCreated);
		DontDestroyOnLoad (gameObject);
	}
}
