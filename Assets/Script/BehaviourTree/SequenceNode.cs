using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviourTree
{

    public class SequenceNode : CompositeNode
    {

        int currentChild;
        BehaviourTreeNodeState childState;

        public SequenceNode(BehaviourTree tree) : base(tree)
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
            while(currentChild < childrens.Count)
            {
                childState = childrens[currentChild].Tick();  
                switch(childState)
                {
                    case BehaviourTreeNodeState.Running:
                        return BehaviourTreeNodeState.Running;
                    case BehaviourTreeNodeState.Successful:
                        currentChild++;
                        break;
                    case BehaviourTreeNodeState.Failed:
                        return BehaviourTreeNodeState.Failed; 
                }
            }
            return BehaviourTreeNodeState.Successful;
        }
    }
}