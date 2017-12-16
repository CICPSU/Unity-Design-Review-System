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
    
    public Animator avatarAnimator;
	
	
	public Vector3 MoveVector {get; set; }
	
	public float VerticalVelocity {get; set;}

    private float jumpStartTime = 0;
    public bool hasJumped = false;
    public bool stopRotation = false;

	void Awake () {
		if(Instance == null)
			Instance = this;
		gravityOn = true;
	}
	
    void Update()
    {


        if (avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && Time.time > jumpStartTime + 1.3f)
            avatarAnimator.SetBool("Jump", false);
        
    }

	public void UpdateMotor () {
        if (!TP_Animator.Instance.avatarAnimator.GetBool("Sitting") && !stopRotation)
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

        // Multiply MoveVector by MoveSpeed;
        MoveVector = MoveVector * MoveSpeed();
        
		if(gravityOn)
        {
			// Reapply VerticalVelocity MoveVector, as it should continue from previous frame but is erased by
			//TPController. It is the falling speed.
			MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);
			// Apply gravity
			ApplyGravity();
		}
		
		// Move the character in world space
        TP_Controller.characterController.Move (MoveVector * Time.deltaTime);
	}


	void ApplyGravity()
    {
		if(MoveVector.y > - TerminalVelocity)
			MoveVector = new Vector3(MoveVector.x,MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);
		if (TP_Controller.characterController.isGrounded && MoveVector.y < -1)  // if the character is on the ground, we want to keep its y speed to a small number. 
																				// otherwise when it falls, it will start with the TerminalVelocity
			MoveVector = new Vector3(MoveVector.x,-1, MoveVector.z);
	}
	
    public void CheckJump()
    {
        if (avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && Time.time > jumpStartTime + .66f && !hasJumped)
        {
            hasJumped = true;
            VerticalVelocity = JumpSpeed;
        }

    }
	
	public void Jump()
    {
        if (TP_Controller.characterController.isGrounded)
        {
            hasJumped = false;
            avatarAnimator.SetBool("Jump", true);
            jumpStartTime = Time.time;
        }
	}
	
	void SnapAlignCharacterWithCamera()
    {
//		if(MoveVector.x != 0 || MoveVector.z != 0){    // comment this out is to make the capsule follows the camera direction all the time
//****************************!!!!!!!!!!!!!!***************** If ()added by me
		if(Input.GetMouseButton(1)|| Input.GetMouseButton(2)){  
//		This line is added by me
//			TP_Camera.Instance.SnapCamera();   // this allows the camera to smooth back to the back of the character
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
				virtualCamera.eulerAngles.y, transform.eulerAngles.z);
		}

		
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
		
		
		return moveSpeed;
	}
}
