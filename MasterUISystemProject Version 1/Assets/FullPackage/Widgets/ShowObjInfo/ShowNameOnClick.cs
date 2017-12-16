using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Contact Yifan Liu ivenaieseccqu@gmail.com for questions, comments or suggestions
// Please Acknowledge Penn State Computer Integrated Construction (CIC) Research Group(http://www.engr.psu.edu/ae/cic/) when using this script.

public class ShowNameOnClick : MonoBehaviour {
	string displayName;
	float distanceFromCamToClickedObject;
	Material[] MaterialsOnObject; 
	List<Color> originalColor = new List<Color>(); //stores the original color of the original materials
	public static bool clicked;  // if the material has been clicked.
	GameObject infoGUItext; //store the GUItext that shows name and distance
	
	void Start(){
		clicked = false;
	}

	//check if anything else has been selected, if so, deselect the object and make the scene ready for 
	//selecting new object.
	static bool ClickChecker(){
		if(DetectMouseClick.CheckObjectClickedLength() == 0){
			return true;
		}
		else{
			DetectMouseClick.CallObjectClickedEvent();
			return true;
		}

		//Report bug if anything goes wrong
		if(DetectMouseClick.CheckObjectClickedLength() > 1){
			Debug.Log("Error: two objects in ObjectClicked event! DetectMouseClick.cs");
			return false;
		}
	}


	void OnMouseUp(){
		if(ClickChecker()){
			if(!clicked){ // this disables all the objects after one is clicked
				MeasureDistance();
				DisplayNameAndDistance();
				ChangeMaterial();
				StartCoroutine (ClickHandler());
			}
		}
	}


	//measure the distance from the object
	void MeasureDistance(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		distanceFromCamToClickedObject = Vector3.Distance(hit.point,Camera.main.transform.position);
		distanceFromCamToClickedObject = distanceFromCamToClickedObject * 3.28f; //convert meter to feet
	}

	//display the distance and the object name
	void DisplayNameAndDistance(){
		int leftBracket = gameObject.name.IndexOf("[");

		if(leftBracket != -1){ // means found "[" in the name
			displayName = gameObject.name.Substring(0,leftBracket);
		}
		else{ // if name does not contain brackets, whic means the object is probably not from revit.
			displayName = gameObject.name.ToString();
		}

		infoGUItext = GameObject.FindWithTag ("ObjectInfo");
		infoGUItext.GetComponent<GUIText>().text = "Name: " + displayName + "\n Distance: " + distanceFromCamToClickedObject.ToString("0.00") + " feet";
	}

	//change the material of an object to cremson(red)
	void ChangeMaterial(){
		Color cremson = new Color(.86f,.0078f,.196f);
		MaterialsOnObject = GetComponent<Renderer>().materials; 

		//store original color in the originalColor list and change the object materials
		foreach(Material material in MaterialsOnObject){
			originalColor.Add (material.color);
			material.color = cremson;
		}
	}

	//change the material back after clicking again
	void RestoreMaterialAndGUItext(){
		for(int i = 0; i < MaterialsOnObject.Length; i++){
			MaterialsOnObject[i].color = originalColor[i];
		}
		if(infoGUItext != null){
			infoGUItext.GetComponent<GUIText>().text = "";
		}
		clicked = false;
		DetectMouseClick.ObjectClicked -= RestoreMaterialAndGUItext;
	}
	
	IEnumerator ClickHandler(){
		yield return null;  // wait for one frame before enabling the mouseclick listener in "DetectMouseClick"
		clicked = true;
		DetectMouseClick.ObjectClicked += RestoreMaterialAndGUItext; //the objectclicked event is used to call RestoreMaterialAndGUItext(), we also use its invocation list to make 
																	//sure only one object is 
	}
}