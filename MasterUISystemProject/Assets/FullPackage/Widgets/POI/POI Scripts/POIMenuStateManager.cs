using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// This script manages the changes to the POIMenu when going between Edit mode and Normal mode.
/// The MenuStateChange delegate allows otehr scripts to add functions that will be called when the state of the POIMenu changes.
/// </summary>
public class POIMenuStateManager : MonoBehaviour {

	public List<MonoBehaviour> disableWhileMenuOpen = new List<MonoBehaviour> ();

	private static bool editModeState = false;

	private bool finshedSetupForModeChange = false;

	public delegate void stateChangeDelegate();

	public static event stateChangeDelegate MenuStateChange;

	void Start(){
		MenuStateChange += reset;
	}

	public static bool EditModeState{
		get{

			return editModeState; //default of bool is false, no need to initialize

		}
		set{
			if(editModeState != value){
				MenuStateChange();
			}

			editModeState = value;
		}
	}

	void Update ()
	{
		if(!finshedSetupForModeChange){
            //this if/else statement changes the time scale while the POIMenu is in Edit mode.
			if (!editModeState){
                ControlUtilities.UnPause();
			}
			else{
                ControlUtilities.Pause();
			}

			//toggle all of the scripts in disableWhileMenuOpen
            //scripts can be added to this list so that they will not be active while the POIMenu is in Edit mode.
			foreach(MonoBehaviour mono in disableWhileMenuOpen)
			{
				mono.enabled = !editModeState;
			}

			finshedSetupForModeChange = true;
		}
	}

	private void reset(){
		finshedSetupForModeChange = false;
	}
}
