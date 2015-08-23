using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

public enum CellTopType {
	flat, plane, midPlane
};

public enum CellTopOrientation {
	north, east, south, west
};

[ExecuteInEditMode]
public class Cell : MonoBehaviour, ISerializationCallbackReceiver
{

	/**
	 * Ser De
	 */

    //Old Info
    [SerializeField]
    private float width;
    [SerializeField]
    private CellTopType topType;
    [SerializeField]
    private int cellTopRotation;
    [SerializeField]
    private float height;


	[SerializeField]
	private bool walkable = true;
	[SerializeField]
	private float walkingHeight;
    [SerializeField]
    private CellProperties properties;

    public CellProperties Properties
    {
        get
        {
            if (this.properties == null)
                this.properties = new CellProperties(height, topType, cellTopRotation, width, new FaceNoSC[0]);

            return properties;
        }
    }

	//TODO remove the face serialization
	[SerializeField]
	private Face[] faces;

    public class CellFace
    {
        public CellFace(Cell c, FaceNoSC f) { this.c = c; this.f = f; }

        public Cell c;
        public FaceNoSC f;

        public override bool Equals(object other)
        {
            if (other is CellFace)
            {
                var cfo = other as CellFace;
                return cfo.f == f && cfo.c == c;
            }

            return false;
        }
    }

    private int getFaceIndex(FaceNoSC f)
    {
        if (f == properties.faces[properties.faces.Length - 1])
        {
            return properties.faces.Length - 1;
        }
        else
        {
            for (int i = 0; i < properties.faces.Length; i++)
            {
                if (properties.faces[i] == f)
                    return i;
            }
        }

        return -1;
    }

    public bool isCoveredByOthers(FaceNoSC f, int pos, Cell[] neighbors)
    {
        bool isCovered = false;

        float height = ((pos / 4) + 1) * this.properties.width;
        int direction = pos % 4;
        switch (direction)
        {
            case 0: direction = 2;break;
            case 2: direction = 0; break;
        }

        if (neighbors[direction] != null)
            if (neighbors[direction].Height >= height)
                isCovered = true;

        return isCovered;

    }

    public CellFace[] getSameSurfaceAdjacentFaces(FaceNoSC face){

        List<CellFace> adjacents = new List<CellFace>();

        int index = getFaceIndex(face);
        Cell[] neighbors = this.Map.getNeightbours(this);

        if (index == this.properties.faces.Length - 1)
        {
            // Es el top
            foreach(var n in neighbors)
                if(n !=null)
                    if(n.Height == this.Height)
                        adjacents.Add(new CellFace(n, n.properties.faces[n.properties.faces.Length-1]));

        }
        else if(index != -1)
        {
            // The lower one
            if (index - 4 >= 0 && !isCoveredByOthers(this.properties.faces[index - 4], index - 4, neighbors))
                adjacents.Add(new CellFace(this, this.properties.faces[index - 4]));
            // The upper one
            if (index + 4 < this.properties.faces.Length-1 && !isCoveredByOthers(this.properties.faces[index + 4], index + 4, neighbors))
                adjacents.Add(new CellFace(this, this.properties.faces[index + 4]));


            int direction = index % 4;
            Cell leftOne = neighbors[(direction+1) % 4];

            //The left one
            if (leftOne != null && index < leftOne.properties.faces.Length - 1 && !isCoveredByOthers(leftOne.properties.faces[index], index, leftOne.Map.getNeightbours(leftOne)))
                adjacents.Add(new CellFace(leftOne, leftOne.properties.faces[index]));

            Cell rightOne = neighbors[(direction + 3) % 4];
            //The rightOne
            if (rightOne != null && index < rightOne.properties.faces.Length - 1 && !isCoveredByOthers(rightOne.properties.faces[index], index, rightOne.Map.getNeightbours(rightOne)))
                adjacents.Add(new CellFace(rightOne, rightOne.properties.faces[index]));

        }

        return adjacents.ToArray();
    }


    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {

    }

