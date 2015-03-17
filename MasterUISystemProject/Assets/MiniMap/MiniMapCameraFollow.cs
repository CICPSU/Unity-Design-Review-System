using UnityEngine;
using System.Collections;

public class MiniMapCameraFollow : MonoBehaviour {

	public Transform Target;
		
	// Update is called once per frame
	void LateUpdate () 
	{
		transform.position = new Vector3(Target.position.x, Target.position.y + 2.6f, Target.position.z);
		transform.eulerAngles = new Vector3 (Target.eulerAngles.x, Target.eulerAngles.y, Target.eulerAngles.z);
		
	}
}
