using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour {

	public static TP_Motor Instance;
	
	public float ForwardSpeed = 3f;
	public float BackwardSpeed = 3f;
	public float VerticalSpeed = 2f; // the speed of acending and descending
	public float StrafingSpeed = 3f;
	public float SlideSpeed = 1.5f;
	public float JumpSpeed = 6f;
	public float Gravity = 21f;
	public float TerminalVelocity = 20f;
	public float SlideThreshold = 0.6f; // the bigger, the easiler to slide
	public float MaxControllableSlideMagnitude = 0.4f;
	public bool gravityOn; 
	public Transform virtualCamera;
	private Vector3 slideDirection;

	
	
	public Vector3 MoveVector {get; set; }
	
	public float VerticalVelocity {get; set;}
		
	void Awake () {
			
		Instance = this;
		gravityOn = true;
	}
	

	public void UpdateMotor () {
		SnapAlignCharacterWithCamera();
		ProcessMotion();
	}
	
	
	//This method process the MoveVector and decide how the character moves
	
	void ProcessMotion(){
		//transform MoveVector to World Space 
		MoveVector = transform.TransformDirection (MoveVector);
		
		// Normalize MoveVector if Magnitude > 1;
		if (MoveVector.magnitude > 1)
			MoveVector = Vector3.Normalize(MoveVector);
		//Apply sliding if applicable
		ApplySlide();
		
		//play animation
		playAnimation();
		
		// Multiply MoveVector by MoveSpeed;
		MoveVector = MoveVector * MoveSpeed();

		if(gravityOn){
			// Reapply VerticalVelocity MoveVector, as it should continue from previous frame but is erased by
			//TPController. It is the falling speed.
			MoveVector = new Vector3(MoveVector.x,VerticalVelocity, MoveVector.z);
			// Apply gravity
			ApplyGravity();
		}
		
		// Move the character in world space
		TP_Controller.characterController.Move (MoveVector * Time.deltaTime);
	}
	

	//this method plays walk/ idle animation 
	void playAnimation(){
		if(MoveVector.magnitude > 0.1f){
			if(!GetComponent<Animation>().IsPlaying("f_walk_neutral_04_inplace")){
			GetComponent<Animation>().Play("f_walk_neutral_04_inplace");	
			}
		}
		if(MoveVector.magnitude == 0){
			if(!GetComponent<Animation>().IsPlaying("f_idle_neutral_04") && !GetComponent<Animation>().IsPlaying("f_gestic_talk_neutral_02")){
			GetComponent<Animation>().Play("f_idle_neutral_04");	
			}
		}
		if(Input.GetKeyUp(KeyCode.T)){
			if(!GetComponent<Animation>().IsPlaying("f_gestic_talk_neutral_02")){
			GetComponent<Animation>().Play("f_gestic_talk_neutral_02");	
			
			}
		}
	}
	
	
	void ApplyGravity(){
		if(MoveVector.y > - TerminalVelocity)
			MoveVector = new Vector3(MoveVector.x,MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);
		if (TP_Controller.characterController.isGrounded && MoveVector.y < -1)  // if the character is on the ground, we want to keep its y speed to a small number. 
																				// otherwise when it falls, it will start with the TerminalVelocity
			MoveVector = new Vector3(MoveVector.x,-1, MoveVector.z);
	}
	
	void ApplySlide(){
		if(!TP_Controller.characterController.isGrounded)	
			return;
		slideDirection = new Vector3(0, 0, 0); // in tutorial it is = Vector3.zero    ***********************************!!!!!!!!***********
		
		RaycastHit hitInfo; // RaycastHit is a struct that holds all the information that ray casting returns
		
		if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo)){
			if (hitInfo.normal.y < SlideThreshold)
				slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z); 
		}
		
		if(slideDirection.magnitude < MaxControllableSlideMagnitude)
			MoveVector +=slideDirection;  // the input still has control on the movement
		else{
			MoveVector = slideDirection; // player input is overwirted by the slideDirection, player loses control	
		}
	}
	
	public void Jump(){
		if(TP_Controller.characterController.isGrounded)	
			VerticalVelocity = JumpSpeed;
	}
	
	void SnapAlignCharacterWithCamera(){
//		if(MoveVector.x != 0 || MoveVector.z != 0){    // comment this out is to make the capsule follows the camera direction all the time
//****************************!!!!!!!!!!!!!!***************** If ()added by me
		if(Input.GetMouseButton(1)|| Input.GetMouseButton(2)){  
//		This line is added by me
//			TP_Camera.Instance.SnapCamera();   // this allows the camera to smooth back to the back of the character
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
				virtualCamera.eulerAngles.y, transform.eulerAngles.z);
		}
/*		
		//This is to check if joystick right trigger is pressed or not
		if(Input.GetAxis("JoystickTrigger") < 0 ){  
//		This line is added by me
//			TP_Camera.Instance.SnapCamera();   // this allows the camera to smooth back to the back of the character
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
		Camera.mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
		}
		*/
		
	}
	
	//This function is used to snap the charater align with the camera when pressing the Q,E keys
	public void SnapCharaterWithCamera_Key(){
		transform.rotation = Quaternion.Euler (transform.eulerAngles.x, virtualCamera.eulerAngles.y, transform.eulerAngles.z);
	}

	
	// MoveSpeed allows applying different speed to movement of different direction
	//It takes in move direction from TP_Animator.
	float MoveSpeed(){
		var moveSpeed = 0f;
		
		switch (TP_Animator.Instance.MoveDirection){
		case TP_Animator.Direction.Stationary:
				moveSpeed = 0;
			break;
		case TP_Animator.Direction.Forward:
				moveSpeed = ForwardSpeed;
			break;
		case TP_Animator.Direction.Backward:
				moveSpeed = BackwardSpeed;
			break;
		case TP_Animator.Direction.Left:
				moveSpeed = StrafingSpeed;
			break;
		case TP_Animator.Direction.Right:
				moveSpeed = StrafingSpeed;
			break;
		case TP_Animator.Direction.LeftForward:
				moveSpeed = ForwardSpeed;
			break;
		case TP_Animator.Direction.RightForward:
				moveSpeed = ForwardSpeed;
			break;
		case TP_Animator.Direction.LeftBackward:
				moveSpeed = BackwardSpeed;
			break;
		case TP_Animator.Direction.RightBackward:
				moveSpeed = BackwardSpeed;
			break; 
		case TP_Animator.Direction.Vertical:
				moveSpeed = VerticalSpeed;
			break;
		}
		
		if (slideDirection.magnitude > 0 ) // when we implement the move direction, we set the move speed to 0 when the move state is stationary, so we won't slide if we 
										// stand still on a slope
			moveSpeed = SlideSpeed;
		return moveSpeed;
	}
}
