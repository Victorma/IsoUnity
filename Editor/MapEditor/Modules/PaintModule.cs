using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PaintModule : MapEditorModule {
	
	public string Name {get{return "Paint";}}
	public int Order {get{return 2;}}

	/**
	 * Lines to start scrolling
	 */
	public int LTSS = 4;

	private Map map;
	private Tool selected;

	private Vector2 scroll;

	// InspectorGUI vars
	private Texture2D paintingTexture;
	private IsoTexture paintingIsoTexture;

	// SceneGUI vars
	private bool collectTexture;
	private bool backupTextures;
	private bool painting;
	

	private GUIStyle titleStyle;

	public PaintModule(){
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
				AssetDatabase.LoadAssetAtPath(path, typeof(IsoTexture));
			loaded = true;
		}
	}
	
	public void OnDisable(){
		Tools.current = selected;
	}

	public void OnInspectorGUI(){
		//paintingMode = EditorGUILayout.BeginToggleGroup("Painting mode",paintingMode);
		EditorGUILayout.HelpBox("Press left button to put the textures over the faces of the cell. Hold shift and press left button to copy the current texture of the hovering face.", MessageType.None);
		EditorGUILayout.Space();

		EditorGUILayout.PrefixLabel("Texture", GUIStyle.none, titleStyle);

		EditorGUILayout.BeginHorizontal();
		
			paintingTexture = UnityEditor.EditorGUILayout.ObjectField("Tile", paintingTexture, typeof(Texture2D), false, GUILayout.MaxHeight(16)) as Texture2D;
			
			if(paintingTexture != null){
				IsoTexture[] textures = TextureManager.getInstance ().textureList (paintingTexture);
				List<string> texts = new List<string> ();

				int isoTextureIndex = textures.Length;
				for (int i = 0; i< textures.Length; i++){
					texts.Add (textures[i].name);
					if(textures[i] == paintingIsoTexture)
						isoTextureIndex = i;
				}
				
				texts.Add("None");
				//TODO CAMBIOS EN LA LISTA DEBERIAN DESELEECIONAR EL ELEMENTO ACTUAL SI ESTE YA NO ESTA EN LA LISTA
				isoTextureIndex = UnityEditor.EditorGUILayout.Popup(isoTextureIndex,texts.ToArray());
				if(isoTextureIndex == textures.Length)		paintingIsoTexture = null;
				else										paintingIsoTexture = textures[isoTextureIndex];

		}
		EditorGUILayout.EndHorizontal();
		
		GUI.backgroundColor = Color.Lerp(Color.black, Color.gray, 0.5f);


		// TODO podria hacerse una clase para este selector y asi reutilizarlo
		Event e = Event.current;
		EditorGUILayout.BeginVertical("Box");
		
			IsoTexture[] isoTextures = TextureManager.getInstance().textureList();
			
			int maxTextures = 8;
			float anchoTextura = (Screen.width - 30) / maxTextures;
			
			// This line controls the case when there are too many textures. In that case, 
			// the line after the if creates a scroll to fix the size to a max, defined in
			// GUILayout.MaxHeight(<<designed size>>)
			if(isoTextures.Length > maxTextures*LTSS)
				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(anchoTextura*LTSS));
				
			
			int currentTexture = 0;
			foreach(IsoTexture it in isoTextures){
				if(it.getTexture() == null)
					continue;
				
				if(currentTexture == 0)	EditorGUILayout.BeginHorizontal();
				
				Rect auxRect = GUILayoutUtility.GetRect(anchoTextura,anchoTextura);
				Rect border = new Rect(auxRect);
				auxRect.x+=2;auxRect.y+=2;auxRect.width-=4;auxRect.height-=4;
				
				if (e.isMouse && border.Contains(e.mousePosition)) { 
					if(e.type == EventType.mouseDown){
						paintingTexture = it.getTexture();
						paintingIsoTexture = it;
						repaint = true;
					}
				}
				
				if(it == paintingIsoTexture)
					EditorGUI.DrawRect(border,Color.yellow);
				GUI.DrawTexture(auxRect,it.getTexture());
				
				currentTexture++;
				if(currentTexture == maxTextures){EditorGUILayout.EndHorizontal(); currentTexture = 0;}
				
			}
			if(currentTexture != 0){
				GUILayoutUtility.GetRect((maxTextures - currentTexture)*anchoTextura,anchoTextura);
				EditorGUILayout.EndHorizontal();
			}
			
			if(isoTextures.Length > maxTextures*LTSS){
				EditorGUILayout.EndScrollView();
			}
		
		EditorGUILayout.EndVertical();

	}
	public void OnSceneGUI(SceneView scene){

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		RaycastHit info = new RaycastHit();
		
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

		GameObject selected = null;
		if(Physics.Raycast(ray, out info/*, LayerMask.NameToLayer("Cells Layer")*/)) //TODO Arreglar esto porque al parecer la distancia no funciona correctamente
			if(info.collider.transform.IsChildOf(this.map.transform))
				selected = info.collider.gameObject;
		
		
		bool paintLater = false;
		bool backupTextures = false;
		
		/* 
		 * Mouse Events of painting mode 
		 */
		if(Event.current.isMouse){
			if(Event.current.button == 0){
				if(Event.current.shift){

					if(Event.current.type == EventType.mouseDown)						collectTexture = true;
					if(collectTexture && Event.current.type != EventType.MouseUp)		backupTextures = true;
					else																collectTexture = false;

				}else if(Event.current.type == EventType.MouseDown){
					painting = true;
					paintLater = true;
				}else if(Event.current.type == EventType.MouseUp){
					painting = false;
				}else{
					if(painting)
						paintLater = true;
				}
			}
		}
		
		/**
		* Working zone
		*/
		
		if(selected != null){
			
			Cell cs = selected.GetComponent<Cell>();
			if(cs!=null){
                FaceNoSC f = cs.getFaceByPoint(info.point);
				if(f!=null){
					if(paintLater && paintingTexture != null){
						f.Texture = paintingTexture;
						f.TextureMapping = paintingIsoTexture;
                        cs.forceRefresh();
					}
					
					if(backupTextures){
						this.paintingTexture = f.Texture;
						this.paintingIsoTexture = f.TextureMapping;

						repaint = true; 
					}

					Vector3[] vertex = f.SharedVertex;
					int[] indexes = f.VertexIndex;
					
					Vector3[] puntos = new Vector3[4];
					for(int i = 0; i< indexes.Length; i++)
						puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);
					
					if(indexes.Length == 3)
						puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);
					
					Handles.DrawSolidRectangleWithOutline(puntos, (Event.current.shift)? Color.blue : Color.yellow, Color.white);
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

