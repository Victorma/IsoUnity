using UnityEngine;
using System.Collections.Generic;
using IsoUnity.Entities;

namespace IsoUnity.Sequences {
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

            var methods = AttributesUtil.GetMethodsWith<GameEventAttribute>(typeof(EventedEventManager), true);
            foreach (var m in methods)
            {
                this.eventEditors.Add(new AttributeEventEditor(new GameEventConfig(m.Key, m.Value)));
            }

            methods = AttributesUtil.GetMethodsWith<GameEventAttribute>(typeof(EventedEntityScript), true);
            foreach(var m in methods)
            {
                this.eventEditors.Add(new AttributeEventEditor(new GameEventConfig(m.Key, m.Value)));
            }

			/*this.eventEditors.Add (new ChangeSwitchEventEditor ());
			this.eventEditors.Add (new AddItemEditor ());
            this.eventEditors.Add(new MoveEventEditor());
            */

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
				return defaultEventEditor.clone();

			foreach (EventEditor eventEditor in eventEditors) {
				if(eventEditor.EventName.ToLower() == eventName.ToLower()){
					return eventEditor.clone();
				}
			}
			return eventEditors[0].clone();
		}
	}
}