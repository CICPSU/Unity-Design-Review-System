using UnityEngine;
using System;
using System.Collections;

public class TP_Controller : MonoBehaviour {
	//store prefabs of ICon lab cameras
//	public GameObject LeftCamera;
//	public GameObject RightCamera;	

	public static CharacterController characterController;
	public static TP_Controller Instance;
    public bool allowPlayerInput = true;
	void Awake () {
		if(characterController == null)
			characterController = GetComponent("CharacterController") as CharacterController;
		if(Instance == null)
			Instance = this;
		
	//	TP_Camera.UseExistingOrCreateNewMainCamera();
	}

	void Update () {
        if (!TP_Animator.Instance.avatarAnimator.GetBool("Sitting") && allowPlayerInput)
        {
            GetLocomontionInput();
            HandleActionInput();
            TP_Motor.Instance.UpdateMotor();
        }
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
			Vector3 moveDirection = transform.InverseTransformDirection(DIRE.Instance.Hand.transform.forward);
			//moveDirection = moveDirection.normalized;
			//Debug.Log("Move Direction: " + moveDirection);
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
	
	void HandleActionInput()
    {
        TP_Motor.Instance.CheckJump();

		if(Input.GetButton("Jump"))
        {// It seems redundent to call a Jump() here which calls the Jump() in TP_Motor
			Jump();
		}// the reason of doing this is to create space for future functions, such as climb and animations	
	}
	
	void Jump(){
		TP_Motor.Instance.Jump();	
	}
	
	public void ToggleCharacterCollisionBasedOnGravity(){
        if (TP_Motor.Instance.gravityOn)
        {
            TurnCharacterCollisionOff();
        }
        else
        {
            TurnCharacterCollisionOn();
        }
    }

    public void TurnCharacterCollisionOn()
    {
        characterController.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;

        TP_Motor.Instance.gravityOn = true;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), LayerMask.NameToLayer("Default"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), LayerMask.NameToLayer("Characters"), false);

        for (int i = 8; i < 15; i++)
        {
            if (!String.IsNullOrEmpty(LayerMask.LayerToName(i)))
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), i, false);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), i, false);
            }
        }
    }

    public void TurnCharacterCollisionOff()
    {
        characterController.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        TP_Motor.Instance.gravityOn = false;
        //as models will by default be loaded in "Default" layer, 
        //disable the collision between "avatar", where player is in, and "default"
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), LayerMask.NameToLayer("Default"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), LayerMask.NameToLayer("Characters"), true);

        //Users may add layers from layer 8 to layer 31
        //For efficiency, we can safely assume that users usually won't create more than 7 layers
        // therefore we only check "ignore raycast layer" against the first 7 user defined layers
        for (int i = 8; i < 15; i++)
        {
            if (!String.IsNullOrEmpty(LayerMask.LayerToName(i)))
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), i, true);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Avatar"), i, true);
            }
        }
    }
}
