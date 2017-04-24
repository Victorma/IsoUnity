using UnityEngine;
using UnityEditor;
using System.Collections;

namespace IsoUnity.Sequences {
	public class DefaultEventEditor : EventEditor {
		
		private SerializableGameEvent ge;
		public DefaultEventEditor() {
		}
		public SerializableGameEvent Result { 
			get{ return ge; }
		}
		public string EventName {
			get{ return ""; }
		}
		public EventEditor clone(){
	        return new DefaultEventEditor();
		}

		public void useEvent(SerializableGameEvent ge){
			this.ge = ge;
		}

		public void detachEvent(SerializableGameEvent ge){}
		
		private string newParameter;
		public void draw(){
			
			ge.Name = EditorGUILayout.TextField ("Name", ge.Name);
			
			foreach (string param in ge.Params) {
	            if (param != "synchronous")
	            {
	                EditorGUILayout.BeginHorizontal();

	                ge.setParameter(param, ParamEditor.LayoutEditorFor(param, ge.getParameter(param)));
	                //ge.setParameter (param, EditorGUILayout.ObjectField (param, (Object)ge.getParameter (param), typeof(Object), true));
	                if (GUILayout.Button("X"))
	                    ge.removeParameter(param);
	                EditorGUILayout.EndHorizontal();
	            }
			}
			EditorGUILayout.BeginHorizontal ();
			newParameter = EditorGUILayout.TextField ("New Parameter", newParameter);
			if (GUILayout.Button ("Add"))
				ge.setParameter (newParameter, null);
			EditorGUILayout.EndHorizontal ();
		}


	}
}