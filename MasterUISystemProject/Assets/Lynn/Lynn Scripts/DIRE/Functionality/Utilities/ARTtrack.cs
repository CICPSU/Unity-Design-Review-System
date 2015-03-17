using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

//read and parse the tracking data directly from a network port
public class ARTtrack : MonoBehaviour {
	UdpClient client;
	IPEndPoint source;
	public Vector3 body_position_vector, body_rotation_vector;

	public bool checkTracking(){
		source = new IPEndPoint( IPAddress.Any, 5000);
		client = new UdpClient();
		client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		client.Client.Bind(source);

		float timeThresold =  Time.realtimeSinceStartup + .1f;
		while(Time.realtimeSinceStartup < timeThresold){
			//wait for data to get to client
		}

		return (client.Available > 0);
	}

	// Use this for initialization
	void Start () 
	{
		body_position_vector = Vector3.zero;
		body_rotation_vector = Vector3.zero;
		//data = client.Receive( ref source );
	}

	 
	// Update is called once per frame
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
				byte[] data = client.Receive(ref source);
				//flush the port by reading through all available data till the most recent one
				while(client.Available > 0){
					data = client.Receive( ref source );
				}
					
					string text = System.Text.Encoding.ASCII.GetString(data);			
					parser (text);
					Debug.Log(text);
			}

		}catch(Exception ex){
			Debug.Log(ex.Message);
		}
	}
	

	void parser(string text){
		int index = 0;

		index = text.IndexOf("6d ");

		index +=3; // update index to the position of the number of bodies

		int number_of_bodies = text[index] - '0';
		if (number_of_bodies > 0){

			index +=3; // update index to the position of the first body ID

			int current_body_id = text[index] - '0'; // the id of the track body
												//xbox controller id = 1, head tracking id = 0
			string[] body_positions_rotations= new string[6];

			index = text.IndexOf("[", index) + 1; // update index to the position of the first numerical data for the body

			//Debug.Log("number of bodies: " + number_of_bodies);

			if (number_of_bodies >= 1){

				int space_index = 0;

				int i = 0;

				for (i = 0; i <5; i++){
						space_index = text.IndexOf(" ", index);
						body_positions_rotations[i] = text.Substring(index, space_index - index);
						index = space_index + 1;
				}// for

				space_index = text.IndexOf("]", index);
				body_positions_rotations[5] = text.Substring(index, space_index - index);


				//for (i=0;i<3;i++){
				//	Debug.Log("Body Data: " + i + " " + body_positions_rotations[i]);
				//}// for

				body_position_vector = new Vector3(float.Parse(body_positions_rotations[0])/1000,float.Parse(body_positions_rotations[1])/1000, float.Parse(body_positions_rotations[2])/(-1000));

				body_rotation_vector = new Vector3(-1*float.Parse(body_positions_rotations[3]), -1*float.Parse(body_positions_rotations[4]), 0);

				//Debug.Log("Body x position: " + body_position_vector.x);

				if (current_body_id == 0){// if head tracking only

					DIRE.Instance.Head.transform.localPosition = body_position_vector;
					//may need stabalization code

				}else{ // if xbox tracking only
					DIRE.Instance.Hand.transform.localPosition = new Vector3(float.Parse(body_positions_rotations[0])/1000,float.Parse(body_positions_rotations[1])/1000, float.Parse(body_positions_rotations[2])/1000);
					
					DIRE.Instance.Hand.transform.localEulerAngles = new Vector3(float.Parse(body_positions_rotations[3]), float.Parse(body_positions_rotations[4]), float.Parse(body_positions_rotations[5]));
				} // else


				if(number_of_bodies == 2){
					for(i = 0; i < 3; i++){
						space_index = text.IndexOf("[", space_index + 1);
					}
					index = space_index + 1;
					for (i = 0; i <5; i++){
						space_index = text.IndexOf(" ", index);
						body_positions_rotations[i] = text.Substring(index, space_index - index);
						index = space_index + 1;
					}// for

					space_index = text.IndexOf("]", index);
					body_positions_rotations[5] = text.Substring(index, space_index - index);
					
					body_position_vector = new Vector3(float.Parse(body_positions_rotations[0])/1000,float.Parse(body_positions_rotations[1])/1000, float.Parse(body_positions_rotations[2])/(-1000));
					
					body_rotation_vector = new Vector3(float.Parse(body_positions_rotations[3]), float.Parse(body_positions_rotations[4]), float.Parse(body_positions_rotations[5]));

					DIRE.Instance.Hand.transform.localPosition = new Vector3(float.Parse(body_positions_rotations[0])/1000,float.Parse(body_positions_rotations[1])/1000, float.Parse(body_positions_rotations[2])/1000);
					Debug.Log(text);
					Debug.Log("hand positions: ");
					for(i = 0; i <= 5; i ++){
						Debug.Log(body_positions_rotations[i] + " i: " + i);
					}

					DIRE.Instance.Hand.transform.localEulerAngles = new Vector3(-float.Parse(body_positions_rotations[3]), -float.Parse(body_positions_rotations[4]), float.Parse(body_positions_rotations[5]));

				}
			} // if (numberofbodies==1)



			
		} // if (numberofbodies>0)
	}// parser
}