	// Deserialization moment
	void Awake (){

#if UNITY_EDITOR

        if (Application.isEditor && !Application.isPlaying)
        {

            // Code to prevent losing information from old versions
            if (faces != null && faces.Length != 0)
            { // The first time i have to initialize those

                FaceNoSC[] nfaces = new FaceNoSC[faces.Length];
                for (int i = 0; i < faces.Length; i++)
                {
                    if (faces[i] == null)
                    {
                        // Debug.Log("Null face detected at " + this + " at map " + Map);
                        break;
                    }

                    nfaces[i] = new FaceNoSC();
                    nfaces[i].Texture = faces[i].Texture;
                    nfaces[i].TextureMapping = faces[i].TextureMapping;
                }


                this.properties = new CellProperties(height, topType, cellTopRotation, 1, faces != null ? nfaces : new FaceNoSC[0]);
                Debug.Log("No era null");
                faces = null;
            }

        }
#endif

		// This will prevent older versions to lose face information about textures.
		/*if (this.faces == null) {
			faces = new Face[faceTextures.Length];
			for (int i = 0; i< faces.Length; i++) {
				faces[i].Texture = faceTextures[i];
				faces[i].TextureMapping = faceMappings[i];
			}
			faceTextures = null;
			faceMappings = null;
		}		*/
	}
	
