using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviourTree;
using UnityEngine.AI;
using System;

public class FollowWait : ActionNode
{
    int anim_MovementHash;
    float duratoin, tempDuration;
    float anim_MovementState, anim_TransitionSpeed;
    
    Animator animator;
    NavMeshAgent navMeshAgent;
    Transform myTransform;

    bool updatePos, updateRot;

    public FollowWait(BehaviourTree tree, float waitDuration, float anim_TransitionSpeed) : base(tree)
    {
        duratoin = waitDuration;
        this.anim_TransitionSpeed = anim_TransitionSpeed;
    }

    protected override void OnStart()
    {
        tempDuration = duratoin;
        anim_MovementHash = Animator.StringToHash("Movement");

        animator = GetData<Animator>("Animator");
        navMeshAgent = GetData<NavMeshAgent>("NavMeshAgent");
        myTransform = GetData<Transform>("Transform");

        updatePos = navMeshAgent.updatePosition;
        updateRot = navMeshAgent.updateRotation;

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    protected override void OnStop()
    {
        if (navMeshAgent) navMeshAgent.updatePosition = updatePos;
        if (navMeshAgent) navMeshAgent.updateRotation = updateRot;
    }

    protected override BehaviourTreeNodeState OnTick()
    {
        if (!GetData<bool>("IsPreviouslyFollowing")) return BehaviourTreeNodeState.Failed;
        UpdateMovement();
        tempDuration -= Time.deltaTime;
        if (tempDuration < 0) AddData("IsPreviouslyFollowing", false); 
        return (tempDuration <= 0) ? BehaviourTreeNodeState.Successful : BehaviourTreeNodeState.Running;
    }

    private void UpdateMovement()
    {
        anim_MovementState = animator.GetFloat(anim_MovementHash);
        if(anim_MovementState > 0)
        {
            anim_MovementState -= Time.deltaTime * anim_TransitionSpeed;
            if (anim_MovementState < 0) anim_MovementState = 0;
        }
        animator.SetFloat(anim_MovementHash, anim_MovementState);

        navMeshAgent.nextPosition = myTransform.position;
    }
    public void ResetNode()
    {
        OnStop();
        isStarted = false;
    }
}
