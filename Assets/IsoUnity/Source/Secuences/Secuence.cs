using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Secuence : ScriptableObject {
	[SerializeField]
	private SecuenceNode root;

    [SerializeField]
    private List<SecuenceNode> nodes;

	public void init(){
		Debug.Log ("Root created");
        this.nodes = new List<SecuenceNode>();
        root = this.createChild();
		Debug.Log (root);
		DontDestroyOnLoad(this);
	}
	public SecuenceNode Root{
		get{ return root;}
		set{ root = value;}
	}

    public SecuenceNode[] Nodes
    {
        get { return nodes.ToArray() as SecuenceNode[]; }
    }

    public SecuenceNode createChild()
    {
        var node = ScriptableObject.CreateInstance<SecuenceNode>();
        node.init(this);
        this.nodes.Add(node);
        return node;
    }

    public bool removeChild(SecuenceNode node)
    {
        int pos = nodes.IndexOf(node);

        if (pos != -1)
        {
            nodes.RemoveAt(pos);
            SecuenceNode.DestroyImmediate(node);
        }

        return pos != -1;
    }

    private void findNodes(SecuenceNode node, Dictionary<SecuenceNode, bool> checkList)
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
            Dictionary<SecuenceNode, bool> found = new Dictionary<SecuenceNode, bool>();
            foreach (SecuenceNode sn in nodes)
                found.Add(sn, false);

            findNodes(root, found);

            int free = 0;
            foreach (var v in found.Values) if (!v) free++;

            return free;
        }
    }

    /*public Rect getRectFor(SecuenceNode node)
    {
        int i = nodes.IndexOf(node);
        Rect r = positions[i];
        if (r == null || r.width == 0)
        {
            // TODO reposition
            r = new Rect(10, 10, 300, 0);
            positions[i] = r;
        }

        return r;
    }

    public void setRectFor(SecuenceNode node, Rect rect)
    {
        int i = nodes.IndexOf(node);
        positions[i] = rect;
    }*/
}
