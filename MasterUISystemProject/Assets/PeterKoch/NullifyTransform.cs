using UnityEngine;
using System.Collections;

public class NullifyTransform : MonoBehaviour {

    public Transform t;

	void LateUpdate () {
        if (t != null)
        {
            transform.localRotation = Quaternion.Inverse(t.localRotation);
            transform.localPosition = -t.localPosition;
        }
	} 
}
