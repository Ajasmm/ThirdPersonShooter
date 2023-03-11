using AI.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayingOrNot : ActionNode
{
    Animator animator;
    int movementHash;
    GamePlayMode gamePlayMode;

    public IsPlayingOrNot(BehaviourTree tree) : base(tree)
    {
    }

    protected override void OnStart()
    {
        animator = GetData<Animator>("Animator");
        movementHash = Animator.StringToHash("Movement");
        gamePlayMode = GetData<GamePlayMode>("GamePlayMode");
    }

    protected override void OnStop()
    {
        
    }

    protected override BehaviourTreeNodeState OnTick()
    {
        if (gamePlayMode.isPlaying) return BehaviourTreeNodeState.Failed;

        animator.SetFloat(movementHash, 0f);
        return BehaviourTreeNodeState.Successful;
    }
}
