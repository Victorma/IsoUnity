using UnityEngine;
using System.Collections.Generic;

public abstract class EventEditorFactory {

	private static EventEditorFactory instance;
	public static EventEditorFactory Intance {
		get{ 
			if(instance == null)
				instance = new EventEditorFactoryImp();
			return instance; 
		}
	}

	public abstract string[] CurrentEventEditors { get; }
	public abstract EventEditor createEventEditorFor (string eventName);

}

public class EventEditorFactoryImp : EventEditorFactory {

	private List<EventEditor> eventEditors;
	private EventEditor defaultEventEditor;

	public EventEditorFactoryImp(){
		this.eventEditors = new List<EventEditor> ();
		this.eventEditors.Add (new MoveEventEditor ());
		this.eventEditors.Add (new ChangeSwitchEventEditor ());
		this.eventEditors.Add (new AddItemEditor ());


		this.defaultEventEditor = new DefaultEventEditor ();
	}

	public override string[] CurrentEventEditors {
		get {
			string[] descriptors = new string[eventEditors.Count+1];
			descriptors[0] = "Default";
			for(int i = 0; i< eventEditors.Count; i++)
				descriptors[i+1] = eventEditors[i].EventName;
			return descriptors;
		}
	}

	public override EventEditor createEventEditorFor (string eventName)
	{
		if (eventName.ToLower () == "default")
			return defaultEventEditor;

		foreach (EventEditor eventEditor in eventEditors) {
			if(eventEditor.EventName.ToLower() == eventName.ToLower()){
				return eventEditor.clone();
			}
		}
		return eventEditors[0];
	}
}