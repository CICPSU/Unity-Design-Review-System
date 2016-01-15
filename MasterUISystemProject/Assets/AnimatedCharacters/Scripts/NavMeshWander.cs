using UnityEngine;
using System.Collections;

public class NavMeshWander : MonoBehaviour {

    public enum WanderMode { Idle, Local, World}

    public float localWanderRadius = 10;

    public Vector3 localWanderCenter = Vector3.zero;

    public GameObject localWanderCenterRef;

    public WanderMode mode = WanderMode.Local;

    private NavMeshAgent navAgent;

    private Animator animator;

    private NavMeshHit hit;

	// Use this for initialization
	void Start () {
        if (localWanderCenterRef != null)
            localWanderCenter = localWanderCenterRef.transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ConfigureDestination();
	}
	
	// Update is called once per frame
	void Update () {
        if (navAgent.isOnNavMesh && navAgent.remainingDistance < 1)
            ConfigureDestination();
	}

    public void ConfigureDestination()
    {
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
