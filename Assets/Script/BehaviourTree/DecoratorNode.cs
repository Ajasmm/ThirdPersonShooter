namespace AI.BehaviourTree
{
    public abstract class DecoratorNode : BehaviourTreeNode {
        public BehaviourTreeNode child;

        protected DecoratorNode(BehaviourTree tree) : base(tree)
        {
        }
    }
}