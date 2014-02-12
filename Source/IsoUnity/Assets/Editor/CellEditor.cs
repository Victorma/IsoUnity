using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
//[CustomEditor(typeof(Cell))]
public class CellEditor : Editor {

	// Targets
	private Cell[] cell;
		
	// Cell properties

	private SerializedProperty map;

	private float heightValue;

	void OnEnable(){

		SceneView.onSceneGUIDelegate += this.OnSceneGUI2;

		//Getting the targets
		ArrayList acell = new ArrayList(targets);
		acell.Add(target);
		cell = acell.ToArray(typeof(Cell)) as Cell[];

		// Base Properties


		//cellTopType = serializedObject.FindProperty("cellTop");
		map = serializedObject.FindProperty("map");
		//height = cell[0].Height;
		//cellTopType = cell[0].CellTop;

		// Search for different properties
		//foreach(Cell c in cell){
		//	if(height != c.Height) height = -1;
		//	if(cellTopType != c.CellTop) cellTopType = CellTopType.undefined;
		//}

	}

	void OnDestroy(){
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI2;
	}
	
	//int selectedTexture;
	public override void OnInspectorGUI(){

		EditorGUI.BeginChangeCheck ();

		serializedObject.Update();

		SerializedProperty height = serializedObject.FindProperty("height");
		SerializedProperty cellTopType = serializedObject.FindProperty("cellTop");

		heightValue = height.floatValue;
		EditorGUI.showMixedValue = height.hasMultipleDifferentValues;
		heightValue = EditorGUILayout.FloatField("Height", heightValue);

		/*
		CellTopType last = cell[0].getTopType();
		if(last != cellTopType){
			foreach(Cell c in cell)
				c.setTop(cellTopType);
		}*/

		CellTopType topType = (CellTopType)cellTopType.enumValueIndex;

		EditorGUI.showMixedValue = cellTopType.hasMultipleDifferentValues;
		topType = (CellTopType)EditorGUILayout.EnumPopup("Top", topType);


		GUIStyle s = new GUIStyle();
		s.padding = new RectOffset(5,5,5,5);
		GUIContent select = new GUIContent("Select Map");
		if (GUI.Button (GUILayoutUtility.GetRect(select, s), select)) selectMap();

		if(EditorGUI.EndChangeCheck())
		{
			if(heightValue != height.floatValue){
				//height.floatValue = heightValue;
				foreach(Cell c in cell)
					c.Height = heightValue;
			}
			if(topType != ((CellTopType)cellTopType.enumValueIndex)){
				foreach(Cell c in cell)
					c.CellTop = topType;
				cellTopType = serializedObject.FindProperty("cellTop");
			}
		}

		serializedObject.ApplyModifiedProperties ();

	}

	void OnSceneGUI (){

		/*SceneView sceneView = SceneView.currentDrawingSceneView;
		if(Event.current.button == 0 )
			if(Event.current.rawType == EventType.MouseDrag){
				movingObject = true;
				
				for(int i = 0; i<cell.Length; i++){
					//if(Selection.Contains(c.gameObject));
					cell[i].Map.updateCell(cell[i], Event.current.type == EventType.MouseUp);
				}
				sceneView.Repaint();
				//Event.current.Use();
			}else if(Event.current.rawType == EventType.mouseMove){
				ratonMovido = true;
			}else if(Event.current.rawType == EventType.mouseMove && HandleUtility.niceMouseDelta > 0 && movingObject){
				
			}else if(Event.current.rawType == EventType.MouseUp){
				if(movingObject)
					movingObject = false;
			}*/

	}

	private void selectMap(){
		if(!this.map.hasMultipleDifferentValues){
			int m = map.objectReferenceInstanceIDValue;
			Selection.activeInstanceID = m;
		}

	}

	void OnSceneGUI2 (SceneView sceneView){

		if(Event.current.button == 0 )
			if(Event.current.rawType == EventType.used){				
				for(int i = 0; i<cell.Length; i++){
					if(cell[i].Map != null)
						cell[i].Map.updateCell(cell[i], Event.current.type == EventType.MouseUp);
				}
				sceneView.Repaint();
				
			}

	}

}
