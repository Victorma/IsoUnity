using UnityEngine;
using UnityEditor;
using System.Collections;

public class AddItemEditor : EventEditor {

	private GameEvent ge;
	public AddItemEditor() {
		this.ge = ScriptableObject.CreateInstance<GameEvent> ();
		ge.Name = this.EventName;
		ge.setParameter ("item", null);
		ge.setParameter ("inventory", null);
	}
	
	public GameEvent Result { 
		get{ return ge; }
	}
	public string EventName {
		get{ return "Add Item"; }
	}
	public EventEditor clone(){
		return new AddItemEditor();
	}
	
	public void useEvent(GameEvent ge){
		this.ge = ge;
		this.ge.Name = this.EventName;
	}
	
	public void draw(){
		
		ge.setParameter("item", EditorGUILayout.ObjectField("Item", (Object)ge.getParameter("item"), typeof(Item), true));
		ge.setParameter("inventory", EditorGUILayout.ObjectField("Inventory", (Object)ge.getParameter("inventory"), typeof(Inventory), true));
	}
}
