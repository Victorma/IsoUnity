using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor {

	Map map;

	GUIStyle normalStyle;
	GUIStyle toolBarStyle;
	GUIStyle pressedStyle;
	GUIStyle infoStyle;
	GUIStyle titleStyle;

	void OnEnable()
	{
		//SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		this.map = (Map)target;
		baseTile = IsoSettingsManager.getInstance().getIsoSettings().defautTextureScale;

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
	
	void OnDestroy() 
	{
		map.removeGhost();
		//SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}
	
/** 
 * Window options
 * */
	
	bool editingMode;
	bool paintingMode;
	bool fixView;
	bool setPerspective = false;
	GameObject baseCell;
	Texture2D baseTile;
	int width;
	int height;
	
	int cellSize;
	
	int gridHeight = 0;
	float angle;
	
	bool movingGrid = false;
	float startingMousePosition;
	int startingGridHeight;
	
	void startMovingGrid(){
		movingGrid = true;
		startingMousePosition = Event.current.mousePosition.y;
		startingGridHeight = gridHeight;
	}
	
	void endMovingGrid(){
		movingGrid = false;
	}
	
	const float movingInterval = 30;
	void moveGrid(){
		if(movingGrid){
			float mouseDifference = startingMousePosition - Event.current.mousePosition.y;
			int movingTimes = Mathf.FloorToInt(mouseDifference / movingInterval);
			
			gridHeight = startingGridHeight + movingTimes;
			if(gridHeight < 0)
				gridHeight = 0;
			this.Repaint();
		}
	}
	
	int pulsado;
	int levantado;
	int movido;
	
	Quaternion fixedRotation;
	void backUpAngle(){
		fixedRotation = SceneView.lastActiveSceneView.camera.transform.rotation;
	}
	
	
	Texture2D paintingTexture;
	Texture2D paintingDecoration;
	bool parallelDecoration;
	bool rotateDecoration;
	int isoTextureIndex;
	int isoDecorationIndex;
	IsoTexture paintingIsoTexture;
	IsoDecoration paintingIsoDecoration;
	int modo = 0;
	int Selected = 0;
	Vector2 scroll = new Vector2(0,0);

	public override void OnInspectorGUI(){

		Event e = Event.current;
		//GUI.Box (Rect (10,10,100,90), "Loader Menu");
		//map.CellPrefab = UnityEditor.EditorGUILayout.ObjectField("Base Cell", map.CellPrefab, typeof(GameObject), true) as GameObject;
		
		if(cellSize < 1)
			cellSize = 1;
		map.setCellSize(cellSize);
		cellSize = UnityEditor.EditorGUILayout.IntField("Cell size", cellSize);

		Rect tool = GUILayoutUtility.GetRect(0,25,toolBarStyle);
		//.tool

		if(Tools.current != Tool.None)
			modo = -1;
		modo = GUI.Toolbar(tool,modo, new GUIContent[5]{new GUIContent("Nada"),new GUIContent("Editar"),new GUIContent("Pintar"), new GUIContent("Decorar"), new GUIContent("Entidades")});
		if(modo != -1)
			Tools.current = Tool.None;
		if(modo != 1 && modo != 3)
			map.removeGhost();

		/*EditorGUILayout.BeginHorizontal();

			GUIContent soltarText = new GUIContent("Nada");
			Rect soltarRect = GUILayoutUtility.GetRect(soltarText, (modo==0)?pressedStyle:normalStyle);
			if ((modo!=0)?GUI.Button (soltarRect, soltarText): GUI.Button (soltarRect, soltarText, pressedStyle)) modo = 0;

			GUIContent editarText = new GUIContent("Editar");
			Rect editarRect = GUILayoutUtility.GetRect(editarText, (modo==1)?pressedStyle:normalStyle);
			if ((modo!=1)?GUI.Button (editarRect, editarText):GUI.Button (editarRect, editarText, pressedStyle)) modo = 1;

			GUIContent pintarText = new GUIContent("Pintar");
			Rect pintarRect = GUILayoutUtility.GetRect(pintarText, (modo==2)?pressedStyle:normalStyle);
			if ((modo!=2)?GUI.Button (pintarRect, pintarText):GUI.Button (pintarRect, pintarText, pressedStyle)) modo = 2;

		EditorGUILayout.EndHorizontal();*/

		switch(modo){
			case 1:{

		/****************
		 * Creating mode
		 **************** */

			EditorGUILayout.HelpBox("Press left button to create cells. Press left button and slide up and down to increase/decrease cell height.", MessageType.None);
			EditorGUILayout.Space();

			GUIStyle style = new GUIStyle();
			style.padding = new RectOffset(2,2,2,2);
			//editingMode = EditorGUILayout.BeginToggleGroup("Editing mode",editingMode);

			GUIContent buttonText = new GUIContent("Set camera to game view");
			Rect buttonRect = GUILayoutUtility.GetRect(buttonText, style);				
			if(GUI.Button(buttonRect, buttonText))
				setPerspective = true;
			GUILayout.Space(5f);
			bool lastFixView = fixView;
			fixView = EditorGUILayout.Toggle("Fix perspective",fixView);
			
			if(lastFixView == false && fixView == true){
				backUpAngle();
			}
				

			gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);

			//EditorGUILayout.EndToggleGroup();


			} break;

			case 2: {
		/*****************
		 * Painting mode
		 ***************** */
			
			//paintingMode = EditorGUILayout.BeginToggleGroup("Painting mode",paintingMode);
			EditorGUILayout.HelpBox("Press left button to put the textures over the faces of the cell. Hold shift and press left button to copy the current texture of the hovering face.", MessageType.None);
			EditorGUILayout.Space();

			//GUILayout.BeginArea(new Rect(0,0,0,100), "Texture: ");

			EditorGUILayout.PrefixLabel("Texture",GUIStyle.none, titleStyle);

			EditorGUILayout.BeginHorizontal();
			
			paintingTexture = UnityEditor.EditorGUILayout.ObjectField("Tile", paintingTexture, typeof(Texture2D), false, GUILayout.MaxHeight(16)) as Texture2D;
			
			if(paintingTexture != null){
				IsoTexture[] textures = TextureManager.getInstance ().textureList (paintingTexture);
				List<string> texts = new List<string> ();
				
				//paintingIsoTexture = textures.Length;
				foreach (IsoTexture it in textures)
					texts.Add (it.name);
				
				texts.Add("None");
				//TODO CAMBIOS EN LA LISTA DEBERIAN DESELEECIONAR EL ELEMENTO ACTUAL SI ESTE YA NO ESTA EN LA LISTA
				isoTextureIndex = UnityEditor.EditorGUILayout.Popup(isoTextureIndex,texts.ToArray());
				if(isoTextureIndex == textures.Length)
					paintingIsoTexture = null;
				else
					paintingIsoTexture = textures[isoTextureIndex];
			}
			EditorGUILayout.EndHorizontal();

			GUI.backgroundColor = Color.Lerp(Color.black, Color.gray, 0.5f);

			EditorGUILayout.BeginVertical("Box");
				
				IsoTexture[] isoTextures = TextureManager.getInstance().textureList();
				
				int maxTextures = 8;
				float anchoTextura = (Screen.width - 30) / maxTextures;

				if(isoTextures.Length > maxTextures*4){
					scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(anchoTextura));
				}
			   
				int currentTexture = 0;
				foreach(IsoTexture it in isoTextures){
					if(it.getTexture() == null)
						continue;

					if(currentTexture == 0)	EditorGUILayout.BeginHorizontal();

					Rect auxRect = GUILayoutUtility.GetRect(anchoTextura,anchoTextura);
					Rect border = new Rect(auxRect);
					auxRect.x+=2;auxRect.y+=2;auxRect.width-=4;auxRect.height-=4;

						if (e.isMouse && border.Contains(e.mousePosition)) 
					{ 
						if(e.type == EventType.mouseDown){
							paintingTexture = it.getTexture();
							paintingIsoTexture = it;
						this.Repaint();
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

			if(isoTextures.Length > maxTextures*4){
				 EditorGUILayout.EndScrollView();
			}

			EditorGUILayout.EndVertical();
			//isoTexture = (IsoTexture)EditorGUILayout.ObjectField(isoTexture,typeof(IsoTexture),true);
			}break;

		case 3: {
		/*****************
		 * Decoration mode
		 ***************** */
		
			//paintingMode = EditorGUILayout.BeginToggleGroup("Painting mode",paintingMode);
			EditorGUILayout.HelpBox("Press left button to put the textures over the faces of the cell. Hold shift and press left button to copy the current texture of the hovering face.", MessageType.None);
			EditorGUILayout.Space();
			
			//GUILayout.BeginArea(new Rect(0,0,0,100), "Texture: ");
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
			
			if(isoDecorations.Length > maxTextures*4){
				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(anchoTextura));
			}
			
			int currentTexture = 0;
			foreach(IsoDecoration it in isoDecorations){
				if(it.getTexture() == null)
					continue;
				
				if(currentTexture == 0)	EditorGUILayout.BeginHorizontal();
				
				Rect auxRect = GUILayoutUtility.GetRect(anchoTextura,anchoTextura);
				Rect border = new Rect(auxRect);
				auxRect.x+=2;auxRect.y+=2;auxRect.width-=4;auxRect.height-=4;
				
				if (e.isMouse && border.Contains(e.mousePosition)) 
				{ 
					if(e.type == EventType.mouseDown){
						paintingIsoDecoration = it;
						this.Repaint();
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
			
			if(isoDecorations.Length > maxTextures*4){
				EditorGUILayout.EndScrollView();
			}
			
			EditorGUILayout.EndVertical();
			//isoTexture = (IsoTexture)EditorGUILayout.ObjectField(isoTexture,typeof(IsoTexture),true);

			}break;

			case 4: {
				colocar = EditorGUILayout.ObjectField(colocar, typeof(Entity),true) as Entity;

			}break;
		}
	}

	Entity colocar;
	GameObject selected;
	GameObject der;
	bool creating = false;
	bool painting = false;
	
	bool collectTexture = false;
	bool collectDecoration = false;
	
	void OnSceneGUI (){
		SceneView sceneView = SceneView.currentDrawingSceneView;
		
		if(modo == 1)
		{
			
			if(setPerspective){
				/* Selection.transforms	*/
				setPerspective = false;
				angle = 30;
				if(baseTile != null){
					float angulo = Mathf.Rad2Deg * Mathf.Acos(baseTile.height / (baseTile.width*1f));
					angle = 90f - angulo;
				}
				sceneView.LookAtDirect(sceneView.pivot,Quaternion.Euler(angle, 45, 0));
				sceneView.orthographic = true;
				
				if(fixView)
					fixedRotation = Quaternion.Euler(angle, 45, 0);
				
				this.Repaint();
			}
			
			if(fixView){
				sceneView.LookAtDirect(sceneView.pivot,fixedRotation);
			}

			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			if(Event.current.isMouse){
				if(Event.current.button == 0){
					if(Event.current.type == EventType.MouseDown){
						creating = true;
						map.addCell(map.getMousePositionOverMap(gridHeight));
					}else if(Event.current.type == EventType.MouseUp){
						creating = false;
					}else{
						if(creating)
							map.addCell(map.getMousePositionOverMap(gridHeight));
					}

				}
				
				if(Event.current.button == 1){
					if(Event.current.type == EventType.MouseDown){
						startMovingGrid();
					}else if(Event.current.type == EventType.MouseUp){
						endMovingGrid();
					}else{
						moveGrid();
					}
				}
			}
			Vector3 centerGridPoint = map.getMousePositionOverMap(gridHeight);

			map.ghostCell(centerGridPoint, 0.5f);

			Vector3[] puntos = new Vector3[4];
			puntos[0] = new Vector3(centerGridPoint.x - cellSize/2.0f,centerGridPoint.y,centerGridPoint.z - cellSize/2.0f);
			puntos[1] = new Vector3(centerGridPoint.x - cellSize/2.0f,centerGridPoint.y,centerGridPoint.z + cellSize/2.0f);
			puntos[2] = new Vector3(centerGridPoint.x + cellSize/2.0f,centerGridPoint.y,centerGridPoint.z + cellSize/2.0f);
			puntos[3] = new Vector3(centerGridPoint.x + cellSize/2.0f,centerGridPoint.y,centerGridPoint.z - cellSize/2.0f);
			Handles.DrawSolidRectangleWithOutline(puntos, Color.yellow, Color.white);
		}
		
		if(modo == 2){
			/*for(int i = 0; i< map.transform.childCount; i++)
				EditorUtility.SetSelectedWireframeHidden(map.transform.GetChild(i).collider., true);*/

			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			RaycastHit info = new RaycastHit();
			
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			if(Physics.Raycast(ray, out info/*, LayerMask.NameToLayer("Cells Layer")*/)){ //TODO Arreglar esto porque al parecer la distancia no funciona correctamente
				if(info.collider.transform.IsChildOf(this.map.transform))
					selected = info.collider.gameObject;
				else
					selected = null;
			}else
				selected = null;
			
			bool paintLater = false;
			bool backupTextures = false;

			/** 
			 * Mouse Events of painting mode 
			 */

			if(Event.current.isMouse){
				if(Event.current.button == 0)
				{
					if(Event.current.shift){
						if(Event.current.type == EventType.mouseDown){
							collectTexture = true;
						}if(collectTexture && Event.current.type != EventType.MouseUp){
							backupTextures = true;
						}else{
							collectTexture = false;
						}
					}
					else if(Event.current.type == EventType.MouseDown){
						painting = true;
						paintLater = true;
					}else if(Event.current.type == EventType.MouseUp){
						painting = false;
					}else{
						if(painting){
							paintLater = true;
						}
					}
				}
			}

			/**
			 * Working zone
			 */
			
			if(selected != null){
				
				Cell cs = selected.GetComponent<Cell>();
				Face f = cs.getFaceByPoint(info.point);
				
				if(f!=null){
					if(paintLater){
						if(paintingTexture != null){
							f.Texture = paintingTexture;
							f.TextureMapping = paintingIsoTexture;
							cs.refresh();
						}
					}
					
					if(backupTextures){
						this.paintingTexture = f.Texture;

						if(paintingTexture != null){
							IsoTexture[] isoTextures = TextureManager.getInstance().textureList(paintingTexture);
							if(f.TextureMapping != null){
								int texture = isoTextures.Length;
								for(int i = 0; i< isoTextures.Length; i++)
								if(f.TextureMapping == isoTextures[i]){texture = i; break;}
								this.isoTextureIndex = texture;
							}else
								this.isoTextureIndex = isoTextures.Length;
						}
						this.Repaint();
					}
					
					//Debug.Log("He seleccionado!" + selected.name);
					Vector3 position = selected.transform.position;
					if(cs != null)
						position.y = cs.Height;
					
					Vector3[] vertex = f.SharedVertex;
					int[] indexes = f.VertexIndex;
					
					Vector3[] puntos = new Vector3[4];
					for(int i = 0; i< indexes.Length; i++){
						puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);
					}

					if(indexes.Length == 3)
						puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);
					
					Color color = Color.yellow;
					if(Event.current.shift)	color = Color.blue;
					
					Handles.DrawSolidRectangleWithOutline(puntos, color, Color.white);
				}
			}
		}

		if(modo == 3){
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			RaycastHit info = new RaycastHit();
			
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			if(Physics.Raycast(ray, out info/*, LayerMask.NameToLayer("Cells Layer")*/)){ //TODO Arreglar esto porque al parecer la distancia no funciona correctamente
				if(info.collider.transform.IsChildOf(this.map.transform))
					selected = info.collider.gameObject;
				else
					selected = null;
			}else
				selected = null;


			bool decorateLater = false;
			
			/** 
			 * Mouse Events of painting mode 
			 */

			if(Event.current.isMouse){
				if(Event.current.button == 0)
				{
					if(selected!=null){
						Cell cs = selected.GetComponent<Cell>();

						if(Event.current.type == EventType.MouseUp){
							decorateLater = true;
						}
					}
				}
			}
			
			/**
			 * Working zone
			 */
			
			if(selected != null){
				
				Cell cs = selected.GetComponent<Cell>();
				Face f = cs.getFaceByPoint(info.point);

				if(cs!=null){
					
					//Debug.Log("He seleccionado!" + selected.name);
					Vector3 position = selected.transform.position;
					if(cs != null)
						position.y = cs.Height;
					
					Vector3[] vertex = f.SharedVertex;
					int[] indexes = f.VertexIndex;
					
					Vector3[] puntos = new Vector3[5];
					for(int i = 0; i< indexes.Length; i++){
						puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);
					}
					puntos[4] = puntos[0];
					
					if(indexes.Length == 3)
						puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);

					Handles.DrawPolyLine(puntos);

					Vector2 directions = new Vector2(puntos[1].x-puntos[0].x, puntos[2].y-puntos[1].y);

					int ang = 0;

					if(directions.x == 0f)

						if(directions.y == 0f)
							ang = 0;
						else
							ang = 2;
					else{
						ang = 1;
					}

					if(decorateLater){
						if(paintingIsoDecoration != null){
							Debug.Log("decor" + info.point);
							cs.addDecoration(info.point, ang, rotateDecoration, parallelDecoration, (Event.current.shift)?false:true, paintingIsoDecoration);
							cs.refresh();
							decorateLater = false;
						}
					}else{
						Debug.Log("gosht" + info.point);
						map.ghostDecoration(cs, info.point, ang, rotateDecoration, parallelDecoration, (Event.current.shift)?false:true, paintingIsoDecoration, 0.5f);
					}
				}
			}
		}

		if(modo == 4){
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			RaycastHit info = new RaycastHit();
			
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			if(Physics.Raycast(ray, out info/*, LayerMask.NameToLayer("Cells Layer")*/)){ //TODO Arreglar esto porque al parecer la distancia no funciona correctamente
				if(info.collider.transform.IsChildOf(this.map.transform))
					selected = info.collider.gameObject;
				else
					selected = null;
			}else
				selected = null;

			bool moveEntity = false;
			if(Event.current.isMouse){
				if(Event.current.button == 0)
				{
					if(Event.current.type == EventType.MouseDown){
						moveEntity = true;
					}
				}
			}

			if(selected != null){

				Cell cs = selected.GetComponent<Cell>();
				Face f = null;

				if(cs!=null)
					f = cs.getFaceByPoint(info.point);
				
				if(f!=null){

					if(moveEntity)
						colocar.Position = cs;

					Vector3[] vertex = f.SharedVertex;
					int[] indexes = f.VertexIndex;
					
					Vector3[] puntos = new Vector3[4];
					for(int i = 0; i< indexes.Length; i++){
						puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);
					}
					
					if(indexes.Length == 3)
						puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);
					
					Color color = Color.yellow;
					if(Event.current.shift)	color = Color.blue;

					Handles.DrawSolidRectangleWithOutline(puntos, color, Color.white);
				}
			}

		}

		sceneView.Repaint();
	}
}
	

