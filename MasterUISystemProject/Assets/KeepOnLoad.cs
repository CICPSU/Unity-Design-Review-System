using UnityEngine;
using System.Collections;

public class KeepOnLoad : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (gameObject);
		if (GameObject.FindGameObjectsWithTag ("KeepOnLoad").Length > 1)
			Destroy (gameObject);
	}
}