	// Serialization moment
	void OnDestroy ()
    {
        if (this.Map != null)
            this.Map.removeCell(this);

        // TODO move this to OnBeforeSerialization
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            // Previous serialization moment, only interesting in editor
            this.GetComponent<MeshFilter>().sharedMesh = null;
            this.GetComponent<Renderer>().sharedMaterial = null;
            UnityEditor.EditorUtility.SetDirty(this);

            // This will close the cycle of the serialized face saving
            faces = null;
        }
#endif
    }


	/*******************************
	 * BEGIN ATRIBS
	 *******************************/

    public Map Map
    {
        get
        {
            /*if(map == null){
                Debug.Log(transform.gameObject.name + "  -  " + (transform.parent == null));
                map = transform.parent.gameObject.GetComponent<Map>();
            }*/
            return transform.parent.gameObject.GetComponent<Map>();
        }

        set
        {
            transform.parent = value.transform;
        }
    }


	public bool Walkable{
		get{ return walkable;}
		set{ this.walkable = value;}
	}

	public float Height {
		get {
            return properties.height;
		}
		set {
            properties.height = Mathf.RoundToInt(value * 2.0f) / 2.0f;
            if (properties.height < 0)
                properties.height = 0;
		}
	}


	public float WalkingHeight {
		get {

			float extra = 0;
            if (properties.top != CellTopType.flat)
            {
                extra = (properties.top == CellTopType.midPlane) ? 0.25f : 0.5f;
			}

            return properties.height + walkingHeight + extra;
		}
		set {
			float extra = 0;
            if (properties.top != CellTopType.flat)
            {
                extra = (properties.top == CellTopType.midPlane) ? 0.25f : 0.5f;
			}

            walkingHeight = value - properties.height - extra;
		}
	}

    public float Width
    {
        get
        {
            return properties.width;
        }
        set
        {
            properties.width = value;
        }
    }

	public CellTopType CellTop {
		get {
			return properties.top;
		}
		set {
            properties.top = value;
		}
	}


	
	public int CellTopRotation {
		get {
			return properties.orientation;
		}
		set {
            properties.orientation = value % 4;
		}
	}

	/** ********************
	 * END ATRIBS
	 * *********************/

	public Cell(){
	}

	public void refresh(){
        if(this.properties.Changed)
            this.regenerateMesh();
	}

    public void forceRefresh()
    {
        this.regenerateMesh();
    }

	private void regenerateMesh(){

        MeshFactory.Instance.Generate(properties);
        this.GetComponent<MeshFilter>().mesh = MeshFactory.Instance.getMesh();
        this.properties.faces = MeshFactory.Instance.getFaces();
        Material myMat = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
        myMat.SetTexture("_MainTex", MeshFactory.Instance.getTexture2D());
        this.GetComponent<Renderer>().sharedMaterial = myMat;
        this.GetComponent<MeshCollider>().sharedMesh = MeshFactory.Instance.getMesh();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
	}


    private static void extractFacesFromMesh(Mesh mesh, CellProperties properties)
    {

        int numLateralFaces = Mathf.CeilToInt(properties.height) * 4;
		//Numero de vertices de la ultima capa
        int[] topFacesNumVertex = (properties.top == CellTopType.flat) ? new int[1] { 4 } : new int[4] { 3, 4, 3, 4 };

		int verticesForThisFace;
		int totalVertices = 0;
		for (int i = 0; i < numLateralFaces + topFacesNumVertex.Length; i++) {
			// Calculamos cuantos vertices tendra la cara. Si es lateral 4 y sino, los que esten preestablecidos.
			verticesForThisFace = (i < numLateralFaces)?4:topFacesNumVertex[i - numLateralFaces];
            FaceNoSC.extractFaceInfoFromMesh(mesh, verticesForThisFace, totalVertices, properties.faces[i]);
			totalVertices += verticesForThisFace;
		}
	}

    private bool firstOppened = true;


    public FaceNoSC getFaceByPoint(Vector3 point)
    {
        if (firstOppened)
        {
            Cell.extractFacesFromMesh(this.GetComponent<MeshFilter>().sharedMesh, properties);
            firstOppened = false;
        }
        Vector3 inversePoint = transform.InverseTransformPoint(point);
        foreach (FaceNoSC f in properties.faces)
        {
            
            if (f.contains(inversePoint))
				return f;
		}

		return null;
	}

	/**##########################################
	 * 			GAME RELATED
	 * */

	public bool isAccesibleBy(Entity entity){
		foreach(Entity e in getEntities()){
			if(!e.letPass(entity))
				return false;
		}
		return walkable;
	}
	// TODO enhacement improve this like in the map
	public Entity[] getEntities(){
		return this.transform.GetComponentsInChildren<Entity>();
	}
	
	/**
	 * Deprecated
	 */
	public void tick(){
		foreach(Entity en in getEntities()){
			en.tick();
		}
	}

	/* #########################################################
	 * 					ENTITIES THINGS
	 * */

	// TODO entity movement or teleportation maybe checked here? dunno...
	public List<Entity> Entities{get;set;}

	public void addEntity(Entity en){

	}

	public void removeEntity(Entity en){

	}

	/* #########################################################
	 * 					DECORATION THINGS
	 * */

	
	private GameObject ghost;

	public GameObject addGhost(Vector3 position, int angle, bool parallel, bool centered, IsoDecoration dec, float intensity){
		if (this.ghost == null) {
			ghost = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultDecorationPrefab) as GameObject;
			ghost.name = "GhostDer";
			ghost.hideFlags = HideFlags.HideAndDontSave;

			Material ghostMaterial = new Material(Shader.Find("Transparent/Diffuse"));
			ghostMaterial.color = new Color(ghostMaterial.color.r,ghostMaterial.color.g,ghostMaterial.color.b,intensity);
			ghost.GetComponent<Renderer>().sharedMaterial = ghostMaterial;
		}
		Decoration der = ghost.GetComponent<Decoration>();
		der.Father = this;
		der.IsoDec = dec;

		der.setParameters (position,angle,parallel,centered);

		return this.ghost;
	}

	public void removeGhost (){
		if (ghost != null)
			GameObject.DestroyImmediate (ghost);
	}


	public GameObject addDecoration(Vector3 position, int angle, bool parallel, bool centered, IsoDecoration dec){

		GameObject newdecoration = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultDecorationPrefab) as GameObject;
		newdecoration.name = "Decoration (clone)";
		newdecoration.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
		newdecoration.GetComponent<Decoration>().Father = this;

		Decoration der = newdecoration.GetComponent<Decoration>();
		der.IsoDec = dec;

		der.setParameters(position,angle,parallel,centered);

		return newdecoration;
	}
	
	public void removeDecoration(Decoration d){

	}

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (this.Map != null)
                this.Map.registerCell(this);
        }
#endif
	}

	void Update () {
        if (this.properties.Changed)
            this.regenerateMesh();
	}

}
