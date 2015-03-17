 using UnityEngine;
using System.Collections;


public class TP_Animator: MonoBehaviour
{
	public enum  Direction{
		Stationary, Forward, Backward, Left, Right, 
		LeftForward, RightForward, LeftBackward, RightBackward, Vertical
	}
	
	
	public static TP_Animator Instance;  // This is just to create a reference to this class
	
	
	public Direction MoveDirection {get; set;}
	
	void Awake(){						//   so when calling the variables or methods of this class from other script,
	Instance = this;					//    we can refer to this instance.
	}
	void Start(){
		
	}
	
	void Update(){
		
	}
	
	public void DetermineCurrentMoveDirection(){
		var forward = false;
		var backward = false;
		var left = false;
		var right = false;
		var vertical = false;
		
		if (TP_Motor.Instance.MoveVector.z > 0)
			forward = true;
		if (TP_Motor.Instance.MoveVector.z < 0)
			backward = true;
		if (TP_Motor.Instance.MoveVector.x > 0)
			right = true;
		if (TP_Motor.Instance.MoveVector.x < 0)
			left = true;
		if (TP_Motor.Instance.MoveVector.y != 0)
			vertical = true;
		
		if(forward){
			if (left)
				MoveDirection = Direction.LeftForward;
			else if (right)
				MoveDirection = Direction.RightForward;
			else {
				MoveDirection = Direction.Forward;	
			}
		}
		else 
		if (backward)	
		{
			if (left)
				MoveDirection = Direction.LeftBackward;
			else if (right)
				MoveDirection = Direction.Backward;
			else {
				MoveDirection = Direction.Backward;	
			}
		}
		else if(left){
			MoveDirection = Direction.Left;
		}
		else if(right){
			MoveDirection = Direction.Right;
		}
		else{
		MoveDirection = Direction.Stationary;	
		}
		if(vertical){
			MoveDirection = Direction.Vertical;
		}

	}
}


