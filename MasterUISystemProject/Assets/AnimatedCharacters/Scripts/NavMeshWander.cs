using UnityEngine;
using System.Collections;

public class NavMeshWander : MonoBehaviour {

    public enum WanderMode { Idle, Local, World}

    public float localWanderRadius = 3;

    public Vector3 localWanderCenter = Vector3.zero;

    public WanderMode mode = WanderMode.Local;

    public float defaultSpeed = 1.5f;

    public float normalSpeedRadius = 3f;

    public float navAgentSpeed;

    private NavMeshAgent navAgent;

    private Animator animator;

    private NavMeshHit hit;

	// Use this for initialization
	void Start () {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navAgentSpeed = defaultSpeed;
        ConfigureDestination();
	}
	
	// Update is called once per frame
	void Update () {
        if (navAgent.isOnNavMesh && (navAgent.remainingDistance < 1 || mode == WanderMode.Idle))
            ConfigureDestination();
	}

    public void ConfigureDestination()
    {
       
        if (localWanderRadius < normalSpeedRadius && navAgentSpeed != defaultSpeed * localWanderRadius / normalSpeedRadius)
        {
            navAgentSpeed = defaultSpeed * localWanderRadius / normalSpeedRadius;
            animator.speed = localWanderRadius / normalSpeedRadius;
        }
        else
            GetComponent<Animator>().speed = 1;
        navAgent.speed = navAgentSpeed;

        

        if (mode == WanderMode.Idle)
        {
            if(navAgent.isOnNavMesh)
                navAgent.Stop();
            animator.SetFloat("Speed", 0f);
        }
        else if (mode == WanderMode.Local)
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
}
