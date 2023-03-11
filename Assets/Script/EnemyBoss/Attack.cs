using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviourTree;
using UnityEngine.AI;

public class Attack : ActionNode
{
    int anim_MovementHash, anim_AttackHash;
    float anim_MovementState, anim_TransitionSpeed;

    Animator animator;
    NavMeshAgent navMeshAgent;
    Transform myTransofrm, playerTransform;
    private bool updatePos;
    private bool updateRot;

    public Attack(BehaviourTree tree) : base(tree)
    {
    }

    protected override void OnStart()
    {
        anim_MovementHash = Animator.StringToHash("Movement");
        anim_AttackHash = Animator.StringToHash("Attack");
        animator = GetData<Animator>("Animator");

        navMeshAgent = GetData<NavMeshAgent>("NavMeshAgent");

        updatePos = navMeshAgent.updatePosition;
        updateRot = navMeshAgent.updateRotation;

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = true;

        myTransofrm = GetData<Transform>("Transform");
        playerTransform = GetData<Transform>("PlayerTransform");

    }

    protected override void OnStop()
    {
        if (navMeshAgent) navMeshAgent.updatePosition = updatePos;
        if (navMeshAgent) navMeshAgent.updateRotation = updateRot;
    }

    protected override BehaviourTreeNodeState OnTick()
    {
        navMeshAgent.SetDestination(playerTransform.position);
        UpdateAnimationState();
        animator.SetTrigger(anim_AttackHash);
        return BehaviourTreeNodeState.Successful;
    }

    private void UpdateAnimationState()
    {
        anim_MovementState = animator.GetFloat(anim_MovementHash);

        if (anim_MovementState > 0) anim_MovementState -= Time.deltaTime * anim_TransitionSpeed;
        if (anim_MovementState < 0) anim_MovementState = 0;

        animator.SetFloat(anim_MovementHash, anim_MovementState);

        navMeshAgent.nextPosition = myTransofrm.position;
    }
}
