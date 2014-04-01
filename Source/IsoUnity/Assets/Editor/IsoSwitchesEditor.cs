using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(IsoSwitches))]
public class IsoSwitchesEditor : Editor{

	private Vector2 scrollposition = new Vector2(0,0);

	IsoSwitches isoSwitches;
	public void OnEnable(){
		isoSwitches = target as IsoSwitches;
		
	}
	
	
	public override void OnInspectorGUI(){

		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(5,5,5,5);

		isoSwitches = target as IsoSwitches;
		
		EditorGUILayout.HelpBox("List of switches that represent the state of the game.", MessageType.None);

		IsoSwitches.ISwitch[] switches = isoSwitches.getList ();
		if(switches != null){
			int i = 0;
			scrollposition = EditorGUILayout.BeginScrollView(scrollposition, GUILayout.ExpandHeight(true));
			foreach(IsoSwitches.ISwitch isw in switches){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("ID: ", GUILayout.Width(27));
				isw.id = EditorGUILayout.TextField(isw.id);
				isw.state = EditorGUILayout.Toggle("Initial State: ", isw.state);
				GUIContent btt = new GUIContent("Remove");
				Rect btr = GUILayoutUtility.GetRect(btt, style);		
				if(GUI.Button(btr,btt)){
					isoSwitches.removeSwitch(isw);
				};
				EditorGUILayout.EndHorizontal();
				i++;
			}
			EditorGUILayout.EndScrollView();
		}

		EditorGUILayout.BeginHorizontal();
		GUIContent bttext = new GUIContent("Add Switch");
		Rect btrect = GUILayoutUtility.GetRect(bttext, style);		
		if(GUI.Button(btrect,bttext)){
			isoSwitches.addSwitch();
		};
		EditorGUILayout.EndHorizontal();
	}
}