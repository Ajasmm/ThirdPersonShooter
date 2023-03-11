using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviourTree;
using UnityEngine.AI;
using System;

public class Wait : ActionNode
{
    int anim_MovementHash;
    float duratoin, tempDuration;
    float anim_MovementState, anim_TransitionSpeed;
    
    Animator animator;
    NavMeshAgent navMeshAgent;
    Transform myTransform;
    
    bool updatePos;
    bool updateRot;
    
    public Wait(BehaviourTree tree, float waitDuration, float anim_TransitionSpeed) : base(tree)
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
        navMeshAgent.updateRotation = true;
    }

    protected override void OnStop()
    {
        if(navMeshAgent) navMeshAgent.updatePosition = updatePos;
        if(navMeshAgent) navMeshAgent.updateRotation = updateRot;
    }

    protected override BehaviourTreeNodeState OnTick()
    {
        UpdateMovement();
        tempDuration -= Time.deltaTime;
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
