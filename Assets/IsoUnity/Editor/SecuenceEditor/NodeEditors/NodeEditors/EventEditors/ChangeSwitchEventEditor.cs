using UnityEngine;
using UnityEditor;
using System.Collections;

public class ChangeSwitchEventEditor : EventEditor {

	private GameEvent ge;
	public ChangeSwitchEventEditor() {
		this.ge = ScriptableObject.CreateInstance<GameEvent> ();
		ge.Name = this.EventName;
		ge.setParameter ("switch", "");
		ge.setParameter ("value", true);
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
		if (ge.getParameter ("switch") == null)
			ge.setParameter ("switch", "");
		if (ge.getParameter ("value") == null)
			ge.setParameter ("value", true);
	}
	
	public void draw(){

		ge.setParameter("switch", EditorGUILayout.TextField("SwitchID", (string) ge.getParameter("switch")));
		ge.setParameter("value", ParamEditor.editorFor("Value", ge.getParameter("value")));
	}
	
}