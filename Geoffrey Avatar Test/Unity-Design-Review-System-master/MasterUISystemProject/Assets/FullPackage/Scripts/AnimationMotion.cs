using UnityEngine;
using System.Collections;

public class AnimationMotion : MonoBehaviour {

	public UnityEngine.AI.NavMeshAgent navAgent;
	public Animator animController;

	void Start()
	{
		navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		animController = GetComponent<Animator>();
	}

	void OnAnimatorMove ()
	{
		//only perform if moving
		if (!navAgent.pathPending)
		{
			//set the navAgent's velocity to the velocity of the animation clip currently playing
			navAgent.velocity = animController.deltaPosition / Time.deltaTime;
			navAgent.speed = navAgent.velocity.magnitude;
			
			//smoothly rotate the character in the desired direction of motion
			if(navAgent.desiredVelocity != Vector3.zero)
			{
				Quaternion lookRotation = Quaternion.LookRotation(navAgent.desiredVelocity);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navAgent.angularSpeed * Time.deltaTime);
			}
		}
	}
}
