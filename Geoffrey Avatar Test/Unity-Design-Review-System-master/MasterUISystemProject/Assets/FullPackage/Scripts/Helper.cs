using System;
using UnityEngine;

public static class Helper
{
	public struct ClipPlanePoints
	{
		public Vector3 UpperLeft;	
		public Vector3 UpperRight;	
		public Vector3 LowerLeft;	
		public Vector3 LowerRight;
		
	}
	
	
	public static float ClampAngle(float angle, float min, float max){
		do{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
		}
		while (angle < -360 || angle > 360);
		
		return Mathf.Clamp(angle, min, max);
		
		
	}
	
	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos){  // pos is the position of the camera
		var ClipPlanePoints = new ClipPlanePoints();
		if (Camera.main == null)
			return ClipPlanePoints;
		var transform = Camera.main.transform;
		
		
		//FOV is Field of View
		var halfFOV = (Camera.main.fieldOfView/2)*Mathf.Deg2Rad;  // converting the FOV from degree to radiance, which is used by unity for angle calculation.
		
		var aspect = Camera.main.aspect;
		var distance = Camera.main.nearClipPlane; //distance from main camera to the near clip plane.
		var height = distance * Mathf.Tan(halfFOV);
		var width = height * aspect;
		
		ClipPlanePoints.LowerRight = pos + transform.right *width;  // move the clip plane point lowerright to the right by the width
		ClipPlanePoints.LowerRight -= transform.up * height; 
		ClipPlanePoints.LowerRight += transform.forward * distance; // move the point forward from the camera
		
		ClipPlanePoints.LowerLeft = pos - transform.right *width;  // move the clip plane point  to the right by the width
		ClipPlanePoints.LowerLeft -= transform.up * height; 
		ClipPlanePoints.LowerLeft += transform.forward * distance; // move the point forward from the camera
		
		ClipPlanePoints.UpperRight = pos + transform.right *width;  // move the clip plane point  to the right by the width
		ClipPlanePoints.UpperRight += transform.up * height; 
		ClipPlanePoints.UpperRight += transform.forward * distance; // move the point forward from the camera
		
		ClipPlanePoints.UpperLeft = pos - transform.right *width;  // move the clip plane point to the right by the width
		ClipPlanePoints.UpperLeft += transform.up * height; 
		ClipPlanePoints.UpperLeft += transform.forward * distance; // move the point forward from the camera
		
		return ClipPlanePoints;
	}
		
	
}

