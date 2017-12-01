using UnityEngine;
using UnityEditor;
using System.Collections;

namespace IsoUnity.Sequences {
	public class ChangeSwitchEventEditor : EventEditor {

		private SerializableGameEvent ge;
		public ChangeSwitchEventEditor() {
		}
		
		public SerializableGameEvent Result { 
			get{ return ge; }
		}
		public string EventName {
			get{ return "ChangeSwitch"; }
		}
		public EventEditor clone(){
			return new ChangeSwitchEventEditor();
		}
		
		public void useEvent(SerializableGameEvent ge){
			this.ge = ge;
			this.ge.Name = this.EventName;
			if (ge.getParameter ("switch") == null)
				ge.setParameter ("switch", "");
			if (ge.getParameter ("value") == null)
				ge.setParameter ("value", true);
		}
		
		public void draw(){

			ge.setParameter("switch", EditorGUILayout.TextField("SwitchID", (string) ge.getParameter("switch")));
			ge.setParameter("value", ParamEditor.LayoutEditorFor("Value", ge.getParameter("value")));
		}

		public void detachEvent(SerializableGameEvent ge)
	    {
	        if ((string)ge.getParameter("switch") == "")
	        {
	            ge.removeParameter("switch");
	            ge.removeParameter("value");
	        }
	    }
		
	}
}