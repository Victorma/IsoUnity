using UnityEngine;
using UnityEditor;
using System.Collections;

public class EmptyNodeEditor : NodeEditor {

	private SecuenceNode node;

	public void draw(){
		EditorGUILayout.HelpBox("Select a content for this node" +
			"or leave it empty to finish the secuence here.",MessageType.Info);
		if(Event.current.type != EventType.layout){
			if(node.Childs.Length != 0)
				node.clearChilds();
		}
	}

	public SecuenceNode Result { get{ return node; } }
	public string NodeName{ get { return "Empty node"; } }
	public NodeEditor clone(){ return new EmptyNodeEditor(); }

	public bool manages(SecuenceNode c) { return c.Content == null; }
	public void useNode(SecuenceNode c) {
		if(c.Content != null)	
			c.Content = null;
		
		node = c;
	}
}
