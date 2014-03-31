using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Talker))]
public class TalkerEditor : Editor {
	
	void OnEnable(){
	}

	SecuenceWindow editor = null;
	public override void OnInspectorGUI(){

		if(GUILayout.Button("Open editor")){
			if(editor == null){
				editor = EditorWindow.GetWindow<SecuenceWindow>();
				editor.Secuence = (target as Talker).Secuence;
			}
		}
		if (GUILayout.Button ("Close editor")) {
			if(editor!=null)
				editor.Close();
		}

	}


}
