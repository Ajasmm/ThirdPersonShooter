using UnityEngine;

namespace AI.BehaviourTree
{
    public abstract class BehaviourTreeNode
    {
        protected BehaviourTree tree;

        protected bool isStarted = false;
        public BehaviourTreeNodeState state = BehaviourTreeNodeState.Running;

        public BehaviourTreeNode(BehaviourTree tree)
        {
            this.tree = tree;
        }

        public virtual BehaviourTreeNodeState Tick()
        {
            if (!isStarted)
            {
                state = BehaviourTreeNodeState.Running;
                isStarted = true;
                OnStart();
            }

            if (isStarted) state = OnTick();

            if (state == BehaviourTreeNodeState.Failed || state == BehaviourTreeNodeState.Successful)
            {
                OnStop();
                isStarted = false;
            }

            return state;
        }
        public T GetData<T>(string key)
        {
            T value;
            object pullObject;
            if (tree.blackBoard.TryGetValue(key, out pullObject))
            {
                // if(pullObject == null) Debug.Log($"error occured with Key : {key} and Object is null");
                value = (T)pullObject;
                return value;
            }

            // Debug.Log("error occured with Key : " + key);
            return default(T);
        }
        public void AddData(string key, object data)
        {
            tree.blackBoard[key] = data;
        }

        protected abstract void OnStart();
        protected abstract BehaviourTreeNodeState OnTick();
        protected abstract void OnStop();
    }
}