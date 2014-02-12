using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum CellTopType {
	flat, plane, midPlane
};

public enum CellTopOrientation {
	north, east, south, west
};

[ExecuteInEditMode]
public class Cell : MonoBehaviour {

	/*******************************
	 * BEGIN ATRIBS
	 *******************************/
	[SerializeField]
	private float height;

	public float Height {
		get {
			return height;
		}
		set {
			float lastHeight = this.height;
			this.height = Mathf.RoundToInt(value*2.0f)/2.0f;
			if(lastHeight != height)
				regenerateMesh();
		}
	}

	[SerializeField]
	private Map map;

	public Map Map{
		get {
			/*if(map == null){
				Debug.Log(transform.gameObject.name + "  -  " + (transform.parent == null));
				map = transform.parent.gameObject.GetComponent<Map>();
			}*/
			return map;
		}

		set {
			transform.parent = value.transform;
			this.map = value;
		}
	}


	[SerializeField]
	private float cellWidth = 1;
	public void setCellWidth(float cellWidth){
		if(this.cellWidth !=cellWidth){
			this.cellWidth = cellWidth;
			regenerateMesh();
		}
	}
	[SerializeField]
	private CellTopType cellTop = CellTopType.flat;

	public CellTopType CellTop {
		get {
			return cellTop;
		}
		set {
			CellTopType lastTop = cellTop;
			cellTop = value;
			if(cellTop != lastTop)
				regenerateMesh();
		}
	}

	// TODO QUITAR ESTO
	public CellTopType getTopType(){
		return cellTop;
	}
	public void setTop(CellTopType cellTopType){
		
		CellTopType lastTop = cellTop;
		cellTop = cellTopType;
		if(cellTop != lastTop)
			regenerateMesh();
	}

	[SerializeField]
	private Vector3[] vertices;
	[SerializeField]
	private List<Vector3> finalVertexList;
	[SerializeField]
	private Mesh auxMesh;
	[SerializeField]
	private Face[] faces;

	/** ********************
	 * END ATRIBS
	 * *********************/

	public Cell(){
	}

	public void refresh(){
		this.updateTextures();
		this.updateMesh();
	}

	private void regenerateMesh(){
		regenerateFaces();

		updateTextures();
		updateMesh();
	}

	private void updateMesh(){

		ArrayList triangles = new ArrayList();
		ArrayList uvs = new ArrayList();
		ArrayList finalVertices = new ArrayList();

		foreach(Face f in faces){
			triangles.AddRange(f.Triangles);
			foreach(int vertex in f.VertexIndex)
				finalVertices.Add(vertices[vertex]);
			uvs.AddRange(f.Uvs);
		}

		auxMesh = new Mesh();

		GetComponent<MeshFilter>().mesh = auxMesh;
		auxMesh.vertices = finalVertices.ToArray(typeof(Vector3)) as Vector3[];
		auxMesh.triangles = triangles.ToArray(typeof(int)) as int[];
		auxMesh.RecalculateNormals();
		auxMesh.uv = uvs.ToArray(typeof(Vector2)) as Vector2[];
		auxMesh.name = "Dynamic Cell";
		auxMesh.RecalculateBounds();
		
		GetComponent<MeshCollider>().sharedMesh = auxMesh;
	}

	private Face createFace(int[] indexes, Face copy, List<Vector3> vertexList, Vector3[] vertices){

		Face f = ScriptableObject.CreateInstance<Face>();
		f.FinalVertexList = vertexList;
		f.SharedVertex = vertices;
		f.VertexIndex = indexes;
		f.regenerateTriangles();

		if(copy != null)
		{
//			f.getAtribsFrom(copy);
			f.Texture = copy.Texture;
			f.TextureMapping = copy.TextureMapping;
		}

		return f;
	}

