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

        transform.position = Vector3.zero;

    }

    /// <summary>
    /// Always make sure this object is located at world Origin (0,0,0)
    /// </summary>
    void Update()
    {
        if (transform.position != Vector3.zero)
        {
            transform.position = Vector3.zero;
        }

    }
}
