using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Teleporter))]
public class TeleporterEditor : Editor {
	
	void OnEnable(){

		
	}
	
	//int selectedTexture;
	public override void OnInspectorGUI(){
		
		EditorGUI.BeginChangeCheck ();
		
		serializedObject.Update();

		SerializedProperty cell = serializedObject.FindProperty("destination");
		SerializedProperty modep = serializedObject.FindProperty("mode");
		SerializedProperty gep = serializedObject.FindProperty("ge");
		SerializedProperty checkablep = serializedObject.FindProperty("checkable");

		EditorGUILayout.PropertyField(cell);

		modep.intValue = GUILayout.Toolbar(modep.intValue, new string[]{"Always", "Event trigger", "Checkable"});

		switch(modep.intValue){
		
		case 1:{
			GameEvent ge = gep.objectReferenceValue as GameEvent;
			if(ge == null)
				ge = ScriptableObject.CreateInstance<GameEvent>();

			string[] editors = EventEditorFactory.Intance.CurrentEventEditors;
			int editorSelected = 0;
			if(ge.Name == null)
				ge.Name = "";
			for (int i = 1; i< editors.Length; i++)
				if (editors [i].ToLower () == ge.Name.ToLower ())
					editorSelected = i;
			int was = editorSelected;
			
			editorSelected = EditorGUILayout.Popup (editorSelected, EventEditorFactory.Intance.CurrentEventEditors);
			if(was != editorSelected && editorSelected == 0)
				ge.Name = "";
			EventEditor editor = EventEditorFactory.Intance.createEventEditorFor (editors[editorSelected]);
			editor.useEvent (ge);		
			
			editor.draw ();

			gep.objectReferenceValue = editor.Result;

			}
		break;
			case 2: {

			Checkable c = (Checkable) checkablep.objectReferenceValue;
			string[] editors = ForkEditorFactory.Intance.CurrentForkEditors;
			int editorSelected = EditorGUILayout.Popup (
				ForkEditorFactory.Intance.ForkEditorIndex(c),
				ForkEditorFactory.Intance.CurrentForkEditors
				);
			
			ForkEditor editor = ForkEditorFactory.Intance.createForkEditorFor (editors[editorSelected]);
			
			editor.useFork (c);		
			
			editor.draw ();
			
			checkablep.objectReferenceValue = editor.Result;

			}break;
		}

		
		if(EditorGUI.EndChangeCheck())
		{			

		}
		
		serializedObject.ApplyModifiedProperties ();
		
	}


	/*
	void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		

		
	}*/
}