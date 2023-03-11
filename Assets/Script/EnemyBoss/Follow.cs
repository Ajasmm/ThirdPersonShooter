using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviourTree;
using UnityEngine.AI;

public class Follow : ActionNode
{
    Transform myTransform, playerTransform;
    Animator animator;
    NavMeshAgent navMeshAgent;

    int anim_MovementHash;
    float anim_MovementState, anim_TransitionSpeed;
    float dotValue;
    private bool updatePos;
    private bool updateRot;

    public Follow(BehaviourTree tree, float anim_TransitionSpeed) : base(tree)
    {
        this.anim_TransitionSpeed = anim_TransitionSpeed;
    }

    protected override void OnStart()
    {
        myTransform = GetData<Transform>("Transform");
        playerTransform = GetData<Transform>("PlayerTransform");

        animator = GetData<Animator>("Animator");
        navMeshAgent = GetData<NavMeshAgent>("NavMeshAgent");
        navMeshAgent.SetDestination(playerTransform.position);
        anim_MovementHash = Animator.StringToHash("Movement");

        updatePos = navMeshAgent.updatePosition;
        updateRot = navMeshAgent.updateRotation;

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = true;
    }

    protected override void OnStop()
    {
        if (navMeshAgent) navMeshAgent.updatePosition = updatePos;
        if (navMeshAgent) navMeshAgent.updateRotation = updateRot;
    }

    protected override BehaviourTreeNodeState OnTick()
    {
        AddData("IsPreviouslyFollowing", true);
        
        UpdateAnimationMove();
        return BehaviourTreeNodeState.Successful;
    }

    private void UpdateAnimationMove()
    {
        anim_MovementState =  animator.GetFloat(anim_MovementHash);

        if (anim_MovementState < 1) anim_MovementState += Time.deltaTime * anim_TransitionSpeed;
        if (anim_MovementState > 1) anim_MovementState = 1;

        animator.SetFloat(anim_MovementHash, anim_MovementState);

        navMeshAgent.SetDestination(playerTransform.position);
        navMeshAgent.nextPosition = myTransform.position;
    }
}
