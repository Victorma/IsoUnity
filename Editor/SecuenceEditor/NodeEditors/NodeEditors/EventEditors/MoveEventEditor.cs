using UnityEngine;
using UnityEditor;
using System.Collections;

public class MoveEventEditor : EventEditor {

	private GameEvent ge;
	public MoveEventEditor() {
		this.ge = ScriptableObject.CreateInstance<GameEvent> ();
		ge.Name = this.EventName;
		ge.setParameter ("entity", null);
		ge.setParameter ("cell", null);
	}

	public GameEvent Result { 
		get{ return ge; }
	}
	public string EventName {
		get{ return "Move"; }
	}
	public EventEditor clone(){
		return new MoveEventEditor();
	}

	public void useEvent(GameEvent ge){
		this.ge = ge;
		this.ge.Name = this.EventName;
	}
	
	public void draw(){

		ge.setParameter("entity", EditorGUILayout.ObjectField("Entity", (Object)ge.getParameter("entity"), typeof(Entity), true));
		ge.setParameter("cell", EditorGUILayout.ObjectField("Cell", (Object)ge.getParameter("cell"), typeof(Cell), true));
	}

}
