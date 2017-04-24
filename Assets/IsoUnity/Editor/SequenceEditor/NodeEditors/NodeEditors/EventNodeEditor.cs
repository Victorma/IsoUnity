using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace IsoUnity.Sequences {
	public class EventNodeEditor : NodeEditor {

		private SequenceNode node;
	    private EventEditor currentEditor;

		public void draw(){
			SerializableGameEvent ge = (SerializableGameEvent)node.Content;
			string[] editors = EventEditorFactory.Intance.CurrentEventEditors;
			int editorSelected = 0;

			for (int i = 1; i< editors.Length; i++)
				if (ge != null && editors [i].ToLower () == ge.Name.ToLower ())
					editorSelected = i;

			int was = editorSelected;
			
			editorSelected = EditorGUILayout.Popup (editorSelected, EventEditorFactory.Intance.CurrentEventEditors);
	        if (currentEditor == null || was != editorSelected)
	        {
	            if(currentEditor != null && ge != null)
	                currentEditor.detachEvent(ge);

	            if(ge != null)
	                ge.Name = "";

	            currentEditor = EventEditorFactory.Intance.createEventEditorFor(editors[editorSelected]);
	            currentEditor.useEvent(ge);
	        }

	        currentEditor.draw();

	        /**
	         *  Game event synchronization
	         * */

	        if (!(ge.getParameter("synchronous") is bool)) ge.setParameter("synchronous", false);
			ge.setParameter("synchronous", EditorGUILayout.Toggle("Synchronous", 
			    (ge.getParameter("synchronous") == null)?false:(bool) ge.getParameter("synchronous")));

			if((bool)ge.getParameter("synchronous"))
				EditorGUILayout.HelpBox("Notice that if there is no EventFinished event, the game will stuck.", MessageType.Warning);
	        /**
	         * Synchronization end
	         * */

			node.Content = currentEditor.Result;
			
			if (Event.current.type != EventType.layout){
	            node.ChildSlots = 1;
			}
		}
		
		public SequenceNode Result { get{ return node; } }
		public string NodeName{ get { return "GameEvent"; } }

	    public string[] ChildNames
	    {
	        get
	        {
	            return new string[]{ "default" };
	        }
	    }

	    public NodeEditor clone(){ return new EventNodeEditor(); }
		
		public bool manages(SequenceNode c) { return c.Content != null && c.Content is SerializableGameEvent; }
		public void useNode(SequenceNode c) {
			if (c.Content == null || !(c.Content is SerializableGameEvent)) {
				c.Content = ScriptableObject.CreateInstance<SerializableGameEvent> ();
			}
			var sge = c.Content as SerializableGameEvent;
			if(sge.Name == null) sge.Name = "";

			this.node = c;
		}
	}
}