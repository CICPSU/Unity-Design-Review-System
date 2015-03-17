using UnityEngine;
using System.Collections;

public class TP_Controller : MonoBehaviour {
	//store prefabs of ICon lab cameras
//	public GameObject LeftCamera;
//	public GameObject RightCamera;	

	public static CharacterController characterController;
	public static TP_Controller Instance;
	void Awake () {
		characterController = GetComponent("CharacterController") as CharacterController;
		Instance = this;
		
	//	TP_Camera.UseExistingOrCreateNewMainCamera();
	}

	void Update () {		
		GetLocomontionInput();
		HandleActionInput();
		TP_Motor.Instance.UpdateMotor();
	}
	
	void GetLocomontionInput(){
		var deadZone = 0.1f;
		
		//store the verticalSpeed(falling speed) before zero out the moveVector.
		// so that during falling, the vertical speed will accelerate continuously
		TP_Motor.Instance.VerticalVelocity = TP_Motor.Instance.MoveVector.y;
		
		TP_Motor.Instance.MoveVector = Vector3.zero; // zero out move vector
		
		//		if(Input.GetAxis("Vertical") > deadZone|| Input.GetAxis("Vertical") < -deadZone )
		if(Input.GetKey (TP_InputManager.instance.forward))
			TP_Motor.Instance.MoveVector += new Vector3 (0, 0, 1);
		if(Input.GetKey (TP_InputManager.instance.backward))
			TP_Motor.Instance.MoveVector += new Vector3(0,0,-1);
		//if(Input.GetAxis("Horizontal") > deadZone|| Input.GetAxis("Horizontal") < -deadZone )
		if(Input.GetKey (TP_InputManager.instance.leftward))
			TP_Motor.Instance.MoveVector += new Vector3 (-1, 0, 0 );
		if(Input.GetKey (TP_InputManager.instance.rightward))
			TP_Motor.Instance.MoveVector += new Vector3 (1, 0, 0 );
		// the difference between GetMouseButton and GetMouseButtonDown is GetMouseButton checks if the button is at the state of being pressed
		// GetMouseButtonDown checks if the button is clicked at that frame, so you cannot hold the button with this command
		if(Input.GetMouseButton(2))
			TP_Motor.Instance.MoveVector += new Vector3(0,0,1);

		//when tracking xbox controller, press A to move to the direction of pointing
		if(Input.GetAxis("XboxPointGo") == 1){
			Vector3 moveDirection = new Vector3(DIRE.Instance.Hand.transform.forward.x, 0, DIRE.Instance.Hand.transform.forward.z);
			moveDirection = moveDirection.normalized;
			TP_Motor.Instance.MoveVector += moveDirection;
			
		}

		if(!TP_Motor.Instance.gravityOn){
			if(Input.GetKey(TP_InputManager.instance.elevate)){
				TP_Motor.Instance.MoveVector += new Vector3(0,1,0);
			//	Debug.Log("up pressed");
		}
			if(Input.GetKey(TP_InputManager.instance.descend)){
				TP_Motor.Instance.MoveVector += new Vector3 (0, -1, 0);
			//	Debug.Log("down pressed");
			}
		}


		TP_Animator.Instance.DetermineCurrentMoveDirection();
		
	}
	
	void HandleActionInput(){
			if(Input.GetButton("Jump")){     // It seems redundent to call a Jump() here which calls the Jump() in TP_Motor
			Jump();
		}									// the reason of doing this is to create space for future functions, such as climb and animations	
	}
	
	void Jump(){
		TP_Motor.Instance.Jump();	
	}
	
	void ToggleCharacterCollisionBasedOnGravity(){
			
	}
}