	private void regenerateFaces(){

		List<Face> tmpFaces = new List<Face>();
		finalVertexList = new List<Vector3>();

		bool hasMediumTop = height != ((float)((int)height));
		int numVert = ((int)height+1)*4 + ((hasMediumTop)?4:0);

		vertices = new Vector3[numVert + ((cellTop != CellTopType.flat)?2:0)];

		// INDEXES FOR TOP FACE
		int vertTopLeft = vertices.Length - 4;		int verTopRight = vertices.Length - 3;
		int vertBotLeft = vertices.Length - 1;		int vertBotRight = vertices.Length - 2;

		//MAIN VARS FOR VERTICES
		float halfWidth = (cellWidth/2.0f);
		Vector3 vectorHeight = new Vector3(0,cellWidth,0);

		// BASE VERTICES
		vertices[0] = new Vector3(-halfWidth,0,-halfWidth);		vertices[1] = new Vector3(halfWidth,0,-halfWidth);
		vertices[2] = new Vector3(halfWidth,0,halfWidth);		vertices[3] = new Vector3(-halfWidth,0,halfWidth);
		if(height>=1) vertices[4] = vertices[0] + vectorHeight;
		else if(hasMediumTop) vertices[4] = vertices[0] + vectorHeight*0.5f;

		//MAIN LATERAL FACE GENERATOR
		for(int i = 4; i < numVert; i++){
			int cutted = (i%4 == 3)?1:0;
			if(i+1 < numVert) 
				vertices[i+1] = vertices[i-3] + vectorHeight*((hasMediumTop && (i+1) >= numVert-4)?0.5f:1f);

			int[] indexes = new int[4] {i-4, i-3-(4*cutted), i+1-(4*cutted), i}; 
			Face last = null;
			if(faces != null){
				if(tmpFaces.Count < faces.Length-1)	last = faces[tmpFaces.Count];
				else if(tmpFaces.Count-4 >= 0) 	last = tmpFaces[tmpFaces.Count-4];
			}

			tmpFaces.Add (createFace(indexes, last, finalVertexList, vertices));
		}

		//EXTRA FACES GENERATOR
		if(cellTop != CellTopType.flat){
			float aumHeight = (cellTop == CellTopType.midPlane)?0.5f:1f;

			vertices[numVert] = vertices[numVert-3] + vectorHeight*aumHeight;
			vertices[numVert+1] = vertices[numVert-2] + vectorHeight*aumHeight;

			//NEW TOP FACE
			vertTopLeft = numVert-4;		verTopRight = numVert;
			vertBotRight = numVert+1;		vertBotLeft = numVert-1;

			// Lado Derecho
			Face f = ScriptableObject.CreateInstance<Face>(); f.FinalVertexList = finalVertexList;	f.SharedVertex = vertices;	
			f.VertexIndex = new int[3] {numVert , numVert-4, numVert-3}; f.regenerateTriangles();
			if(faces != null){
				if(faces.Length > tmpFaces.Count && tmpFaces.Count != faces.Length-1)
				{ f.Texture = faces[tmpFaces.Count].Texture; f.TextureMapping = faces[tmpFaces.Count].TextureMapping; }
				else if(tmpFaces.Count-4 >= 0)
				{ f.Texture = tmpFaces[tmpFaces.Count-4].Texture; f.TextureMapping = tmpFaces[tmpFaces.Count-4].TextureMapping; }
			}
			tmpFaces.Add (f);
			
			//Lado Izquierdo
			f = ScriptableObject.CreateInstance<Face>(); f.FinalVertexList = finalVertexList;	f.SharedVertex = vertices;	
			f.VertexIndex = new int[3] {numVert+1, numVert-2, numVert-1}; f.regenerateTriangles();
			if(faces != null){
				if(faces.Length > tmpFaces.Count && tmpFaces.Count != faces.Length-1)
				{ f.Texture = faces[tmpFaces.Count].Texture; f.TextureMapping = faces[tmpFaces.Count].TextureMapping; }
				else if(tmpFaces.Count-4 >= 0)
				{ f.Texture = tmpFaces[tmpFaces.Count-4].Texture; f.TextureMapping = tmpFaces[tmpFaces.Count-4].TextureMapping; }
			}
			tmpFaces.Add (f);
			
			//Parte de atras
			f = ScriptableObject.CreateInstance<Face>(); f.FinalVertexList = finalVertexList;	f.SharedVertex = vertices;	
			f.VertexIndex = new int[4] {numVert-3, numVert-2, numVert+1, numVert}; f.regenerateTriangles();
			if(faces != null && faces.Length > tmpFaces.Count && tmpFaces.Count != faces.Length-1)
			if(faces != null){
				if(faces.Length > tmpFaces.Count && tmpFaces.Count != faces.Length-1)
				{ f.Texture = faces[tmpFaces.Count].Texture; f.TextureMapping = faces[tmpFaces.Count].TextureMapping; }
				else if(tmpFaces.Count-4 >= 0)
				{ f.Texture = tmpFaces[tmpFaces.Count-4].Texture; f.TextureMapping = tmpFaces[tmpFaces.Count-4].TextureMapping; }
			}
			tmpFaces.Add (f);
		}

		//TOP FACE GENERATOR
		Face topFace = ScriptableObject.CreateInstance<Face>(); topFace.FinalVertexList = finalVertexList;	topFace.SharedVertex = vertices;	
		topFace.VertexIndex = new int[4] {vertBotLeft, vertTopLeft, verTopRight, vertBotRight};	topFace.regenerateTriangles();
		if(faces != null && faces.Length >=1){ topFace.Texture = faces[faces.Length-1].Texture; topFace.TextureMapping = faces[faces.Length-1].TextureMapping; }
		tmpFaces.Add (topFace);		

		faces = tmpFaces.ToArray();//..ToArray(typeof(Face)) as Face[];

	}

