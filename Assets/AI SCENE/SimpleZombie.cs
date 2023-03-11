using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(NavMeshAgent))]
public class SimpleZombie : MonoBehaviour
{
    [SerializeField] List<Transform> wayPoints;

    Animator animator;
    NavMeshAgent navAgent;
    Transform myTransform;

    Vector3 destinationPos, currentPos, nextPos;

    int currentPoint = 0;

    private void Start()
    {
        myTransform = gameObject.GetComponent<Transform>();
        currentPos = myTransform.position;

        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updatePosition = false;
    }

    private void Update()
    {
        currentPos = myTransform.position;
        UpdateDestinationPoint(currentPos);
        navAgent.SetDestination(destinationPos);
        navAgent.nextPosition = currentPos;
        animator.SetFloat("Forward", 1);
    }

    private void UpdateDestinationPoint(Vector3 currentPos)
    {
        if (wayPoints.Count <= 0) destinationPos = currentPos;

        if((destinationPos - currentPos).magnitude < 0.25F)
        {
            currentPoint++;
            if (currentPoint >= wayPoints.Count) currentPoint = 0;
        }

        destinationPos = wayPoints[currentPoint].position;
    }
}
