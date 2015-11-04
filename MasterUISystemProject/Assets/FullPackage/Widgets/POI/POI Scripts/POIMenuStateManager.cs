using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

			return editModeState; //default of bool is faulse, no need to initialize

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
			if (!editModeState){
				Time.timeScale = 1;
			}
			else{
				Time.timeScale = 0;
			}

			Time.fixedDeltaTime = 0.02f * Time.timeScale; // fixed update is 50fps, which is 0.02s when time scale is 1

			//switch off 
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
