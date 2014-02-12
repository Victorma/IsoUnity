using UnityEngine;
using System.Collections;

public class Texturizador : MonoBehaviour {
	
	public Texture2D superior;
	public bool superiorEsIsometrica;
	public Texture2D lateral;
	public bool lateralEsIsometrica;
	public Texture2D inferior;
	public bool inferiorEsIsometrica;
	
	private Texture2D[] AllCubeTextures;

	public Texture2D TextureAtlas;
	
	public Rect[] AtlasUvs;
	
	public Vector3[] vertices;
	public Vector2[] uv;
	
	public int[] triangles;
	
	public Rect[] posTexturas;
	
	public GameObject go;
	
	// Use this for initialization
	void Start () {
		TextureAtlas = new Texture2D(10,10);
		
		AllCubeTextures = new Texture2D[3];
		AllCubeTextures[0] = superior;
		AllCubeTextures[1] = lateral;
		AllCubeTextures[2] = inferior;
		
		posTexturas = TextureAtlas.PackTextures(AllCubeTextures,0);
		Mesh mesh = (this.GetComponent("MeshFilter") as MeshFilter).mesh;
		
		vertices = mesh.vertices;
		triangles = mesh.triangles;
		Vector2[] uvs = new Vector2[vertices.Length];
		
		
		// Lateral frontal
		uvs[0] = new Vector2(posTexturas[1].x,posTexturas[1].y);
		uvs[1] = new Vector2(posTexturas[1].x + posTexturas[1].width,posTexturas[1].y);
		uvs[2] = new Vector2(posTexturas[1].x,posTexturas[1].y + posTexturas[1].height);
		uvs[3] = new Vector2(posTexturas[1].x + posTexturas[1].width,posTexturas[1].y + posTexturas[1].height);
		
		
		// Superior 
		if(superiorEsIsometrica)
		{
			//Esquina inferior izquierda
			uvs[4] = new Vector2(posTexturas[0].x,posTexturas[0].y + posTexturas[0].height*0.5f);
			//Esquina inferior derecha
			uvs[5] = new Vector2(posTexturas[0].x + posTexturas[0].width*0.5f,posTexturas[0].y);
			//Esquina superior izquierda
			uvs[8] = new Vector2(posTexturas[0].x + posTexturas[0].width*0.5f,posTexturas[0].y + posTexturas[0].height);
			//Esquina superior derecha
			uvs[9] = new Vector2(posTexturas[0].x + posTexturas[0].width,posTexturas[0].y + posTexturas[0].height*0.5f);
		}
		else
		{
			uvs[4] = new Vector2(posTexturas[0].x, posTexturas[0].y);
			uvs[5] = new Vector2(posTexturas[0].x + posTexturas[0].width,posTexturas[0].y);
			uvs[8] = new Vector2(posTexturas[0].x,posTexturas[0].y + posTexturas[0].height);
			uvs[9] = new Vector2(posTexturas[0].x + posTexturas[0].width,posTexturas[0].y + posTexturas[0].height);
		}
		
		uvs[6] = mesh.uv[6]; 
		uvs[7] = mesh.uv[7];


		
		/*
		uvs[8] = new Vector2(0,0);
		uvs[9] = new Vector2(0.5f,0);
		uvs[10] = new Vector2(0,1);
		uvs[11] = new Vector2(0.5f,1);
		
		uvs[12] = new Vector2(0,0);
		uvs[13] = new Vector2(0.5f,0);
		uvs[14] = new Vector2(0,1);
		uvs[15] = new Vector2(0.5f,1);
		
		uvs[16] = new Vector2(0,0);
		uvs[17] = new Vector2(0.5f,0);
		uvs[18] = new Vector2(0,1);
		uvs[19] = new Vector2(0.5f,1);
		
		uvs[20] = new Vector2(0,0);
		uvs[21] = new Vector2(0.5f,0);
		uvs[22] = new Vector2(0,1);
		uvs[23] = new Vector2(0.5f,1);
		*/
		for (int i = 10 ; i < uvs.Length; i++)
			uvs[i] = mesh.uv[i];
	
		mesh.uv = uvs;
		
		uv = mesh.uv;
		this.renderer.material.SetTexture("_MainTex",TextureAtlas);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
