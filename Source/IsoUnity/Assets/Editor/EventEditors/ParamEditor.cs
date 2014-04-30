using UnityEngine;
using UnityEditor;
using System.Collections;

public class ParamEditor {

	private static System.Type[] tipos = new System.Type[]{
		typeof(Object),
		typeof(int),
		typeof(bool),
		typeof(float),
		typeof(string)
	};

	public static object editorFor(string label, object v){

		EditorGUILayout.BeginHorizontal();

		int pretipo = 0;
		if(v!=null)
			for(int i = 0; i< tipos.Length; i++)
				if(v.GetType() == tipos[i]){
					pretipo = i;
					break;
				}
		string[] nombres = new string[tipos.Length];
		for(int i = 0; i< tipos.Length; i++)
			nombres[i] = tipos[i].ToString();

		int tipo = EditorGUILayout.Popup(pretipo, nombres);

		object returnable = v;

		if(pretipo != tipo){
			switch(tipo){
				case 0: returnable = null; break;
				case 1: returnable = 0; break;
				case 2: returnable = false; break;
				case 3: returnable = 0f; break;
				case 4: returnable = ""; break;
			}
		}

		switch(tipo){
			case 0:
			Object miTipo = EditorGUILayout.ObjectField((Object)returnable,typeof(MonoScript),false);
			returnable = EditorGUILayout.ObjectField(label, (Object)returnable, (miTipo == null)?typeof(Object):miTipo.GetType(), true);
			break;
			case 1: returnable = EditorGUILayout.IntField(label,(int)returnable); break;
			case 2: returnable = EditorGUILayout.Toggle(label,(bool)returnable); break;
			case 3: returnable = EditorGUILayout.FloatField(label,(float)returnable); break;
			case 4: returnable = EditorGUILayout.TextField(label,(string)returnable); break;
		}



		EditorGUILayout.EndHorizontal();

		return returnable;
	}
}
