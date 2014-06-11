using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SecuenceNode : ScriptableObject {
	[SerializeField]
	private List<SecuenceNode> childs;
	[SerializeField]
	private Object content = null;

	public void init(){
		childs = new List<SecuenceNode> ();
		DontDestroyOnLoad (this);
	}
	
	public SecuenceNode[] Childs {
		get{ return childs.ToArray() as SecuenceNode[]; }
	}
	
	public string Name{
		get{ return name;} 
		set{ name = value;}
	}
	
	public Object Content{
		get{ return content;}
		set{ content = value;}
	}
	
	public void clearChilds(){
		foreach (ScriptableObject node in childs)
			if(Application.isEditor)
				ScriptableObject.DestroyImmediate (node);
			else
				ScriptableObject.Destroy (node);
		childs.Clear();
	}
	
	public SecuenceNode addNewChild(){
		SecuenceNode node = ScriptableObject.CreateInstance<SecuenceNode>();
		node.init ();
		this.childs.Add(node);
		return node;
	}
	
	public void removeChild(int i){
		SecuenceNode node = this.childs [i];
		if (node != null) {
			this.childs.RemoveAt(i);
			if(Application.isEditor)
				ScriptableObject.DestroyImmediate (node);
			else
				ScriptableObject.Destroy (node);;
		}

	}
	
	public void removeChild(SecuenceNode child){
		this.childs.Remove(child);
		if(Application.isEditor)
			ScriptableObject.DestroyImmediate (child);
		else
			ScriptableObject.Destroy (child);
	}

}