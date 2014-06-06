using UnityEngine;
using UnityEditor;
using System.Collections;

public class DecorateModule : MapEditorModule {
	
	public string Name {get{return "Decorate";}}
	public int Order {get{return 3;}}

	private Map map;
	private Tool selected;

	// InspectorGUI vars
	private Vector2 scroll;
	private bool parallelDecoration;
	private bool rotateDecoration;
	private IsoDecoration paintingIsoDecoration;

	// SceneGUI vars


	private GUIStyle titleStyle;
	
	public DecorateModule(){
		titleStyle = new GUIStyle();
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.margin = new RectOffset(0,0,5,5);
	}
	
	public void useMap(Map map){
		this.map = map;
	}

	private bool loaded = false;
	public void OnEnable(){
		selected = Tools.current;
		Tools.current = Tool.None;

		if(!loaded){
			string[] paths = AssetDatabase.GetAllAssetPaths();
			foreach(string path in paths)
				AssetDatabase.LoadAssetAtPath(path, typeof(IsoDecoration));
			loaded = true;
		}
	}
	
	public void OnDisable(){
		Tools.current = selected;
		map.removeGhost();
	}

	public void OnInspectorGUI(){

		EditorGUILayout.HelpBox("Press left button to put the textures over the faces of the cell. Hold shift and press left button to copy the current texture of the hovering face.", MessageType.None);
		EditorGUILayout.Space();

		parallelDecoration = EditorGUILayout.Toggle("Draw Parallel",parallelDecoration);
		EditorGUILayout.Space();
		
		rotateDecoration = EditorGUILayout.Toggle("Rotate to be Perpendicular",rotateDecoration);
		EditorGUILayout.Space();
		
		EditorGUILayout.PrefixLabel("Decoration Objects",GUIStyle.none, titleStyle);
		
		GUI.backgroundColor = Color.Lerp(Color.black, Color.gray, 0.5f);
		
		EditorGUILayout.BeginVertical("Box");
		
		IsoDecoration[] isoDecorations = DecorationManager.getInstance().textureList();
		
		int maxTextures = 8;
		float anchoTextura = (Screen.width - 30) / maxTextures;
		
		if(isoDecorations.Length > maxTextures*4)
			scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(anchoTextura));

		Event e = Event.current;

		int currentTexture = 0;
		foreach(IsoDecoration it in isoDecorations){
			if(it.getTexture() == null)
				continue;
			
			if(currentTexture == 0)	EditorGUILayout.BeginHorizontal();
			
			Rect auxRect = GUILayoutUtility.GetRect(anchoTextura,anchoTextura);
			Rect border = new Rect(auxRect);
			auxRect.x+=2;auxRect.y+=2;auxRect.width-=4;auxRect.height-=4;
			
			if (e.isMouse && border.Contains(e.mousePosition)) 	{ 
				if(e.type == EventType.mouseDown){
					paintingIsoDecoration = it;
					this.Repaint = true;
				}
			}
			
			if(it == paintingIsoDecoration)
				EditorGUI.DrawRect(border,Color.yellow);
			GUI.DrawTexture(auxRect,it.getTexture());
			
			currentTexture++;
			if(currentTexture == maxTextures){EditorGUILayout.EndHorizontal(); currentTexture = 0;}
			
		}
		
		if(currentTexture != 0){
			GUILayoutUtility.GetRect((maxTextures - currentTexture)*anchoTextura,anchoTextura);
			EditorGUILayout.EndHorizontal();
		}
		
		if(isoDecorations.Length > maxTextures*4)
			EditorGUILayout.EndScrollView();

		
		EditorGUILayout.EndVertical();
	}


	public void OnSceneGUI(SceneView scene){

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		RaycastHit info = new RaycastHit();

		GameObject selected = null;
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		if(Physics.Raycast(ray, out info/*, LayerMask.NameToLayer("Cells Layer")*/)) //TODO Arreglar esto porque al parecer la distancia no funciona correctamente
			if(info.collider.transform.IsChildOf(this.map.transform))
				selected = info.collider.gameObject;

		/** 
		 * Mouse Events of painting mode 
		 */

		bool decorateLater = false;
		if(Event.current.isMouse && Event.current.button == 0 && selected != null && Event.current.type == EventType.MouseUp)
			decorateLater = true;

		/**
		 * Working zone
		 */
		
		if(selected != null){
			Cell cs = selected.GetComponent<Cell>();
			if(cs != null){
				Face f = (cs!=null)?cs.getFaceByPoint(info.point):null;
				if(f!=null){
					Vector3 position = selected.transform.position;
					if(cs != null)
						position.y = cs.Height;
					
					Vector3[] vertex = f.SharedVertex;
					int[] indexes = f.VertexIndex;
					
					Vector3[] puntos = new Vector3[5];
					for(int i = 0; i< indexes.Length; i++)
						puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);
					if(indexes.Length == 3)//Triangular faces
						puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);
					puntos[puntos.Length-1] = puntos[0]; // Closes the line
					Handles.DrawPolyLine(puntos);
					
					Vector2 directions = new Vector2(puntos[1].x-puntos[0].x, puntos[2].y-puntos[1].y);
					int ang = 0;
					if(directions.x == 0f)
						if(directions.y == 0f)	ang = 0;
						else					ang = 2;
					else ang = 1;
					
					if(paintingIsoDecoration != null){
						Debug.Log ("Decoration");
						if(decorateLater){
							cs.addDecoration(info.point, ang, rotateDecoration, parallelDecoration, (Event.current.shift)?false:true, paintingIsoDecoration);
							cs.refresh();
						}else
							map.ghostDecoration(cs, info.point, ang, rotateDecoration, parallelDecoration, (Event.current.shift)?false:true, paintingIsoDecoration, 0.5f);
					}
				}
			}
		}
	}
	
	public void OnDestroy(){}

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
