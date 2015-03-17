using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//This class sets the position of DisplayOrigin based on if tracking data exists
public class DisplaySystemHandler : MonoBehaviour {
	public int fieldOfView_oneScreen = 60;

	//called by Artrack start(), when no tracking data coming in, set the head to the geocenter of the projecting system. 
	//provides generic methods to calculate the geometric center(the point that is equal distance to all screens) of a display system.
	public Vector3 calculateGeometricCenter(){
		List<Vector3> listOfIntersections = new List<Vector3>();
		List<Vector3> listOfScreenCenter = new List<Vector3>();

		//if there is only one screen, set the geometry center to a location such that the field of view is symetric and equals to fieldOfView_oneScreen
		if(DIRE.Instance.settings.screens.Count == 1){
			//vector(0,0, -1) is the normal of the screen before rotation. Quarternion * vector rotate the vector by the quaternion
			//screenCenterOne(center point of screen 1) and normal vector defines a line starting from the center point. 
			Vector3 normalVectorScreenOne = Quaternion.Euler(DIRE.Instance.settings.screens[0].Location.Orientation) * new Vector3(0, 0, -1); 
			Vector3 screenCenterOne = Quaternion.Euler(DIRE.Instance.settings.screens[0].Location.Orientation) * (new Vector3(DIRE.Instance.settings.screens[0].Location.Width * 0.5f, DIRE.Instance.settings.screens[0].Location.Height * 0.5f, 0)) + DIRE.Instance.settings.screens[0].Location.Position;	
			float distanceFromScreenCenter = (DIRE.Instance.settings.screens[0].Location.Width/2)/Mathf.Tan(fieldOfView_oneScreen/2);
			return normalVectorScreenOne*distanceFromScreenCenter + screenCenterOne;
		}
	
		//find all the intersections points of all screen pairs.
		for(int i = 0; i < DIRE.Instance.settings.screens.Count; i++){
			for(int j = i + 1; j < DIRE.Instance.settings.screens.Count; j++){
				//vector(0,0, -1) is the normal of the screen before rotation. Quarternion * vector rotate the vector by the quaternion
				//screenCenterOne(center point of screen 1) and normal vector defines a line starting from the center point. 
				Vector3 normalVectorScreenOne = Quaternion.Euler(DIRE.Instance.settings.screens[i].Location.Orientation) * new Vector3(0, 0, -1); 
				Vector3 screenCenterOne = Quaternion.Euler(DIRE.Instance.settings.screens[i].Location.Orientation) * (new Vector3(DIRE.Instance.settings.screens[i].Location.Width * 0.5f, DIRE.Instance.settings.screens[i].Location.Height * 0.5f, 0)) + DIRE.Instance.settings.screens[i].Location.Position;

				listOfScreenCenter.Add(screenCenterOne);

				Vector3 normalVectorScreenTwo = Quaternion.Euler(DIRE.Instance.settings.screens[j].Location.Orientation) * new Vector3(0, 0, -1);
				Vector3 screenCenterTwo = Quaternion.Euler(DIRE.Instance.settings.screens[j].Location.Orientation) * (new Vector3(DIRE.Instance.settings.screens[j].Location.Width * 0.5f, DIRE.Instance.settings.screens[j].Location.Height * 0.5f, 0)) + DIRE.Instance.settings.screens[j].Location.Position;

				Vector3 intersectionPoint = calculateIntersectionOfTwoLines(normalVectorScreenOne, normalVectorScreenTwo, screenCenterOne, screenCenterTwo);

				//vector3(10000,0,0) means no intersection
				if(intersectionPoint != new Vector3(10000, 0, 0)){
					listOfIntersections.Add(intersectionPoint);
				}
			}
		}

		//calculate the average of all the intersection of screen normals
		Vector3 averageOfIntersection = Vector3.zero;
		if(listOfIntersections.Count != 0){ 
			foreach(Vector3 item in listOfIntersections){
				averageOfIntersection += item;
			}
			averageOfIntersection /= listOfIntersections.Count;
		}else{ 
			//****!!!!!! WE ASSUME WHEN NORMAL OF MULTIPLE SCREENS DONT INTERSECT, THE SCREENS ARE PARALLEL AND FORM ONE BIG WALL
			//*****!!!IF YOUR SYSTEM HAS NO PARALLEL SCREENS AND THOSE SCREENS DON'T HAVE INTERSECTIONS, THIS WON'T WORK
			//calculates physical distance(in meter) corresponding to a pixel
			float metersByPixel = DIRE.Instance.settings.screens[0].Location.Width/DIRE.Instance.settings.screens[0].Viewport.Width;
			float totalScreenWidth = metersByPixel * DIRE.Instance.settings.Width;
			Vector3 parallelScreenNormal = Quaternion.Euler(DIRE.Instance.settings.screens[0].Location.Orientation) * new Vector3(0, 0, -1);

			Vector3 averageOfScreenCenters = Vector3.zero;
			foreach(Vector3 item in listOfScreenCenter){
				averageOfScreenCenters += item;
			}
			averageOfScreenCenters /= listOfScreenCenter.Count;
			averageOfIntersection = averageOfScreenCenters + parallelScreenNormal * (totalScreenWidth/2)/Mathf.Tan(fieldOfView_oneScreen/2);
		}

		//return the average of the screen intersections
		return averageOfIntersection;
	}
	
	//returns the point of interesection of two lines, or (10000, 0, 0) if they don't intersect
	//positionOne and positionTwo are the center points of the screens
	Vector3 calculateIntersectionOfTwoLines(Vector3 normalOne, Vector3 normalTwo, Vector3 positionOne, Vector3 positionTwo){
		//source code: http://stackoverflow.com/questions/2316490/the-algorithm-to-find-the-point-of-intersection-of-two-3d-line-segment
		Vector3 dC = positionTwo - positionOne;

		//if two normal vectors overlaps
		if(Vector3.Cross(normalOne,normalTwo) == Vector3.zero){
			return (positionOne + (dC.magnitude /2)*normalOne);
		}

		if(Vector3.Dot(dC,Vector3.Cross(normalOne,normalTwo)) == 0.0){
			Vector3 abCross = Vector3.Cross(normalOne,normalTwo);
			float s = Vector3.Dot(Vector3.Cross(dC,normalTwo),Vector3.Cross(normalOne,normalTwo))/(Mathf.Pow(abCross.x,2)+Mathf.Pow(abCross.y,2)+Mathf.Pow(abCross.z,2));
			Vector3 intersectionPoint = positionOne + normalOne*s;
			return(intersectionPoint);
		}
		return(new Vector3(10000,0,0));
	}

	public void offsetDisplayOriginByGeometricCenter(){ 
		DIRE.Instance.DisplayOrigin.transform.localPosition = - DIRE.Instance.displayGeometricCenter;
	}

	public void offsetHeadToGeometricCenter(){
		DIRE.Instance.Head.transform.localPosition = DIRE.Instance.displayGeometricCenter;
	}

}
