 using UnityEngine;
using System.Collections;


public class TP_Animator: MonoBehaviour
{
	public enum  Direction{
		Stationary, Forward, Backward, Left, Right, 
		LeftForward, RightForward, LeftBackward, RightBackward, Vertical
	}
	
	
	public static TP_Animator Instance;  // This is just to create a reference to this class

    public float avatarRotation;

	public Direction MoveDirection {get; set;}

	private Animator avatarAnimator;

	void Awake(){						//   so when calling the variables or methods of this class from other script,
	Instance = this;					//    we can refer to this instance.
		avatarAnimator = GetComponentInChildren<Animator> ();
	}
	void Start(){
		
	}
	
	void Update(){
		
	}
	
	public void DetermineCurrentMoveDirection(){

        //x is rotation right, y is rotation left
        avatarAnimator.SetFloat("Direction", avatarRotation);

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
			//avatarAnimator.SetFloat("Direction", 0.0f);
            avatarAnimator.SetFloat("Speed", TP_Motor.Instance.ForwardSpeed);
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
            avatarAnimator.SetFloat("Speed", -1f * TP_Motor.Instance.BackwardSpeed);
			if (left)
				MoveDirection = Direction.LeftBackward;
			else if (right)
				MoveDirection = Direction.RightBackward;
			else {
				MoveDirection = Direction.Backward;	
			}
		}
		else if(left){
			MoveDirection = Direction.Left;
			avatarAnimator.SetFloat("Strafe", -1.0f);
		}
		else if(right){
			MoveDirection = Direction.Right;
			avatarAnimator.SetFloat("Strafe", 1.0f);
		}
		else{
    		MoveDirection = Direction.Stationary;
            avatarAnimator.SetFloat("Speed", 0.0f);
		}
		if(vertical){
			MoveDirection = Direction.Vertical;
		}

	}
}


