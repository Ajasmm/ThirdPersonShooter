using System.Collections;
using System.Collections.Generic;

namespace AI.BehaviourTree
{
    public abstract class CompositeNode : BehaviourTreeNode
    {
        public List<BehaviourTreeNode> childrens = new List<BehaviourTreeNode>();

        protected CompositeNode(BehaviourTree tree) : base(tree)
        {
        }
    }
}