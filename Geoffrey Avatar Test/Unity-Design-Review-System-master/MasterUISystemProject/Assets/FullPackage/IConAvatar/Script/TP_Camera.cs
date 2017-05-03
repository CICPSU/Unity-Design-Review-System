using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class TP_Camera : MonoBehaviour {
	public static TP_Camera Instance;
	
	public Transform TargetLookAt;
	public Transform cameraDistanceCheck;
    public GameObject avatarMesh;
    public bool stopRotation = false;
	public float Distance = 0.3f;   // distance from the camera to the capsule(TargetLookAt)
								    // was 5f
	public float DistanceMin = 0.3f;    // near limit
	public float DistanceMax = 10f;     //far limit
	public float DistanceSmooth = 0.05f;    //Transition time in seconds
	public float DistanceResumeSmooth = 1f;
	
	public float X_MouseSensitivity = 5f;
	public float Y_MouseSensitivity = 5f;
    
	public float MouseWheelSensitivity = 2.5f;	
	public float X_Smooth = 0.05f;
	public float Y_Smooth = 0.1f;
	public float Y_MinLimit = -40f;
	public float Y_MaxLimit = 80f;
	public float OcclusionDistanceStep = 0.5f;
	public int MaxOcclusionChecks = 1;
    public bool iconLabCam = false;

    //used to control the smoothDamn of camera snap
	public float XSnapSmooth = 100f;
	public float YSnapSmooth = 1.5f;
	private float velXSnap = 0f;
	private float velYSnap = 0f;
	private float velZSnap = 0f;

	
	private float mouseX = 0f;  // rotation around Y axis, 0 is right behind the avatar
	private float mouseY = 0f;  // rotation around X axis
	
	private float velX = 0f;
	private float velY = 0f;
	private float velZ = 0f;
	private float velDistance = 0f; //speed along the smoothing curve
	private  float startDistance = 0f;  //validated start distance
	private Vector3 position = Vector3.zero;
	public float desiredDistance = 0f;  //distance you want to move to
	public Vector3 desiredPosition = Vector3.zero;
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0f; // this will store the distance value very time we move the mousewheel
	private float showSpeedGUITime;
	
	private bool showSpeed = false;
	
	private int countRightClick = 0;    // tell if it is the first time right mouse is clicked. 
	
	private Rect showSpeedRect;
	private GUIStyle style;
		
	void Awake ()
    {
        if(Instance == null)
		    Instance = this;
	}
	
	void Start()
    {
		showSpeedRect= new Rect (Screen.width/2, Screen.height - 20, 120, 20);
		style = new GUIStyle();
		style.fontSize = 15;
		Distance = Mathf.Clamp (Distance, DistanceMin, DistanceMax);
		startDistance = Distance;
		Reset();
	}
		
	void LateUpdate () {
		if (TargetLookAt == null)
			return;

        if(!stopRotation)
            HandlePlayerInput();
        checkCameraCharacterDistance(desiredPosition, cameraDistanceCheck.position);

        CalculateDesiredPosition();
        
        UpdatePosition();
        
	}
	
	void HandlePlayerInput(){
//		Debug.Log("Cliked before function");
		float deadZone = 0.01f;  // tutorial writes it as "var deadZone = 0.1f;"
// ********************!!!!!!!!!!!!!!!!!!!******** GetMouseButton(0) 	
		if(Input.GetMouseButton(1) || Input.GetMouseButton(2)){      // GetMouseButton(1) is right mouse, 2 is scrollWheel, 0 is leftMouse. 
											//It returns true if the right mouse is clicked
			// The RMB (Right Mouse Button)	 is down get mouse Axis input
			 // Input.GetAxis("Horizontal") gets the keyboard input. GetAxis("Mouse X") gets the mouse input
			
//			Debug.Log("Clicked in the function");
			mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
			mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
//			Debug.Log("mouse move" + Input.GetAxis("Mouse X") * X_MouseSensitivity);
		}

        //*******!!!!!!!!*********** allow pressing a,d to rotate the charater.	
        /// avatarRotation less than 0 rotate left, greater than 0 rotate right
        TP_Animator.Instance.avatarRotation = 0f;	
		if(Input.GetKey(TP_InputManager.instance.rotateRight) || Input.GetKey(TP_InputManager.instance.rotateLeft)){	
			if(Input.GetKey (TP_InputManager.instance.rotateRight)){
                TP_Animator.Instance.avatarRotation = 90f;
				mouseX += TP_InputManager.instance.rotateKeySensitivity;
			}
			if(Input.GetKey (TP_InputManager.instance.rotateLeft)){
				mouseX -= TP_InputManager.instance.rotateKeySensitivity;
                TP_Animator.Instance.avatarRotation = -90f;
			}
            //		Debug.Log ("Rotate: " + Input.GetAxis ("Rotate"));
            if (!TP_Animator.Instance.avatarAnimator.GetBool("Sitting"))
                TP_Motor.Instance.SnapCharaterWithCamera_Key();
		}

		if(Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl)){
			mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
			mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
			if(mouseX != 0){
				countRightClick = 0;	
			}
		}
	/*	
		if(Input.GetAxis("JoystickTrigger") != 0){
			mouseX += Input.GetAxis("JoystickLookX") * X_MouseSensitivity;
			mouseY -= Input.GetAxis("JoystickLookY") * Y_MouseSensitivity;
			if(mouseX != 0){
				countRightClick = 0;	
			}
		}
	*/
		
		// This is where we will limit mouseY, mouseY will be limited between Y_MinLimit and Y_MaxLimit
		mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);
		
        /// EventSystem.current.IsPointerOverGameObject returns true if the mouse is over a UI element.  This is used to prevent the scrollwheel from zooming the camera in and out when over a menu.
		if((Input.GetAxis("Mouse ScrollWheel") < - deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone) && !EventSystem.current.IsPointerOverGameObject()){
			desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel")* MouseWheelSensitivity, DistanceMin, DistanceMax);
			preOccludedDistance = desiredDistance;
			distanceSmooth = DistanceSmooth;
		}
		
