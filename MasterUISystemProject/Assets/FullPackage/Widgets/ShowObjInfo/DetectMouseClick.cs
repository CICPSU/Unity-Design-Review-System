using UnityEngine;
using System.Collections;


// Contact Yifan Liu ivenaieseccqu@gmail.com for questions, comments or suggestions
// Please Acknowledge Penn State Computer Integrated Construction (CIC) Research Group(http://www.engr.psu.edu/ae/cic/) when using this script.
public class DetectMouseClick : MonoBehaviour {
	public delegate void showInfoDelegate();
	public static event showInfoDelegate ObjectClicked;

	// Use this for initialization
	void Start () {
	
	}

	//return how many objects there are in the ObjectClicked event 
	public static int CheckObjectClickedLength(){
		if(ObjectClicked != null){
			return ObjectClicked.GetInvocationList().Length;
		}
		else{
			return 0;
		}
	}

	public static void CallObjectClickedEvent(){
		ObjectClicked();
	}

	// Update is called once per frame
	void Update () {
		//This script is activated when an object is clicked
		if(ShowNameOnClick.clicked){

			//After an object is clicked, listen to another click to cancel the click on the object.
			if(Input.GetMouseButtonUp(0)){
				Debug.Log ("pressed left mouse");
				if(ObjectClicked != null){
					if(ObjectClicked.GetInvocationList().Length > 1){
						Debug.Log("Error: more than one object subsribed to the ObjectClicked Event (DetectMouseClick.cs)");
						Debug.Log("Check if you have two ShowNameOnClickScript on the same object");
					}
					//invoke the RestoreMaterialAndGUItext function in ShowNameOnClick.cs to restore the material back to the original material.
					ObjectClicked();
				}
			}
		}
	}
}
