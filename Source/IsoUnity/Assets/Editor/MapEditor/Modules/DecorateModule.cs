using UnityEngine;
using UnityEditor;
using System.Collections;

public class DecorateModule : MapEditorModule {
	
	public string Name {get{return "Decorate";}}
	public int Order {get{return 3;}}

	/**
	 * Lines to start scrolling
	 */
	public int LTSS = 4;

	private Map map;
	private Tool selected;

	// InspectorGUI vars
	private Vector2 scroll;
	private bool parallelDecoration;
	private IsoDecoration paintingIsoDecoration;

	// SceneGUI vars

	private GUIStyle titleStyle;

	//AutoAnimator Vars;
	private bool autoanimate = false;
	private int[] FrameSecuence;
	private float FrameRate;
	
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

		GUIStyle style = new GUIStyle();

		EditorGUILayout.HelpBox("Press left button to put the textures over the faces of the cell. Hold shift and press left button to copy the current texture of the hovering face.", MessageType.None);
		EditorGUILayout.Space();

		parallelDecoration = EditorGUILayout.Toggle("Draw Parallel",parallelDecoration);
		EditorGUILayout.Space();

		autoanimate = EditorGUILayout.Toggle ("Auto Animate", autoanimate);

		if (autoanimate) {
			FrameRate = float.Parse(EditorGUILayout.TextField("Frame Rate:", FrameRate.ToString()));

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Frame Secuence:",GUIStyle.none, titleStyle);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.HelpBox("Let the frame secuence list empty if you want to use the default frame loop.", MessageType.None);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical();

			GUIContent btt = new GUIContent("Add Frame");
			Rect btr = GUILayoutUtility.GetRect(btt, style);		
			if(GUI.Button(btr,btt)){
				if(FrameSecuence==null) FrameSecuence = new int[0];
				int [] tmpFrameSecuence = new int[FrameSecuence.Length+1]; 
				for(int i=0; i<FrameSecuence.Length; i++) tmpFrameSecuence[i] = FrameSecuence[i];
				tmpFrameSecuence[tmpFrameSecuence.Length-1] = 0;
				FrameSecuence=tmpFrameSecuence;
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			btt = new GUIContent("Remove Frame");
			btr = GUILayoutUtility.GetRect(btt, style);		
			if(GUI.Button(btr,btt)){
				if(FrameSecuence.Length>0){
					if(FrameSecuence==null) FrameSecuence = new int[0];
					int [] tmpFrameSecuence = new int[FrameSecuence.Length-1]; 
					for(int i=0; i<FrameSecuence.Length-1; i++) tmpFrameSecuence[i] = FrameSecuence[i];
					FrameSecuence=tmpFrameSecuence;
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			if(FrameSecuence!=null)
			for(int i=0; i<FrameSecuence.Length; i++)
				FrameSecuence[i] = int.Parse(EditorGUILayout.TextField(i.ToString(), FrameSecuence[i].ToString()));
		}
		EditorGUILayout.Space();
		
		EditorGUILayout.PrefixLabel("Decoration Objects",GUIStyle.none, titleStyle);
		
		GUI.backgroundColor = Color.Lerp(Color.black, Color.gray, 0.5f);
		
		EditorGUILayout.BeginVertical("Box");
		
		IsoDecoration[] isoDecorations = DecorationManager.getInstance().textureList();
		
		int maxTextures = 8;
		float anchoTextura = (Screen.width - 30) / maxTextures;
	
		if(isoDecorations.Length > maxTextures*LTSS)
			scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(anchoTextura*LTSS));

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
		
		if(isoDecorations.Length > maxTextures*LTSS)
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
                FaceNoSC f = (cs != null) ? cs.getFaceByPoint(info.point) : null;
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
						if(decorateLater){
							GameObject tmpdec = cs.addDecoration(info.point, ang, parallelDecoration, (Event.current.shift)?false:true, paintingIsoDecoration);

							if(autoanimate){
								AutoAnimator tmpautoanim = tmpdec.AddComponent<AutoAnimator>() as AutoAnimator;

								tmpautoanim.FrameSecuence = this.FrameSecuence;
								tmpautoanim.FrameRate = this.FrameRate;
							}

							cs.refresh();
						}else
							map.ghostDecoration(cs, info.point, ang, parallelDecoration, (Event.current.shift)?false:true, paintingIsoDecoration, 0.5f);
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
