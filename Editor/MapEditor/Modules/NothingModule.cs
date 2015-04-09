using UnityEngine;
using UnityEditor;
using System.Collections;

public class NothingModule : MapEditorModule {
	
	public string Name {
		get{
			return "Nothing";
		}
	}
	public int Order {get{return 0;}}

	public void useMap(Map map){}
	
	public void OnEnable(){}
	public void OnDisable(){}
	public void OnInspectorGUI(){}
	public void OnSceneGUI(SceneView scene){}
	public void OnDestroy(){}
	
	public bool Repaint {
		get{return false;}
		set{}
	}
}
