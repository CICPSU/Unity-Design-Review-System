using UnityEngine;
using System.Collections;

public class MessageTimer : MonoBehaviour {

	public int messageSeconds = 5;

	private float messageStart;

	void Awake()
	{
		messageStart = Time.time;
	}

	void Update()
	{
		if(Time.time >= messageStart + messageSeconds)
		{
			MessageHandler.RemoveMessage(this.gameObject);
		}
		else if(Time.time >= messageStart + messageSeconds - 2)
		{
			MessageHandler.FadeMessage(this.gameObject);
		}
	}
}
