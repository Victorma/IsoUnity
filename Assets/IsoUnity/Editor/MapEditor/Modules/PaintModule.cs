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

    public Texture2D PaintingTexture{ get{return paintingTexture;} set{paintingTexture = value;}}
    public IsoTexture PaintingIsoTexture{ get{return paintingIsoTexture;} set{paintingIsoTexture = value;}}

	// SceneGUI vars
	private bool collectTexture;
	private bool backupTextures;
	private bool painting;

    //Brushes
    private int selectedBrush = 0;
    private Brush[] brushes;
    private string[] brushNames;

	private GUIStyle titleStyle;

	public PaintModule(){
		titleStyle = new GUIStyle();
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.margin = new RectOffset(0,0,5,5);

        brushes = new Brush[] { new NormalBrush(), new SurfaceBrush() };
        List<string> names = new List<string>();

        foreach (var b in brushes)
        {
            b.PaintModule = this;
            names.Add(b.BrushName);
        }

        brushNames = names.ToArray();
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
                if (path.StartsWith("Assets") || path.StartsWith("assets"))
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

        EditorGUILayout.PrefixLabel("Brush", GUIStyle.none, titleStyle);

        selectedBrush = EditorGUILayout.Popup(selectedBrush, brushNames);

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

		
		
        /**
         * BRUSH ACTIONS
         **/

        brushes[selectedBrush].Prepare();
        brushes[selectedBrush].Display();
        brushes[selectedBrush].Work();
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

    private abstract class Brush
    {

        protected PaintModule paintModule = null;
        public PaintModule PaintModule { set { paintModule = value; } }

        public abstract string BrushName { get; }
        public abstract void Prepare();
        public abstract void Display();
        public abstract void Work();

    }

    private abstract class LeftButtonAbstractBrush : Brush{

        protected bool painting = false;
        protected bool paintLater = false;
        protected bool collectTexture = false;
        protected bool backupTextures = false;

        protected Cell cs;
        protected FaceNoSC f;

        public override void Prepare()
        {
            paintLater = false;
            backupTextures = false;

            RaycastHit info = new RaycastHit();

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            GameObject go = null;
            if (Physics.Raycast(ray, out info/*, LayerMask.NameToLayer("Cells Layer")*/)) //TODO Arreglar esto porque al parecer la distancia no funciona correctamente
                if (info.collider.transform.IsChildOf(paintModule.map.transform))
                    go = info.collider.gameObject;

            if (Event.current.isMouse)
            {
                if (Event.current.button == 0)
                {
                    if (Event.current.shift)
                    {

                        if (Event.current.type == EventType.mouseDown) collectTexture = true;
                        if (collectTexture && Event.current.type != EventType.MouseUp) backupTextures = true;
                        else collectTexture = false;

                    }
                    else if (Event.current.type == EventType.MouseDown)
                    {
                        painting = true;
                        paintLater = true;
                    }
                    else if (Event.current.type == EventType.MouseUp)
                    {
                        painting = false;
                    }
                    else
                    {
                        if (painting)
                            paintLater = true;
                    }
                }
            }
            if (go != null)
                cs = go.GetComponent<Cell>();
            else cs = null;
            if (cs != null)
                f = cs.getFaceByPoint(info.point);
            else f = null;
        }

    }

    private class NormalBrush : LeftButtonAbstractBrush
    {

        public override string BrushName { get{ return "Single Face"; } }
        

        public override void Display()
        {
            if (f != null)
            {
                Vector3[] vertex = f.SharedVertex;
                int[] indexes = f.VertexIndex;

                Vector3[] puntos = new Vector3[4];
                for (int i = 0; i < indexes.Length; i++)
                    puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);

                if (indexes.Length == 3)
                    puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);

                Handles.DrawSolidRectangleWithOutline(puntos, (Event.current.shift) ? Color.blue : Color.yellow, Color.white);
            }
        }

        public override void Work()
        {
            if(f!=null){
				if(paintLater && paintModule.PaintingTexture != null){
					f.Texture = paintModule.PaintingTexture;
					f.TextureMapping = paintModule.PaintingIsoTexture;
                    cs.forceRefresh();
				}
					
				if(backupTextures){
					paintModule.PaintingTexture = f.Texture;
					paintModule.PaintingIsoTexture = f.TextureMapping;

					paintModule.Repaint = true; 
				}
			}
        }
    }

    private class SurfaceBrush : LeftButtonAbstractBrush
    {

        public override string BrushName { get { return "Surface"; } }

        private Vector3[] getRealPointsOf(FaceNoSC f)
        {
            Vector3[] vertex = f.SharedVertex;
            int[] indexes = f.VertexIndex;

            Vector3[] puntos = new Vector3[4];
            for (int i = 0; i < indexes.Length; i++)
                puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);

            if (indexes.Length == 3)
                puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);

            return puntos;
        }

        private List<Cell.CellFace> faces;

        private List<Cell.CellFace> generateFaces(Cell.CellFace cf)
        {
            List<Cell.CellFace> faces = new List<Cell.CellFace>();
            Dictionary<FaceNoSC, bool> closed = new Dictionary<FaceNoSC, bool>();

            faces.Add(cf);
            closed.Add(cf.f, false);
            generateFaces(cf, faces, closed);

            return faces;
        }

        private void generateFaces(Cell.CellFace cf, List<Cell.CellFace> result, Dictionary<FaceNoSC, bool> closed)
        {
            closed[cf.f] = true;

            Cell.CellFace[] faces = cf.c.getSameSurfaceAdjacentFaces(cf.f);
            foreach (var nf in faces)
                if (!closed.ContainsKey(nf.f))
                {
                    result.Add(nf);
                    closed.Add(nf.f, false);
                    generateFaces(nf, result, closed);
                }

        }

        public override void Prepare()
        {
            base.Prepare();

            if (f != null) faces = generateFaces(new Cell.CellFace(cs, f));
            else faces = null;
            

        }

        public override void Display()
        {
            if (faces != null)
            {
                foreach (var cfs in faces)
                {

                    if (cfs.f != null)
                    {
                        Vector3[] vertex = cfs.f.SharedVertex;
                        int[] indexes = cfs.f.VertexIndex;

                        Vector3[] puntos = new Vector3[4];
                        for (int i = 0; i < indexes.Length; i++)
                            puntos[i] = cfs.c.transform.TransformPoint(vertex[indexes[i]]);

                        if (indexes.Length == 3)
                            puntos[3] = cfs.c.transform.TransformPoint(vertex[indexes[2]]);

                        Handles.DrawSolidRectangleWithOutline(puntos, (Event.current.shift) ? Color.blue : new Color(255f,255f,0f,0.5f), Color.white);
                    }
                }
            }
        }

        public override void Work()
        {

            if (faces != null)
            {

                Dictionary<Cell, List<FaceNoSC>> eachCellFaces = new Dictionary<Cell, List<FaceNoSC>>();
                foreach (var cfs in faces)
                {
                    // First i recopilate all faces
                    if (paintLater && paintModule.PaintingTexture != null)
                    {
                        if (!eachCellFaces.ContainsKey(cfs.c))
                            eachCellFaces.Add(cfs.c, new List<FaceNoSC>());

                        eachCellFaces[cfs.c].Add(cfs.f);
                    }

                    if (backupTextures)
                    {
                        paintModule.PaintingTexture = cfs.f.Texture;
                        paintModule.PaintingIsoTexture = cfs.f.TextureMapping;

                        paintModule.Repaint = true;
                    }
                }

                // Then I update all of them
                foreach (var clf in eachCellFaces)
                {
                    foreach (var eachFace in clf.Value)
                    {
                        eachFace.Texture = paintModule.PaintingTexture;
                        eachFace.TextureMapping = paintModule.PaintingIsoTexture;
                        
                    }
                    clf.Key.forceRefresh();
                }
            }
        }
    }
	
}

