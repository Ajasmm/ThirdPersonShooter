using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviourTree;
using System;

public class IsInRange : ActionNode
{
    Transform myTransform, playerTransform;
    PlayerController playerController;

    private float range, distance;
    Vector3 currentPos, playerPos;

    public Action IfInRange;

    public IsInRange(BehaviourTree tree, float range) : base(tree)
    {
        this.range = range;
    }

    protected override void OnStart()
    {
        myTransform = GetData<Transform>("Transform");
        playerTransform = GetData<Transform>("PlayerTransform");
        playerController = playerTransform.GetComponent<PlayerController>();
    }

    protected override void OnStop()
    {
        
    }

    protected override BehaviourTreeNodeState OnTick()
    {
        if (playerController.health <= 0) return BehaviourTreeNodeState.Failed;

        currentPos = myTransform.position;
        playerPos = playerTransform.position;

        currentPos.y = 0;
        playerPos.y = 0;

        distance = (currentPos - playerPos).magnitude;
        if (distance < range && IfInRange != null) IfInRange();
        return (distance < range) ? BehaviourTreeNodeState.Successful : BehaviourTreeNodeState.Failed;
    }
}
