using UnityEngine;
using System.Collections.Generic;

public abstract class NodeEditorFactory {
	
	private static NodeEditorFactory instance;
	public static NodeEditorFactory Intance {
		get{ 
			if(instance == null)
				instance = new NodeEditorFactoryImp();
			return instance; 
		}
	}
	
	public abstract string[] CurrentNodeEditors { get; }
	public abstract NodeEditor createNodeEditorFor (string nodeName);	
	public abstract int NodeEditorIndex(SecuenceNode node);
}

public class NodeEditorFactoryImp : NodeEditorFactory {
	
	private List<NodeEditor> nodeEditors;
	private NodeEditor defaultNodeEditor;
	
	public NodeEditorFactoryImp(){
		this.nodeEditors = new List<NodeEditor> ();
		this.nodeEditors.Add (new EmptyNodeEditor ());
		this.nodeEditors.Add (new EventNodeEditor());
		this.nodeEditors.Add (new ForkNodeEditor());
		this.nodeEditors.Add (new DialogNodeEditor());
		

	}
	
	public override string[] CurrentNodeEditors {
		get {
			string[] descriptors = new string[nodeEditors.Count+1];
			for(int i = 0; i< nodeEditors.Count; i++)
				descriptors[i] = nodeEditors[i].NodeName;
			return descriptors;
		}
	}
	
	
	public override NodeEditor createNodeEditorFor (string nodeName)
	{
		foreach (NodeEditor nodeEditor in nodeEditors) {
			if(nodeEditor.NodeName.ToLower() == nodeName.ToLower()){
				return nodeEditor.clone();
			}
		}
		return null;
	}

	public override int NodeEditorIndex(SecuenceNode node){
		
		int i = 0;
		foreach (NodeEditor nodeEditor in nodeEditors) 
			if(nodeEditor.manages(node))	return i;
		else 							i++;
		
		return 0;
	}
}