/*		legacy code before DIRE
		//Toggle camera between ICon lab camera and single camera
		if(Input.GetKeyUp(TP_InputManager.instance.toggleCamera)){
			if(iconLabCam){
				iconLabCam = false;
				//GameObject[] children = new GameObject[2];
				transform.Find("Left Camera").gameObject.SetActive(false);
				transform.Find("Right Camera").gameObject.SetActive(false);
				gameObject.GetComponent<Camera>().rect = new Rect (0,0,1,1);
				gameObject.GetComponent<Camera>().fieldOfView = 60;
			}
			else{
				iconLabCam = true;
				transform.Find("Left Camera").gameObject.SetActive(true);
				transform.Find("Right Camera").gameObject.SetActive(true);
				gameObject.GetComponent<Camera>().rect = new Rect(0.3333334f, 0,0.3333334f, 1);
				gameObject.GetComponent<Camera>().fieldOfView = 34;
			}
		}
		*/
		
		//Toggle gravity with G
		if(Input.GetKeyUp(TP_InputManager.instance.gravity)){
            //	Debug.Log("G pressed");
            TP_Controller.Instance.ToggleCharacterCollisionBasedOnGravity();
		}
		
		//check speed adjustment, if the speed is within limit, adjust speed.
		if(TP_Motor.Instance.ForwardSpeed <= 5f){
			if(Input.GetKeyUp(TP_InputManager.instance.increaseSpeed)){
				showSpeed = true;
				TP_Motor.Instance.ForwardSpeed += 0.5f;
				TP_Motor.Instance.BackwardSpeed += 0.5f;
				showSpeedGUITime = Time.time;
			}
		}
		
		if(TP_Motor.Instance.ForwardSpeed > 0f){
			if(Input.GetKeyUp (TP_InputManager.instance.decreaseSpeed)){
				showSpeed = true;
				TP_Motor.Instance.ForwardSpeed -= 0.5f;	
				TP_Motor.Instance.BackwardSpeed -= 0.5f;
				showSpeedGUITime = Time.time;
			}
		}



	}
	
	
	//all the smoothing to the distance happens here
	void CalculateDesiredPosition(){
		//Evaluate distance
		//ResetDesiredDistance(); // check if it is still occluded or not.
		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth * Time.timeScale); // (video No. 19). Google Mathf.SmoothDamp. VelDistance is the speed along the 
																									//smoothing curve
		// Calculate desired position
		//freeze mouseY when tracking is active ==>as we don't want to tilt when tracking is active
		if(!DIRE.Instance.trackingActive){
			desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
		}else{ //force camera to look horizontal and prevent it from tilting
			this.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
			desiredPosition = CalculatePosition(0, mouseX, Distance);
		}
	}
	
	Vector3 CalculatePosition(float rotationX, float rotationY, float distanceIn){
		/*****!!!!! To adjust the camera starting rotation, change the direction vector to point to the starting position !!!!!******/
		Vector3 direction = new Vector3(0, 0, -distanceIn);  // we want the camera to be behind the avatar
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
		//return the desired position
		return TargetLookAt.position +rotation * direction; // vector multiplied by a quaternion returns a vector, which is the new location after applying the rotation
	}
	
	void UpdatePosition(){
		var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth * Time.timeScale);
		var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth * Time.timeScale);
		var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth * Time.timeScale);

            position = new Vector3(posX, posY, posZ);
            transform.position = position;
            transform.LookAt(TargetLookAt);
	}
	
	public void Reset(){
		mouseX = 0;  // the tutorial does not have f as float
		mouseY = 10;
		Distance = startDistance;
		desiredDistance = Distance;
		preOccludedDistance = Distance;
	}


	//This method switches the camera to first person view
	// shoot a ray from camera to cameraDistanceCheck(a child of the Character.). adjust the visibility of the cameraDistanceCheck based
	// on the distance from camera to the character.
	public void checkCameraCharacterDistance(Vector3 from, Vector3 to){
		/*
		//PRE: from is the transform.position of the camera
		//		to is the transfoorm.position of the cameraDistanceCheck
		//POST: change the transparency and visibility of the cameraDistanceCheck and its children
		RaycastHit hitInfo;
		//Debug.Log("in the function");
		Debug.DrawLine(from, to, Color.green);
		if(Physics.Linecast(from, to, out hitInfo)){
			
			if (hitInfo.distance < .8 && hitInfo.collider.tag != "Player"){
				cameraDistanceCheck.parent.Find("f020_hipoly_81_bones_opacity_C").GetComponent<Renderer>().enabled = false;
			}
			if(hitInfo.distance > .9 && hitInfo.collider.tag != "Player"){
				cameraDistanceCheck.parent.Find("f020_hipoly_81_bones_opacity_C").GetComponent<Renderer>().enabled = true;
			}
		}
		//	Debug.Log(hitInfo.collider.tag);
*/
		Vector3 lynnCameraVector = transform.parent.FindChild("targetLookAt").position - desiredPosition;
//		Debug.Log("desired position " + desiredPosition);
//		Debug.Log("lynn camera vector" + lynnCameraVector);
		if(lynnCameraVector.magnitude < 0.8f)
		{
            //cameraDistanceCheck.parent.Find("f020_hipoly_81_bones_opacity_C").GetComponent<Renderer>().enabled = false;
            //cameraDistanceCheck.parent.FindChild("AvatarMesh").gameObject.SetActive(false);
            avatarMesh.SetActive(false);
		}
		else
		{
            //cameraDistanceCheck.parent.Find("f020_hipoly_81_bones_opacity_C").GetComponent<Renderer>().enabled = true;
            //cameraDistanceCheck.parent.FindChild("AvatarMesh").gameObject.SetActive(true);
            avatarMesh.SetActive(true);
		}

	}	



