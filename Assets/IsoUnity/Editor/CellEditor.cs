using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(Cell))]
public class CellEditor : Editor {

	// Targets
	private Cell[] cell;
		
	// Cell properties
    private Quaternion angle = new Quaternion();

	private float heightValue;
    private bool justCreated = true;

	void OnEnable(){

		SceneView.onSceneGUIDelegate += this.OnSceneGUI2;
		//Getting the targets
		ArrayList acell = new ArrayList();
        if (targets != null)
            acell.AddRange(targets);
        else
            acell.Add(target);
		cell = acell.ToArray(typeof(Cell)) as Cell[];

		// Base Properties
        angle.SetFromToRotation(new Vector3(0, 0, 1f), new Vector3(0, 1f, 0));

	}

	void OnDestroy(){
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI2;
	}
	[SerializeField]
	Cell[] vec;

	//int selectedTexture;
	public override void OnInspectorGUI(){

		EditorGUI.BeginChangeCheck ();

		serializedObject.Update();

		SerializedProperty height = serializedObject.FindProperty("properties.height");
        SerializedProperty walkable = serializedObject.FindProperty("walkable");
        SerializedProperty cellTopType = serializedObject.FindProperty("properties.top");
        SerializedProperty cellTopRotation = serializedObject.FindProperty("properties.orientation");

		EditorGUI.showMixedValue = walkable.hasMultipleDifferentValues;
		EditorGUILayout.PropertyField (walkable);


		heightValue = height.floatValue;
		EditorGUI.showMixedValue = height.hasMultipleDifferentValues;
		heightValue = EditorGUILayout.FloatField("Height", heightValue);

		CellTopType topType = (CellTopType)cellTopType.enumValueIndex;

		EditorGUI.showMixedValue = cellTopType.hasMultipleDifferentValues;
		topType = (CellTopType)EditorGUILayout.EnumPopup("Top", topType);

		GUIStyle s = new GUIStyle();
		s.padding = new RectOffset(5,5,5,5);

		int topRotation = cellTopRotation.intValue;

		GUIContent topRotationText = new GUIContent("Top Rotation: " + ((cellTopRotation.hasMultipleDifferentValues)?"Mixed":""+topRotation*90));
		if (GUI.Button (GUILayoutUtility.GetRect(topRotationText, s), topRotationText)) topRotation +=1;


		GUIContent select = new GUIContent("Select Map");
		if (GUI.Button (GUILayoutUtility.GetRect(select, s), select)) selectMap();

		if(EditorGUI.EndChangeCheck())
		{
			if(heightValue != height.floatValue){
                foreach (Cell c in cell)
                    c.Height = heightValue;
			}
			if(topType != ((CellTopType)cellTopType.enumValueIndex)){
				foreach(Cell c in cell)
					c.CellTop = topType;
				cellTopType = serializedObject.FindProperty("cellTop");
			}

			if(topRotation != cellTopRotation.intValue){
				foreach(Cell c in cell)
					c.CellTopRotation = topRotation;
				cellTopRotation = serializedObject.FindProperty("cellTopRotation");
			}
            foreach (Cell c in cell)
                EditorUtility.SetDirty(c);
		}

		serializedObject.ApplyModifiedProperties ();

        /*vec = cell[0].Map.getNeightbours(cell[0]);
        foreach (Cell ce in vec)
            EditorGUILayout.ObjectField(ce, typeof(Cell), true);*/

	}

    Vector3 ident = new Vector3(0, 0, 0);
    Vector3 diference = new Vector3(0,0,0);
    Vector3 origin;
    Dictionary<Cell, float> heights = new Dictionary<Cell, float>();

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

        Cell c = (Cell) target;

        if (target == cell[0])
        { 
            Bounds bounds = new Bounds(cell[0].GetComponent<Renderer>().bounds.center,new Vector3(0,0,0));
            for(int i = 0 ; i<cell.Length; i++)
                bounds.Encapsulate(cell[i].GetComponent<Renderer>().bounds);

            MyHandles.DragHandleResult dhResult;


            Vector3 screenStart = SceneView.currentDrawingSceneView.camera.WorldToScreenPoint(bounds.center);
            Vector3 end = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(screenStart + new Vector3(0, 120, 0));

            float height = (end -  bounds.center).magnitude;

            end = bounds.center + new Vector3(0, height, 0);

            Handles.DrawLine(bounds.center, end);

            Vector3 newPosition = MyHandles.DragHandle(end, angle, 0.1f*height, Handles.CubeCap, Color.white, Color.gray, out dhResult);
            
            switch (dhResult)
            {
                case MyHandles.DragHandleResult.LMBPress:
                    heights.Clear();    
                    origin = newPosition;
                    break;
                case MyHandles.DragHandleResult.LMBDrag:
                    if (justCreated)
                    {
                        heights.Clear();
                        origin = newPosition;
                        justCreated = false;
                    }

                    diference = newPosition - origin;
                    break;
                case MyHandles.DragHandleResult.LMBRelease:
                    diference = ident;
                    break;
            }

        }

        if (!heights.ContainsKey(c))
            heights.Add(c, c.Height);

        if (diference.y != 0)
        {
            Undo.RecordObject(target, "Cell height changed...");
            float height = c.Height;
            c.Height = heights[c] + diference.y;
            EditorUtility.SetDirty(c);

            if (c.Height != height)
                c.gameObject.BroadcastMessage("Update");
        }


	}

    bool hasMultipleDifferentValues(System.Object[] o, string propertyName)
    {
        System.Reflection.PropertyInfo property = o[0].GetType().GetProperty(propertyName);
        object value = property.GetValue(o[0],null);
        int i = 0;
        while (i < o.Length && value == property.GetValue(o[i], null)) i++;

        return i != o.Length;
    }

	private void selectMap(){

        if (!hasMultipleDifferentValues(cell, "Map"))
        {
            int m = cell[0].transform.parent.GetInstanceID();
            Selection.activeInstanceID = m;
        }

	}

	void OnSceneGUI2 (SceneView sceneView){

		if(Event.current.button == 0 )
			if(Event.current.type == EventType.DragUpdated){				
				for(int i = 0; i<cell.Length; i++){
					if(cell[i].Map != null)
						cell[i].Map.updateCell(cell[i], Event.current.type == EventType.MouseUp);
				}
				sceneView.Repaint();
				
			}

	}

}
