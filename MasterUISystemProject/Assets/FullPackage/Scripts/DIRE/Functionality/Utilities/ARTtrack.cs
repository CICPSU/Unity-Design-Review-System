using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

//known limitation with current implementation. When use hand tracking only, your height is going to be 1.4m instead of Lynn's height.

//read and parse the tracking data directly from a network port
public class ARTtrack : MonoBehaviour {
	UdpClient client;
	IPEndPoint source;

	private Vector3 head_position_vector, head_rotation_vector, hand_position_vector, hand_rotation_vector;

	private string[] head_positions_rotations = new string[6];
	private string[] hand_positions_rotations = new string[6];

	public void InitializeTracking()
	{
		source = new IPEndPoint( IPAddress.Any, 5000);
		client = new UdpClient();
		client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		client.Client.Bind(source);
	}

	public bool CheckTracking(){


		float timeThresold =  Time.realtimeSinceStartup + .1f;
		while(Time.realtimeSinceStartup < timeThresold){
			//wait for data to get to client
		}

		return (client.Available > 0);
	}

	// Use this for initialization
	void Start () 
	{
		head_position_vector = Vector3.zero;
		head_rotation_vector = Vector3.zero;
		//data = client.Receive( ref source );
	}

	//A GUI button calls this
	//Param: true to turn on 
	public void SetTracking (bool turnOn){
		DIRE.Instance.trackingActive = turnOn && CheckTracking ();
		//reset the head position when turnning tracking off
		if(!DIRE.Instance.trackingActive){
			transform.localPosition = Vector3.zero;
            //GetComponent<DisplaySetup>().offsetDisplayOriginByGeometricCenter();
            //GetComponent<DisplaySetup>().offsetHeadToGeometricCenter();
            GetComponent<DIRE>().offsetDisplayOriginByGeometricCenter();
            GetComponent<DIRE>().offsetHeadToGeometricCenter();
		}
	}


	void Update () {
		try{
			//check if tracking data exist
			if(DIRE.Instance.trackingActive){
				if(client == null) {
					DIRE.Instance.trackingActive = false;
					throw new Exception("client reference is not set to an instance!");
				}
				if(source ==null) {
					DIRE.Instance.trackingActive = false;
					throw new Exception("source reference is not set to an instance!");
				}

				track();
			}
		}catch(Exception ex){
			Debug.Log(ex.Message);
		}
	}
	

	void track(){
		byte[] data = client.Receive(ref source);
		//flush the port by reading through all available data till the most recent one
		while(client != null && client.Available > 0){
			data = client.Receive( ref source );
		}
		
		string text = System.Text.Encoding.ASCII.GetString(data);	
		parse (text);
		setTransforms();
	}