// show current speed for 3 seconds when speed is adjusted.
	void OnGUI(){
		if(showSpeed){
			if(Time.time - showSpeedGUITime > 3.0f){
				showSpeed = false;
			}
			GUI.Label (showSpeedRect, "Current Speed: " + TP_Motor.Instance.ForwardSpeed, style);
			//GUI.Label(Rect(10, 10, 100, 20), "Hello World!");
			//GUI.Label(Rect(100, 20, 80, 20), "Current Speed: ");
			
		}
	}

#region obselete code


	/*
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 

	//called by controller. NOT USED IN DIRE
	// Make sure a camera is always in the scene, and add an instance of TP_Camera Script to the mainCamera
	public static void UseExistingOrCreateNewMainCamera(){  //This function is called by TP Controller, TP camera does not call this function
		GameObject tempCamera;
		GameObject targetLookAt;
		GameObject cameraDistanceCheckObject;
		
		//instantiate two left and right cameras for the ICon lab camera
		//	GameObject leftCam = new GameObject("LeftCamera");
		GameObject leftCam = Instantiate(TP_Controller.Instance.LeftCamera.gameObject) as GameObject;
		//	GameObject rightCam = new GameObject("RightCamera");
		GameObject rightCam = Instantiate(TP_Controller.Instance.RightCamera.gameObject) as GameObject;
		
		TP_Camera myCamera;
		
		if(Camera.mainCamera != null){
			tempCamera = Camera.mainCamera.gameObject;
			leftCam.transform.parent = tempCamera.transform;
			leftCam.transform.localPosition = new Vector3(0,0,0);
			leftCam.transform.localRotation = Quaternion.Euler (0,315.6443f,0);
			rightCam.transform.parent = tempCamera.transform;
			rightCam.transform.localPosition = new Vector3(0,0,0);
			rightCam.transform.localRotation = Quaternion.Euler (0,44.355564f,0);
			leftCam.SetActive(false);
			rightCam.SetActive(false);
		}
		else {
			tempCamera = new GameObject("Main Camera");	
			tempCamera.AddComponent("Camera"); // this is to make the tempCamera object a camera by giving it a camera component
			tempCamera.AddComponent ("Flare Layer");
			tempCamera.AddComponent ("GUILayer");
			tempCamera.AddComponent ("Audio listenr");
			tempCamera.tag = "MainCamera";
			leftCam.transform.parent = tempCamera.transform;
			leftCam.transform.localPosition = new Vector3(0,0,0);
			leftCam.transform.localRotation = Quaternion.Euler (0,315.6443f,0);
			rightCam.transform.parent = tempCamera.transform;
			rightCam.transform.localPosition = new Vector3(0,0,0);
			rightCam.transform.localRotation = Quaternion.Euler (0,44.355564f,0);
			leftCam.SetActive(false);
			rightCam.SetActive(false);
		}
		tempCamera.AddComponent("TP_Camera"); // This is to attach the TP_Camera script to the camera
		
		myCamera = tempCamera.GetComponent("TP_Camera") as TP_Camera; // find a reference for the just added TP Camera Component
		
		targetLookAt = GameObject.Find("targetLookAt") as GameObject;
		
		cameraDistanceCheckObject = GameObject.Find("CameraDistanceCheck") as GameObject;
		// if the targetLookAt does not exist, create one
		if(targetLookAt == null){
			targetLookAt = new GameObject("targetLookAt");	
			targetLookAt.transform.position = Vector3.zero;  // set its location to world origin
		}
		myCamera.TargetLookAt = targetLookAt.transform;
		myCamera.cameraDistanceCheck = cameraDistanceCheckObject.transform;
	}


// adjust the position of the camera if occluded
	bool CheckIfOccluded(int count){
		var isOccluded = false;
		
		// Check what the nearest distance the camera should go to if it is occluded.
		var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);
		
		if(nearestDistance != -1){
			if (count < MaxOcclusionChecks){
				isOccluded = true;
				Distance -= OcclusionDistanceStep; // this is incrementally pull the camera close
				
				
				//to prevent the camera from getting too close. The number comes from experience. Closer than that, the smoothing will not work properly
				// you can test it and set appropriate number.
///***** To switch to a FPS, put the camera to the TargetLookAt and fade the avatar out.  ********
				if(Distance < 0.25f)
					Distance = 0.25f;
					
			}
			else {
			Distance = nearestDistance 	- Camera.mainCamera.nearClipPlane;
			desiredDistance = Distance;
			distanceSmooth = DistanceResumeSmooth;  // from second from the last video
			}
		}
			
		return isOccluded;
	}


	void ResetDesiredDistance(){
		if(desiredDistance < preOccludedDistance){
			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
			var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);
			
			if(nearestDistance == -1 || nearestDistance > preOccludedDistance){
				desiredDistance = preOccludedDistance;	
			}
		}
	}

// returns a nearestDistance that the camera should go to in order to not being occluded
	float CheckCameraPoints(Vector3 from, Vector3 to){
		var nearestDistance = -1f; // arbitary number used to test if any ray hit anything, the nearestDistance is -1 when not hitting anything
			
		RaycastHit hitInfo;
		
		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);
		
		//Draw lines in the editor to make it easier to visualize
		
	//	Debug.DrawLine(from, to + transform.forward * -camera.nearClipPlane, Color.red);		
		Debug.DrawLine(from, clipPlanePoints.UpperLeft);
		Debug.DrawLine(from, clipPlanePoints.LowerLeft);
		Debug.DrawLine(from, clipPlanePoints.UpperRight);
		Debug.DrawLine(from, clipPlanePoints.LowerRight);
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);
		
		
		//use a line cast to determine the nearest distance. Linecast has start and end point, but raycast doesn't have end points.
		if(Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")    // out hitInfo means the output of the function go into hitInfo
			nearestDistance = hitInfo.distance;
		if(Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player"){    // out hitInfo means the output of the function go into hitInfo
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;
		}
		if(Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player"){    // out hitInfo means the output of the function go into hitInfo
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;
		}
		if(Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player"){    // out hitInfo means the output of the function go into hitInfo
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance;
		}
		
		// In the tutorial, the "To" is "to + transform.forward * camera.nearClipPlane"
		if(Physics.Linecast(from, to + transform.forward * camera.nearClipPlane , out hitInfo) && hitInfo.collider.tag != "Player"){    // out hitInfo means the output of the function go into hitInfo
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			nearestDistance = hitInfo.distance - 3.0f;
		}
		
		return nearestDistance;
	}
	*/
#endregion
}