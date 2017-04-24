using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace IsoUnity.Sequences {
	public class EmptyNodeEditor : NodeEditor {

		private SequenceNode node;

		public void draw(){
			EditorGUILayout.HelpBox("Select a content for this node" +
				"or leave it empty to finish the secuence here.",MessageType.Info);
			if(Event.current.type != EventType.layout){
				if(node.Childs.Length != 0)
					node.clearChilds();
			}
		}

		public SequenceNode Result { get{ return node; } }
		public string NodeName{ get { return "Empty node"; } }

	    public string[] ChildNames{ get{ return null; } }

	    public NodeEditor clone(){ return new EmptyNodeEditor(); }

		public bool manages(SequenceNode c) { return c.Content == null; }
		public void useNode(SequenceNode c) {
			if(c.Content != null)	
				c.Content = null;

	        c.ChildSlots = 0;
			
			node = c;
		}
	}
}