	//parse needs cleanedup based on the use case in the icon lab.  
	// this function wont be necessary once cluster input is added to the release of unity 5 and the icon lab can use the input map from the arl
	void parse(string text){
		int index = 0;

		index = text.IndexOf("6d ");

		index +=3; // update index to the position of the number of bodies

		int number_of_bodies = text[index] - '0';

		if (number_of_bodies > 0){

			index +=3; // update index to the position of the first body ID

			int current_body_id = text[index] - '0'; // the id of the track body
												//xbox controller id = 1, head tracking id = 0
			head_positions_rotations = new string[6];
			hand_positions_rotations = new string[6];

			index = text.IndexOf("[", index) + 1; // update index to the position of the first numerical data for the body

			//when there is only one body visible in tracking
			if(number_of_bodies == 1){
				//head visible
				if(current_body_id == 0)
				{
					//read in the first body we are tracking -- head in this case
					int space_index = 0;
					int i = 0;
					for (i = 0; i <5; i++){
						space_index = text.IndexOf(" ", index);
						head_positions_rotations[i] = text.Substring(index, space_index - index);
						index = space_index + 1;
					}
					space_index = text.IndexOf("]", index);
					head_positions_rotations[5] = text.Substring(index, space_index - index);

					head_position_vector = new Vector3(float.Parse(head_positions_rotations[0])/1000,float.Parse(head_positions_rotations[1])/1000, float.Parse(head_positions_rotations[2])/(-1000));
					head_rotation_vector = new Vector3(-1*float.Parse(head_positions_rotations[3]), -1*float.Parse(head_positions_rotations[4]), 0);
				}
				else if (current_body_id == 1) //hand tracking only
				{
					//read in the first body we are tracking -- hand in this case
					int space_index = 0;
					int i = 0;
					for (i = 0; i <5; i++){
						space_index = text.IndexOf(" ", index);
						hand_positions_rotations[i] = text.Substring(index, space_index - index);
						index = space_index + 1;
					}
					space_index = text.IndexOf("]", index);
					hand_positions_rotations[5] = text.Substring(index, space_index - index);

					head_position_vector = DIRE.Instance.displayGeometricCenter;
					head_rotation_vector = Vector3.zero;
					hand_position_vector = new Vector3(float.Parse(hand_positions_rotations[0])/1000,float.Parse(hand_positions_rotations[1])/1000, float.Parse(hand_positions_rotations[2])/(-1000));
					hand_rotation_vector = new Vector3(float.Parse(hand_positions_rotations[3]), float.Parse(hand_positions_rotations[4]), float.Parse(hand_positions_rotations[5]));

				}
			}

			//both head and hand are visible in tracking system
			if(number_of_bodies == 2){
				//read head data
				int space_index = 0;
				int i = 0;
				for (i = 0; i <5; i++){
					space_index = text.IndexOf(" ", index);
					head_positions_rotations[i] = text.Substring(index, space_index - index);
					index = space_index + 1;
				}
				space_index = text.IndexOf("]", index);
				head_positions_rotations[5] = text.Substring(index, space_index - index);

				//copy data to head position/rotation vector
				head_position_vector = new Vector3(float.Parse(head_positions_rotations[0])/1000,float.Parse(head_positions_rotations[1])/1000, float.Parse(head_positions_rotations[2])/(-1000));
				head_rotation_vector = new Vector3(-1*float.Parse(head_positions_rotations[3]), -1*float.Parse(head_positions_rotations[4]), 0);

				//skip ahead to hand data session
				for(i = 0; i < 3; i++){
					space_index = text.IndexOf("[", space_index + 1);
				}

				//read hand data
				index = space_index + 1;
				for (i = 0; i <5; i++){
					space_index = text.IndexOf(" ", index);
					hand_positions_rotations[i] = text.Substring(index, space_index - index);
					index = space_index + 1;
				}// for

				space_index = text.IndexOf("]", index);
				hand_positions_rotations[5] = text.Substring(index, space_index - index);

				hand_position_vector = new Vector3(float.Parse(hand_positions_rotations[0])/1000,float.Parse(hand_positions_rotations[1])/1000, float.Parse(hand_positions_rotations[2])/(-1000));
				hand_rotation_vector = new Vector3(float.Parse(hand_positions_rotations[3]), float.Parse(hand_positions_rotations[4]), float.Parse(hand_positions_rotations[5]));

			}
		} // if (numberofbodies>0)
		
	}// parser

