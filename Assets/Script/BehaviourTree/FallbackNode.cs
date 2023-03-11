using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviourTree
{
    public class FallbackNode : CompositeNode
    {
        int currentChild = 0;
        BehaviourTreeNodeState childState;

        public FallbackNode(BehaviourTree tree) : base(tree)
        {
        }

        protected override void OnStart()
        {
            currentChild = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override BehaviourTreeNodeState OnTick()
        {
            if (childrens.Count == 0) return BehaviourTreeNodeState.Successful;

            while(currentChild < childrens.Count)
            {
                childState = childrens[currentChild].Tick();
                switch(childState)
                {
                    case BehaviourTreeNodeState.Running:
                        currentChild = 0;
                        return BehaviourTreeNodeState.Running;
                    case BehaviourTreeNodeState.Successful:
                        return BehaviourTreeNodeState.Successful;
                    case BehaviourTreeNodeState.Failed:
                        currentChild++;
                        break;
                }
            }
            return BehaviourTreeNodeState.Failed;
        }
    }
}