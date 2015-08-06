using UnityEngine;
using UnityEditor;
using System.Collections;

public class EventNodeEditor : NodeEditor {

	private SecuenceNode node;

	public void draw(){
		GameEvent ge = (GameEvent)node.Content;
		string[] editors = EventEditorFactory.Intance.CurrentEventEditors;
		int editorSelected = 0;
		if(ge.Name == null)
			ge.Name = "";
		for (int i = 1; i< editors.Length; i++)
			if (editors [i].ToLower () == ge.Name.ToLower ())
				editorSelected = i;
		int was = editorSelected;
		
		editorSelected = EditorGUILayout.Popup (editorSelected, EventEditorFactory.Intance.CurrentEventEditors);
		if(was != editorSelected && editorSelected == 0)
			ge.Name = "";
		EventEditor editor = EventEditorFactory.Intance.createEventEditorFor (editors[editorSelected]);
		editor.useEvent (ge);		
		
		editor.draw ();

		ge.setParameter("synchronous", EditorGUILayout.Toggle("Synchronous", 
		    (ge.getParameter("synchronous") == null)?false:(bool) ge.getParameter("synchronous")));
		if((bool)ge.getParameter("synchronous")){
			EditorGUILayout.HelpBox("Notice that if there is no EventFinished event, the game will stuck.", MessageType.Warning);
		}
		
		node.Content = editor.Result;
		
		if (Event.current.type != EventType.layout){
			int l = node.Childs.Length;
			if (l != 1) {
				if(l == 0) node.addNewChild();
				else while(l > 1){
					node.removeChild(l-1);
					l--;
				}
				//this.Repaint ();
			}
		}
	}
	
	public SecuenceNode Result { get{ return node; } }
	public string NodeName{ get { return "GameEvent"; } }
	public NodeEditor clone(){ return new EventNodeEditor(); }
	
	public bool manages(SecuenceNode c) { return c.Content != null && c.Content is GameEvent; }
	public void useNode(SecuenceNode c) {
		if(c.Content == null || !(c.Content is GameEvent))	
			c.Content = ScriptableObject.CreateInstance<GameEvent>();

		this.node = c;
	}
}