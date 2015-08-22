using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditModule : MapEditorModule {
	
	public string Name {get{return "Edit";}}
	public int Order {get{return 1;}}

	private const float movingInterval = 30;
	private const int cellSize = 1;

	private Map map;
	private Tool selected;

	// InspectorGUI vars
	private bool setPerspective;
	private bool fixView = true;
	private float gridHeight;
	private Quaternion fixedRotation;

	// SceneGUI vars
	private bool creating = false;
	private bool movingGrid = false;
	private float startingMousePosition;
	private float startingGridHeight;


	private void backUpAngle(){
		fixedRotation = SceneView.lastActiveSceneView.camera.transform.rotation;
	}

	private void startMovingGrid(){
		movingGrid = true;
		startingMousePosition = Event.current.mousePosition.y;
		startingGridHeight = gridHeight;
	}
	
	private void endMovingGrid(){
		movingGrid = false;
	}

	private void moveGrid(){
		if(movingGrid){
			float mouseDifference = startingMousePosition - Event.current.mousePosition.y;
			int movingTimes = Mathf.FloorToInt(mouseDifference / movingInterval);
			
			gridHeight = startingGridHeight + movingTimes;
			if(gridHeight < 0)
				gridHeight = 0;
		}
	}

	private void startCreatingCells(){
		creating = true;
		createCell();
	}
	
	private void endCreatingCells(){
		creating = false;
	}
	
	private void createCell(){
        if (creating)
        {
            map.addCell(map.getMousePositionOverMap(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), gridHeight));
            EditorUtility.SetDirty(map);
        }
	}

	
	public void useMap(Map map){
		this.map = map;
	}

	public void OnEnable(){
		selected = Tools.current;
		Tools.current = Tool.None;
        backUpAngle();
	}

	public void OnDisable(){
		Tools.current = selected;
		map.removeGhost();
	}

	public void OnInspectorGUI(){

		EditorGUILayout.HelpBox("Press left button to create cells. Press left button and slide up and down to increase/decrease cell height.", MessageType.None);
		EditorGUILayout.Space();
		
		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(2,2,2,2);

		GUILayout.Space(5f);

		// Fix view
		bool lastFixView = fixView;
		fixView = EditorGUILayout.Toggle("Fix perspective",fixView);
		if(lastFixView == false && fixView == true)
			backUpAngle();
		
		// Grid Height
		gridHeight = EditorGUILayout.FloatField("Grid Height", gridHeight);

	}

	public void OnSceneGUI(SceneView scene){
		
		if(fixView)
			scene.LookAtDirect(scene.pivot, fixedRotation);
		
		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		if(Event.current.isMouse){
			if(Event.current.button == 0){
				if(Event.current.type == EventType.MouseDown)		startCreatingCells();
				else if(Event.current.type == EventType.MouseUp)	endCreatingCells();
				else												createCell();
			}
			else if(Event.current.button == 1){
				if(Event.current.type == EventType.MouseDown)		startMovingGrid();
				else if(Event.current.type == EventType.MouseUp)	endMovingGrid();
				else												moveGrid();
			}
			scene.Repaint();
		}

		Vector3 centerGridPoint = map.getMousePositionOverMap(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.RoundToInt(gridHeight*2f)/2f);
		
		map.ghostCell(centerGridPoint, 0.5f);
		
		Vector3[] puntos = new Vector3[4];
		puntos[0] = new Vector3(centerGridPoint.x - cellSize/2.0f,centerGridPoint.y,centerGridPoint.z - cellSize/2.0f);
		puntos[1] = new Vector3(centerGridPoint.x - cellSize/2.0f,centerGridPoint.y,centerGridPoint.z + cellSize/2.0f);
		puntos[2] = new Vector3(centerGridPoint.x + cellSize/2.0f,centerGridPoint.y,centerGridPoint.z + cellSize/2.0f);
		puntos[3] = new Vector3(centerGridPoint.x + cellSize/2.0f,centerGridPoint.y,centerGridPoint.z - cellSize/2.0f);
		Handles.DrawSolidRectangleWithOutline(puntos, Color.yellow, Color.white);
	}

	public void OnDestroy(){

	}

	private bool repaint;
	public bool Repaint {
		get{
			return this.repaint;
		}
		set{
			this.repaint = value;
		}
	}
	
}
