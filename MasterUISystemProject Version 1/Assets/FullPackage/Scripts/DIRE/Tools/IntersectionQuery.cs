using UnityEngine;
using System.Collections;
using System;

public class IntersectionQuery : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		lineRenderer = gameObject.GetComponent<LineRenderer> ();
	}
	
	RaycastHit hit;
	LineRenderer lineRenderer = null;
/*
	void OnGUI()
	{
		if (hit.distance > 0) 
		{
			string data = ""; 

			data += hit.transform.gameObject.name + Environment.NewLine;
			data += hit.point + ":" + String.Format("{0:F2}", hit.distance) + "M" + Environment.NewLine;

//			foreach (Camera cam in Camera.allCameras)
//			{
//				Vector3 viewPortCoord = cam.WorldToViewportPoint( hit.point );
//
//				bool inView = viewPortCoord.x >= 0 && viewPortCoord.x <= 1f &&
//                              viewPortCoord.y >= 0 && viewPortCoord.y <= 1f &&
//                              viewPortCoord.z >= 0;
//
//				data += cam.name + ": " + viewPortCoord + ": " + inView + Environment.NewLine;
//
////				if ( true )
////				{
////					Vector3 screenPt = cam.WorldToScreenPoint (hit.point);
////					screenPt.y = Screen.height - screenPt.y;
////				}
//			
//			}

			GUI.Box (new Rect (2325, 10, 150, 40), data);
		}
	}
*/
	// Update is called once per frame
	void Update () 
	{
		/*
		Physics.Raycast (transform.position, transform.forward, out hit);
		if (hit.distance > 0)
			lineRenderer.SetPosition (1, new Vector3(0,0,1) * hit.distance);
		else
		*/
		lineRenderer.SetPosition(0, transform.position);
        if (DIRE.Instance.trackingActive)
            lineRenderer.SetPosition(1, transform.position + transform.forward * 100);
        else
            lineRenderer.SetPosition(1, transform.position);
	}
}
