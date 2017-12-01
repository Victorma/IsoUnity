using UnityEngine;
using UnityEditor;
using System.Collections;
using IsoUnity.Entities;

namespace IsoUnity.Sequences {
	public class AddItemEditor : EventEditor {

		private SerializableGameEvent ge;
		public AddItemEditor() {
		}
		
		public SerializableGameEvent Result { 
			get{ return ge; }
		}
		public string EventName {
			get{ return "Add Item"; }
		}
		public EventEditor clone(){
			return new AddItemEditor();
		}
		
		public void useEvent(SerializableGameEvent ge){
			this.ge = ge;
			this.ge.Name = this.EventName;
		}
		
		public void draw(){
			
			ge.setParameter("item", EditorGUILayout.ObjectField("Item", (Object)ge.getParameter("item"), typeof(Item), true));
			ge.setParameter("inventory", EditorGUILayout.ObjectField("Inventory", (Object)ge.getParameter("inventory"), typeof(Inventory), true));
		}

		public void detachEvent(SerializableGameEvent ge)
	    {
	        if (ge.getParameter("item") == null)       ge.removeParameter("item");
	        if (ge.getParameter("inventory") == null)  ge.removeParameter("inventory");
	    }
	}
}