	public void setTransforms()
	{

		// the next two lines set the head and hand positions
		DIRE.Instance.Head.transform.localPosition = head_position_vector;
		DIRE.Instance.Hand.transform.localPosition = hand_position_vector;
		// the next section of code is to point the hand so it faces the correct direction
		Matrix4x4 RotationMatrix = new Matrix4x4();
		
		RotationMatrix.SetColumn(0, new Vector4(Mathf.Cos(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Cos(-hand_rotation_vector.y*Mathf.PI/180), 
		                                        Mathf.Sin(hand_rotation_vector.z*Mathf.PI/180)* Mathf.Cos(-hand_rotation_vector.x*Mathf.PI/180)+Mathf.Cos(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.y*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.x*Mathf.PI/180), 
		                                        Mathf.Sin(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.x*Mathf.PI/180)+ Mathf.Cos(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.y*Mathf.PI/180)* Mathf.Cos(-hand_rotation_vector.x*Mathf.PI/180), 
		                                        0));
		RotationMatrix.SetColumn(1, new Vector4(-Mathf.Sin(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Cos(-hand_rotation_vector.y*Mathf.PI/180), 
		                                        Mathf.Cos(hand_rotation_vector.z*Mathf.PI/180)* Mathf.Cos(-hand_rotation_vector.x*Mathf.PI/180)- Mathf.Sin(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.y*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.x*Mathf.PI/180), 
		                                        Mathf.Cos(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.x*Mathf.PI/180)- Mathf.Sin(hand_rotation_vector.z*Mathf.PI/180)*Mathf.Sin(-hand_rotation_vector.y*Mathf.PI/180)* Mathf.Cos(-hand_rotation_vector.x*Mathf.PI/180), 
		                                        0));
		RotationMatrix.SetColumn(2, new Vector4(Mathf.Sin(-hand_rotation_vector.y*Mathf.PI/180), 
		                                        -Mathf.Cos(-hand_rotation_vector.y*Mathf.PI/180)* Mathf.Sin(-hand_rotation_vector.x*Mathf.PI/180), 
		                                        Mathf.Cos(-hand_rotation_vector.y*Mathf.PI/180)*Mathf.Cos(-hand_rotation_vector.x*Mathf.PI/180),
		                                        0));
		RotationMatrix.SetColumn(3, new Vector4(0,0,0,1));
		
		Vector4 forward4 = RotationMatrix.MultiplyVector(new Vector4 (0,0,1,0));
		Vector3 pointDirection = new Vector3(forward4.x, forward4.y, forward4.z);
		//pointDirection = POI_ReferenceHub.Instance.Avatar.transform.TransformDirection(pointDirection);
		pointDirection = DIRE.Instance.DisplayOrigin.transform.TransformDirection (pointDirection);
		/**
 * 
					Vector3 pointDirection = DIRE.Instance.Hand.transform.localPosition + new Vector3(Mathf.Sin(float.Parse(head_positions_rotations[4])*Mathf.PI/180), 
					                                                                                  -Mathf.Cos(float.Parse(head_positions_rotations[4])*Mathf.PI/180)* Mathf.Sin(float.Parse(head_positions_rotations[3])*Mathf.PI/180), 
					                                                                                  Mathf.Cos(float.Parse(head_positions_rotations[4])*Mathf.PI/180)*Mathf.Cos(float.Parse(head_positions_rotations[3])*Mathf.PI/180));
					Vector3 upVector = new Vector3(-Mathf.Sin(float.Parse(head_positions_rotations[5])*Mathf.PI/180)*Mathf.Cos(float.Parse(head_positions_rotations[4])*Mathf.PI/180), 
					                               Mathf.Cos(float.Parse(head_positions_rotations[5])*Mathf.PI/180)* Mathf.Cos(float.Parse(head_positions_rotations[3])*Mathf.PI/180)- Mathf.Sin(float.Parse(head_positions_rotations[5])*Mathf.PI/180)*Mathf.Sin(float.Parse(head_positions_rotations[4])*Mathf.PI/180)*Mathf.Sin(float.Parse(head_positions_rotations[3])*Mathf.PI/180), 
					                               Mathf.Cos(float.Parse(head_positions_rotations[5])*Mathf.PI/180)*Mathf.Sin(float.Parse(head_positions_rotations[3])*Mathf.PI/180)+ Mathf.Sin(float.Parse(head_positions_rotations[5])*Mathf.PI/180)*Mathf.Sin(float.Parse(head_positions_rotations[4])*Mathf.PI/180)* Mathf.Cos(float.Parse(head_positions_rotations[3])*Mathf.PI/180));
*/
		DIRE.Instance.Hand.transform.LookAt(DIRE.Instance.Hand.transform.position + pointDirection);
		//DIRE.Instance.Hand.transform.localEulerAngles = new Vector3(-float.Parse(head_positions_rotations[3]), -float.Parse(head_positions_rotations[4]), float.Parse(head_positions_rotations[5]));

	}
}
