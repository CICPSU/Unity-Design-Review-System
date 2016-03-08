using UnityEngine;
using System.Collections;

public class NavMeshWander : MonoBehaviour {

    public Vector3 dropPoint;

    public enum WanderMode { Idle, Patrol, Explore}

    public float localWanderRadius = 3;

    public Vector3 localWanderCenter = Vector3.zero;

    public WanderMode mode = WanderMode.Patrol;

    public float defaultSpeed = 1.5f;

    public float normalSpeedRadius = 3f;

    public float navSpeedRatio = 1;

    public int poiDestination = 0;

    private NavMeshAgent navAgent;

    private Animator animator;

    private NavMeshHit hit;

    private float idleTimer = 0;

    public bool userDestination = false;

	// Use this for initialization
	void Start () {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navSpeedRatio = 1;
        ConfigureDestination();
	}
	
	// Update is called once per frame
	void Update () {
        if (navAgent.remainingDistance < .5f && !navAgent.pathPending)
        {
            //Debug.Log("reached destination");
            if (navAgent.isOnNavMesh)
                navAgent.Stop();
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("Direction", 0f);

            userDestination = false;
        }
        else
        {
            idleTimer = Time.time;
            animator.SetFloat("Speed", 1f);
        }
        //Debug.Log(Time.time - idleTimer);

        if (navAgent.isOnNavMesh && Time.time - idleTimer > 3 && mode != WanderMode.Idle && !userDestination)
            ConfigureDestination();
	}

    public void ConfigureDestination()
    {
        //Debug.Log("auto configure dest");
        if (localWanderRadius < normalSpeedRadius && navSpeedRatio != localWanderRadius / normalSpeedRadius && mode == WanderMode.Patrol)
        {
            navSpeedRatio = localWanderRadius / normalSpeedRadius;
            
        }
        else
            navSpeedRatio = 1;
        
        if (mode == WanderMode.Idle)
        {
            if(navAgent.isOnNavMesh)
                navAgent.Stop();
            animator.SetFloat("Speed", 0f);
        }
        else if (mode == WanderMode.Patrol)
        {
            NavMesh.SamplePosition(localWanderCenter + new Vector3(Random.Range(-localWanderRadius, localWanderRadius), 0, Random.Range(-localWanderRadius, localWanderRadius)), out hit, 10, -1);
            navAgent.SetDestination(hit.position);
            navAgent.Resume();
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            NavMesh.SamplePosition(transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), out hit, 10, -1);
            navAgent.SetDestination(hit.position);
            navAgent.Resume();
            animator.SetFloat("Speed", 1f);
        }
    }

    public void ConfigureDestination(int poiIndex)
    {
        NavMesh.SamplePosition(POIButtonManager.originalHandler.projectPOIs[poiIndex].position, out hit, 10, -1);
        //Debug.Log(POIButtonManager.originalHandler.projectPOIs[poiIndex].position + " " + hit.position);
        navAgent.SetDestination(hit.position);
        localWanderCenter = hit.position;
        navAgent.Resume();
        animator.SetFloat("Speed", 1f);
        userDestination = true;
        poiDestination = poiIndex + 1;
    }

    void OnAnimatorMove()
    {
        //only perform if moving
        if (!(navAgent.remainingDistance < .5f) && !navAgent.pathPending)
        {
            if (Vector3.Angle(transform.forward, navAgent.desiredVelocity) > 5)
                animator.SetFloat("Direction", Vector3.Angle(transform.forward, navAgent.destination - transform.position));
            else
                animator.SetFloat("Direction", 0);
            /*
            if (animator.GetFloat("Speed") == 1)
            {
                if (Vector3.Angle(transform.forward, navAgent.desiredVelocity) > 45)
                    animator.SetFloat("Direction", 90f);
                else if (Vector3.Angle(transform.forward, navAgent.desiredVelocity) < -45)
                    animator.SetFloat("Direction", -90f);
                else
                    animator.SetFloat("Direction", 0f);    
            }
            */
            animator.speed = navSpeedRatio;
            navAgent.velocity = animator.deltaPosition / Time.deltaTime * navSpeedRatio;
            navAgent.speed = navAgent.velocity.magnitude;
           
            //smoothly rotate the character in the desired direction of motion
            if (navAgent.desiredVelocity != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(navAgent.desiredVelocity);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navAgent.angularSpeed * Time.deltaTime);
            }
        }
    }
}
