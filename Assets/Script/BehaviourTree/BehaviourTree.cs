using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviourTree
{
    public class BehaviourTree
    {
        public Dictionary<string, object> blackBoard = new Dictionary<string, object>();
        public BehaviourTreeNode rootNode;

        public BehaviourTreeNodeState UpdateTree()
        {
            if (rootNode == null) return BehaviourTreeNodeState.Failed;
            return rootNode.Tick();
        }

        public void AddData(string name, object data)
        {
            blackBoard[name] = data;
        }
        public void RemoveData(string name)
        {
            blackBoard[name] = null;
        }
    }
}