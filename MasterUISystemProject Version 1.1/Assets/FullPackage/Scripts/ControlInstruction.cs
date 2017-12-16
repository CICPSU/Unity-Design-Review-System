using UnityEngine;
using System.Collections;

public class ControlInstruction : MonoBehaviour {
	public Rect InstructionLabelRect;
	void Awake(){
		InstructionLabelRect = new Rect (20, 30, 300, 70);	
	}
		
void OnGUI(){
		GUI.Box (InstructionLabelRect, "");
		GUI.Label(InstructionLabelRect, "Scroll Wheel to Zoom/Switch to first person\n A,D to rotate \n W,S to move forward and backward " );	
	}
}
