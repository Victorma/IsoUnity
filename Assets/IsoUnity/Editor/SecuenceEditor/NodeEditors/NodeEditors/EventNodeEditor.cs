using UnityEngine;
using UnityEditor;
using System.Collections;

public class EventNodeEditor : NodeEditor {

	private SecuenceNode node;
    private EventEditor currentEditor;

	public void draw(){
		GameEvent ge = (GameEvent)node.Content;
		string[] editors = EventEditorFactory.Intance.CurrentEventEditors;
		int editorSelected = 0;

		for (int i = 1; i< editors.Length; i++)
			if (editors [i].ToLower () == ge.Name.ToLower ())
				editorSelected = i;

		int was = editorSelected;
		
		editorSelected = EditorGUILayout.Popup (editorSelected, EventEditorFactory.Intance.CurrentEventEditors);
        if (currentEditor == null || was != editorSelected)
        {
            if(currentEditor != null)
                currentEditor.detachEvent(ge);

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
	
	public SecuenceNode Result { get{ return node; } }
	public string NodeName{ get { return "GameEvent"; } }
	public NodeEditor clone(){ return new EventNodeEditor(); }
	
	public bool manages(SecuenceNode c) { return c.Content != null && c.Content is GameEvent; }
	public void useNode(SecuenceNode c) {
		if(c.Content == null || !(c.Content is GameEvent))	
			c.Content = ScriptableObject.CreateInstance<GameEvent>();

        var ge = c.Content as GameEvent;
        if(ge.Name == null) ge.Name = "";

		this.node = c;
	}
}