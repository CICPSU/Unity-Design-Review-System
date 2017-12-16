using UnityEngine;
using System.Collections;

//this class should be attached to every widget. 
//toggling the active state of each widget should be through this class rather than directly toggle gameobject
public class Widget : MonoBehaviour {

	private bool active = true;

	public bool Active{
		get{return active;}
		set{
			foreach(Transform child in transform){
				//only control the true on screen UI. UIs for 3d objects are controlled separatedly
				//objs tagged "UIfor3Dobjects":DropCharacterInfoPanel, DropCharacterEditPanel, MarkerInfoCanvas 
				if(child.gameObject.tag != "UIfor3Dobjects"){
					child.gameObject.SetActive(value);
				}
			}
			active = value;
		}
	}

}
