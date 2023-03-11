using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviourTree;
using UnityEngine.AI;

public class Patrol : ActionNode
{
    List<Transform> wayPoints;
    int currentWaypoint = 0, anim_MovementHash;

    Vector3 destination, currentPos;

    Transform myTransform;
    NavMeshAgent navMeshAgent;
    Animator animator;

    float anim_MovementState, anim_TransitionSpeed;


    private bool updatePos;
    private bool updateRot;

    public Patrol(BehaviourTree tree, float anim_TransitionSpeed) : base(tree)
    {
        this.anim_TransitionSpeed = anim_TransitionSpeed;
    }

    protected override void OnStart()
    {
        myTransform = GetData<Transform>("Transform");
        navMeshAgent = GetData<NavMeshAgent>("NavMeshAgent");
        animator = GetData<Animator>("Animator");

        updatePos = navMeshAgent.updatePosition;
        updateRot = navMeshAgent.updateRotation;

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = true;

        anim_MovementHash = Animator.StringToHash("Movement");
        wayPoints = GetData<List<Transform>>("Waypoints");

        destination = wayPoints[currentWaypoint].position;
        navMeshAgent.SetDestination(destination);

    }

    protected override void OnStop()
    {
        if (navMeshAgent) navMeshAgent.updatePosition = updatePos;
        if (navMeshAgent) navMeshAgent.updateRotation = updateRot;
    }

    protected override BehaviourTreeNodeState OnTick()
    {
        currentPos = myTransform.position;
        UpdateAnimMovementState(anim_TransitionSpeed);

        destination.y = 0;
        currentPos.y = 0;
        if((destination - currentPos).magnitude < 0.5F)
        {
            UpdateWaypoint();
            return BehaviourTreeNodeState.Successful;
        }
        return BehaviourTreeNodeState.Running;
    }


    private void UpdateAnimMovementState(float anim_TransitionSpeed)
    {
        anim_MovementState = animator.GetFloat(anim_MovementHash);
        if(anim_MovementState < 1)
        {
            anim_MovementState += Time.deltaTime * anim_TransitionSpeed;
            if (anim_MovementState > 1) anim_MovementState = 1;
        }
        animator.SetFloat(anim_MovementHash, anim_MovementState);
        navMeshAgent.nextPosition = currentPos;
    }
    private void UpdateWaypoint()
    {
        currentWaypoint++;
        if (currentWaypoint >= wayPoints.Count) currentWaypoint = 0;
    }

    public void ResetNode()
    {
        isStarted = false;
    }
}
