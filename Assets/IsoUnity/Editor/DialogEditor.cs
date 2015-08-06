using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Dialog))]
public class DialogEditor : Editor {
		public DialogEditor ()
		{
		}

	GUIStyle normalStyle;
	GUIStyle toolBarStyle;
	GUIStyle pressedStyle;
	GUIStyle infoStyle;
	GUIStyle titleStyle;

	private Dialog dialog;
	
	void OnEnable()
	{
		//SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		this.dialog = (Dialog)target;
		
		normalStyle = new GUIStyle();
		normalStyle.padding = new RectOffset(5,5,5,5);
		
		pressedStyle = new GUIStyle();
		pressedStyle.padding = new RectOffset(5,5,5,5);
		GUIStyleState fo = new GUIStyleState();
		
		Texture2D lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
		lineTex.SetPixel(0, 1, Color.Lerp(Color.black, Color.gray, 0.5f));
		lineTex.Apply();
		
		fo.textColor = Color.white;
		
		fo.background = lineTex;
		
		pressedStyle.fontStyle = FontStyle.Bold;
		pressedStyle.alignment = TextAnchor.MiddleCenter;
		pressedStyle.normal = fo;
		
		toolBarStyle = new GUIStyle();
		toolBarStyle.margin = new RectOffset(50,50,5,10);
		
		infoStyle = new GUIStyle();
		infoStyle.margin = new RectOffset(10,10,0,10);
		
		titleStyle = new GUIStyle();
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.margin = new RectOffset(0,0,5,5);
	}

	public override void OnInspectorGUI(){
		
		dialog = target as Dialog;
		
		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(5,5,5,5);

		
		// Textura
		if(dialog != null){
			dialog.id = UnityEditor.EditorGUILayout.TextField("Name", dialog.id);
			Dialog.Fragment[] fragments = dialog.getFragments();
			Dialog.DialogOption[] options = dialog.getOptions();

			EditorGUILayout.HelpBox("You have to add at least one", MessageType.None);
			if(fragments != null){
				foreach(Dialog.Fragment frg in fragments){
					EditorGUILayout.BeginHorizontal();
					frg.Face = EditorGUILayout.ObjectField(frg.Face, typeof(Texture2D), true, GUILayout.Width(60),GUILayout.Height(60)) as Texture2D;
					EditorGUILayout.BeginVertical();
					frg.Name = EditorGUILayout.TextField(frg.Name);
					frg.Msg = EditorGUILayout.TextArea(frg.Msg,GUILayout.Height(40));
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();

					/*
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Reset: ", GUILayout.Width(40));
					frg.reset = EditorGUILayout.Toggle(frg.reset);
					EditorGUILayout.EndHorizontal();
					*/

					GUIContent btt = new GUIContent("Remove");
					Rect btr = GUILayoutUtility.GetRect(btt, style);		
					if(GUI.Button(btr,btt)){
						dialog.removeFragment(frg);
					};
					EditorGUILayout.EndVertical();

					EditorGUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EndHorizontal();

			GUIContent bttext = new GUIContent("Add Fragment");
			Rect btrect = GUILayoutUtility.GetRect(bttext, style);		
			if(GUI.Button(btrect,bttext)){
				dialog.addFragment();
			};

			EditorGUILayout.HelpBox("Options are the lines between you have to choose at the end of the dialog. Leave empty to do nothing, put one to execute this as the dialog ends, or put more than one to let the player choose between them.", MessageType.None);
			if(options != null){
				foreach(Dialog.DialogOption opt in options){
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Tag: ", GUILayout.Width(27));
					opt.tag = EditorGUILayout.TextField(opt.tag);
					EditorGUILayout.LabelField("Text: ", GUILayout.Width(35));
					opt.text = EditorGUILayout.TextField(opt.text);
					GUIContent btt = new GUIContent("Remove");
					Rect btr = GUILayoutUtility.GetRect(btt, style);		
					if(GUI.Button(btr,btt)){
						dialog.removeOption(opt);
					};
					EditorGUILayout.EndHorizontal();
				}
			}

			bttext = new GUIContent("Add Option");
			btrect = GUILayoutUtility.GetRect(bttext, style);		
			if(GUI.Button(btrect,bttext)){
				dialog.addOption();
			};


		}else{
			EditorGUILayout.LabelField("Please select a texture o create a new one!", style);
		}
		
	}
}
