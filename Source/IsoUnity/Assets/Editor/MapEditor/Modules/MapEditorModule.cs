using UnityEngine;
using UnityEditor;
using System.Collections;

public interface MapEditorModule {

	string Name {get;}
	bool Repaint {get; set;}
	int Order {get;}

	void useMap(Map map);

	void OnEnable();
	void OnDisable();
	void OnInspectorGUI();
	void OnSceneGUI(SceneView scene);
	void OnDestroy();

}
