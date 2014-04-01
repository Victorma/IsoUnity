using UnityEngine;
using UnityEditor;
using System.Collections;

public class ChangeSwitchEventEditor : EventEditor {

	private GameEvent ge;
	public ChangeSwitchEventEditor() {
		this.ge = ScriptableObject.CreateInstance<GameEvent> ();
		ge.Name = this.EventName;
		ge.setParameter ("switch", "");
		ge.setParameter ("state", true);
	}
	
	public GameEvent Result { 
		get{ return ge; }
	}
	public string EventName {
		get{ return "ChangeSwitch"; }
	}
	public EventEditor clone(){
		return new ChangeSwitchEventEditor();
	}
	
	public void useEvent(GameEvent ge){
		this.ge = ge;
		this.ge.Name = this.EventName;
	}
	
	public void draw(){

		ge.setParameter("switch", EditorGUILayout.TextField("SwitchID", (string) ge.getParameter("switch")));
		ge.setParameter ("value", EditorGUILayout.Toggle("Value", (bool) ge.getParameter("value")));
	}
	
}