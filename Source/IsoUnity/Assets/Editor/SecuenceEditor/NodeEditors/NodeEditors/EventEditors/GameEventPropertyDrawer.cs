using UnityEngine;
using UnityEditor;	
using System.Collections;

public class GameEventPropertyDrawer : PropertyDrawer {

	int selected = 0;
	void OnGUI(Rect position, SerializedProperty property, GUIContent label){

		EditorGUI.BeginProperty (position, label, property);

			GameEvent ge = (GameEvent)property.objectReferenceValue;
			string[] editors = EventEditorFactory.Intance.CurrentEventEditors;
			for (int i = 0; i< editors.Length; i++)
					if (editors [i].ToLower () == ge.Name.ToLower ())
							selected = i;
				
			selected = EditorGUILayout.Popup (selected, EventEditorFactory.Intance.CurrentEventEditors);
			
			ge.Name = editors[selected];
			
			EventEditor editor = EventEditorFactory.Intance.createEventEditorFor (ge.Name);
			editor.useEvent (ge);		
			
			editor.draw ();
			
			ge = editor.Result;


		EditorGUI.EndProperty ();

	}
}
