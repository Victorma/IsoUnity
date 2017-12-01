using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace IsoUnity.Sequences {
	[System.Serializable]
	public class Sequence : ScriptableObject, ISerializationCallbackReceiver {

        /// <summary>
        /// Sequence current is used to know what is the current executing Sequence
        /// </summary>
        public static Sequence current;

        [SerializeField]
	    protected SequenceNode root;

	    [SerializeField]
	    protected IsoSwitches localVariables;

	    protected Dictionary<string, SequenceNode> nodeDict;

	    void Awake()
	    {
	        if (this.ids == null)
	            this.ids = new List<string>();
	        if (this.nodeDict == null)
	            this.nodeDict = new Dictionary<string, SequenceNode>();
	        if (localVariables == null)
	            this.localVariables = IsoSwitches.CreateInstance<IsoSwitches>();
	    }

	    public SequenceNode Root
	    {
	        get { return root; }
	        set { root = value; }
	    }

	    public SequenceNode[] Nodes
	    {
	        get { return nodeDict.Values.ToArray() as SequenceNode[]; }
	    }

	    public SequenceNode this[string node]
	    {
	        get
	        {
	            if (!nodeDict.ContainsKey(node)) CreateNode(node, null);
	            return nodeDict[node];
	        }
			set 
			{
				if (!nodeDict.ContainsKey (node)) {
					nodeDict.Add (node, value);
				} else {
					nodeDict [node] = value;
				}
			}
	    }

	    public IsoSwitches LocalVariables
	    {
	        get
	        {
	            return localVariables;
	        }
	    }

	    public virtual bool ContainsVariable(string id)
	    {
	        return localVariables.containsSwitch(id);
	    }

	    public virtual object GetVariable(string id)
	    {
	        return localVariables.consultSwitch(id);
	    }

	    public virtual void SetVariable(string id, object value)
	    {
	        localVariables.getSwitch(id).State = value;
	    }

	    public virtual SequenceNode CreateNode(string id, object content = null, int childSlots = 0)
	    {
	        var node = CreateInstance<SequenceNode>();
	        node.init(this);
	        this.nodeDict.Add(id, node);
	        node.Content = content;
	        return node;
	    }

	    public virtual SequenceNode CreateNode(object content = null, int childSlots = 0)
	    {
	        var node = CreateInstance<SequenceNode>();
	        node.init(this);
	        this.nodeDict.Add(node.GetInstanceID().ToString(), node);
	        node.Content = content;
	        return node;
	    }

	    public virtual bool RemoveNode(SequenceNode node)
	    {
	        var id = string.Empty;
	        foreach(var kv in nodeDict)
	        {
	            if(kv.Value == node)
	            {
	                id = kv.Key;
	                break;
	            }
	        }

	        return string.IsNullOrEmpty(id) ? false : RemoveNode(id);
	    }

	    public virtual bool RemoveNode(string id)
	    {
	        var contains = nodeDict.ContainsKey(id);
	        if (contains)
	        {
	            var node = nodeDict[id];
	            nodeDict.Remove(id);
	            SequenceNode.DestroyImmediate(node, true);
	        }
	        return contains;
	    }

	    private void findNodes(SequenceNode node, Dictionary<SequenceNode, bool> checkList)
	    {
	        if (node == null)
	            return;

	        if (checkList.ContainsKey(node))
	            checkList[node] = true;

	        foreach (var c in node.Childs)
	            findNodes(c, checkList);
	    }


	    public int FreeNodes
	    {
	        get
	        {
	            Dictionary<SequenceNode, bool> found = new Dictionary<SequenceNode, bool>();
	            foreach (SequenceNode sn in nodes)
	                found.Add(sn, false);

	            findNodes(root, found);

	            int free = 0;
	            foreach (var v in found.Values) if (!v) free++;

	            return free;
	        }
	    }


		public virtual Sequence Clone()
		{
			var clone = this.MemberwiseClone () as Sequence;

			// Clone the nodes
			var clonedNodes = new Dictionary<SequenceNode, SequenceNode> ();
			foreach (var n in Nodes)
				clonedNodes.Add (n, n.Clone ());

			// Assign the childs
			foreach (var kv in clonedNodes)
				for (int i = 0; i < kv.Value.ChildSlots; i++) 
					if(kv.Value.Childs[i] != null)
						kv.Value.Childs [i] = clonedNodes [kv.Value.Childs [i]];

			// Fill up the sequence
			clone.nodeDict = new Dictionary<string, SequenceNode> ();
			foreach (var kv in nodeDict)
				clone.nodeDict.Add (kv.Key, clonedNodes [kv.Value]);

            if(Root)
                clone.Root = clonedNodes [Root];

			// Clone the objects
			clone.objectPool = new Dictionary<string, object>();
			foreach (var kv in objectPool)
				clone.objectPool.Add (kv.Key, kv.Value);

			// Local variables
			clone.localVariables = localVariables.Clone ();

			return clone;
		}

	    /**************************
	     * Object pool
	     * ************************/
	    protected Dictionary<string, object> objectPool = new Dictionary<string, object>();

	    public object GetObject(string name)
	    {
	        return objectPool.ContainsKey(name) ? objectPool[name] : null;
	    }

	    public void SetObject(string name, object value)
	    {
	        if (objectPool.ContainsKey(name))
	            objectPool[name] = value;
	        else
	            objectPool.Add(name, value);
	    }

	    /**************************
	     * Serialization
	     * ***********************/
	    [SerializeField]
	    private List<SequenceNode> nodes;
	    [SerializeField]
	    private List<string> ids;

	    public virtual void OnBeforeSerialize()
	    {
	        this.nodes = nodeDict.Values.ToList();
	        this.ids = nodeDict.Keys.ToList();
	    }

	    public virtual void OnAfterDeserialize()
	    {
	        nodeDict = new Dictionary<string, SequenceNode>();

	        using (var n = nodes.GetEnumerator())
	        using (var i = ids.GetEnumerator())
	        {
	            while (n.MoveNext() && i.MoveNext())
	                nodeDict.Add(i.Current, n.Current);
	        }
	    }
	}
}