	private void generateTriangles(int[] face, int[] triangles, int pos){
		//The face has to be in counter clock wise
		
		triangles[pos] = face[0];
		triangles[pos+1] = face[2];
		triangles[pos+2] = face[1];
		
		triangles[pos+3] = face[2];
		triangles[pos+4] = face[0];
		triangles[pos+5] = face[3];

	}
	
	public void updateTextures(){

		// BASE TEXTURE ATLAS
		Texture2D TextureAtlas = new Texture2D(10,10);
		TextureAtlas.anisoLevel = 0;
		TextureAtlas.filterMode = FilterMode.Point;

		//RECOPILATING TEXTURES
		Texture2D[] AllCubeTextures = new Texture2D[faces.Length];
		for(int i = 0; i< faces.Length; i++)
			AllCubeTextures[i] = (faces[i] as Face).Texture;

		Rect[] posTexturas = TextureAtlas.PackTextures(AllCubeTextures,0);

		for(int i = 0; i< faces.Length; i++)
			faces[i].regenerateUVs(posTexturas[i]);

		this.renderer.material.SetTexture("_MainTex",TextureAtlas);
	}
	
	public Face getFaceByPoint(Vector3 point){

		foreach(Face f in faces){
			if(f.contains(transform.InverseTransformPoint(point)))
				return f;
		}

		return null;
	}

	/**##########################################
	 * 			GAME RELATED
	 * */

	public bool isAccesibleBy(Entity entity){
		foreach(Entity e in getEntities()){
			//if(entity.canEnter(e))
				return true;
		}
		return false;
	}

	public Entity[] getEntities(){
		return this.transform.GetComponentsInChildren<Entity>();
	}

	public void tick(){
		foreach(Entity en in getEntities()){
			en.tick();
		}
	}

	/* #########################################################
	 * 					ENTITIES THINGS
	 * */
	
	[SerializeField]
	private List<Entity> entities = new List<Entity>();
	public List<Entity> Entities{get;set;}

	public void addEntity(Entity en){
		if(!entities.Contains(en)){
			entities.Add(en);
		}
	}

	public void removeEntity(Entity en){
		if(entities.Contains(en))
			entities.Remove (en);
	}

	/* #########################################################
	 * 					DECORATION THINGS
	 * */
	
	[SerializeField]
	private List<Decoration> decorations;
	
	public void addDecoration(GameObject dec, IsoDecoration iDec, float x, float y, Cell father){
		if (decorations == null)
			decorations = new List<Decoration> ();
		
		Decoration dr = dec.GetComponent<Decoration> ();
		dr.X = x; dr.Y = y; dr.Father = father; dr.IsoDec = iDec;
		this.decorations.Add(dr);
	}
	
	public void removeDecoration(Decoration d){
		if (decorations == null)
			decorations = new List<Decoration> ();
		else {
			if(decorations.Contains(d))
				decorations.Remove(d);
		}
	}


	// Use this for initialization
	void Start () {

	}

	Transform t_trans;
	// Update is called once per frame
	Vector3 lastLocalPosition;
	void Update () {
		if(t_trans == null)
			t_trans = this.transform;

		if(lastLocalPosition != t_trans.localPosition){
			if(Height + t_trans.localPosition.y > 0)
				Height+= t_trans.localPosition.y;
			else Height = 0;

			t_trans.localPosition = new Vector3(t_trans.localPosition.x, 0, t_trans.localPosition.z);
			lastLocalPosition = t_trans.localPosition;
		}
	}


	void OnDestroy () {
		if(this.Map != null)
			this.Map.removeCell(this);
	}
};
