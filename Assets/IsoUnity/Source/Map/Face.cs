using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Face : ScriptableObject, IComparable<Face> {
	
	private int[] vertexIndex;
	private int[] finalVertexListIndexes;

	public int[] VertexIndex {
		get {
			return vertexIndex;
		}
		set {
			vertexIndex = value;
			
			finalVertexListIndexes = new int[vertexIndex.Length];
			for(int i = 0; i< vertexIndex.Length; i++){
				finalVertexListIndexes[i] = finalVertexList.Count;
				finalVertexList.Add(sharedVertex[vertexIndex[i]]);
			}
		}
	}
	
	private Vector3[] sharedVertex;
	public Vector3[] SharedVertex {
		get {
			return sharedVertex;
		}
		set {
			sharedVertex = value;
		}
	}

	private List<Vector3> finalVertexList;
	public List<Vector3> FinalVertexList {	
		get {
			return finalVertexList;
		}
		set {
			finalVertexList = value;
		}
	}

	[SerializeField]
	private Texture2D texture;
	public Texture2D Texture {
		get {
			return texture;
		}
		set {
			texture = value;
		}
	}

    [SerializeField]
	private IsoTexture textureMapping;
	public IsoTexture TextureMapping {
		get {
			return textureMapping;
		}
		set {
			textureMapping = value;
		}
	}

	private int[] triangles;
	public int[] Triangles {
		get {
			return triangles;
		}
	}

	private Vector2[] uvs;
	public Vector2[] Uvs {
		get {
			return uvs;
		}
	}
	
	public void regenerateTriangles(){
		
		this.triangles = new int[6];
		
		triangles[0] = finalVertexListIndexes[0];
		triangles[1] = finalVertexListIndexes[2];
		triangles[2] = finalVertexListIndexes[1];
		
		if(vertexIndex.Length == 4){
			triangles[3] = finalVertexListIndexes[2];
			triangles[4] = finalVertexListIndexes[0];
			triangles[5] = finalVertexListIndexes[3];
		}
		
	}

	public static Face extractFaceInfoFromMesh(Mesh mesh, int numVertices, int offset){
        Face face = new Face();

		face.sharedVertex = mesh.vertices;
		face.vertexIndex = new int[numVertices];
		for (int i = 0; i < numVertices; i++) 
			face.vertexIndex[i] = offset+i;

		face.finalVertexListIndexes = face.vertexIndex;
		face.regenerateTriangles ();

		return face;
	}
	
	public void regenerateUVs(Rect textureRect){
		
		this.uvs = new Vector2[vertexIndex.Length];
		
		if(vertexIndex.Length == 4){
			Vector2 topLeft = new Vector2(textureRect.x, textureRect.y + textureRect.height);
			Vector2 topRight = new Vector2(textureRect.x + textureRect.width, textureRect.y + textureRect.height);
			Vector2 botLeft = new Vector2(textureRect.x, textureRect.y);
			Vector2 botRight = new Vector2(textureRect.x + textureRect.width, textureRect.y);
			
			int cornerBotLeft 	= 0,
				cornerBotRight 	= 1,
				cornerTopRight	= 2,
				cornerTopLeft	= 3;
			
			if(textureMapping != null){
				cornerBotLeft = (cornerBotLeft + textureMapping.Rotation)%4;
				cornerBotRight = (cornerBotRight + textureMapping.Rotation)%4;
				cornerTopLeft = (cornerTopLeft + textureMapping.Rotation)%4;
				cornerTopRight = (cornerTopRight + textureMapping.Rotation)%4;
			}
			
			uvs[cornerBotLeft] = botLeft;
			uvs[cornerBotRight] = botRight;
			uvs[cornerTopRight] = topRight;
			uvs[cornerTopLeft] = topLeft;
			
			if(textureMapping != null){
				uvs[cornerBotLeft] += new Vector2(0,textureRect.height * (1f - (textureMapping.getYCorner() / (textureMapping.getTexture().height*1.0f))));
				uvs[cornerBotRight] -= new Vector2(textureRect.width * (1f - (textureMapping.getOppositeXCorner() / (textureMapping.getTexture().width*1.0f))), 0);
				uvs[cornerTopRight] -= new Vector2(0, textureRect.height * (textureMapping.getOppositeYCorner() / (textureMapping.getTexture().height*1.0f)));
				uvs[cornerTopLeft] += new Vector2(textureRect.width * (textureMapping.getXCorner() / (textureMapping.getTexture().width*1.0f)),0); 
			}
		}else if(vertexIndex.Length == 3) {
			Vector2 topRight = new Vector2(textureRect.x + textureRect.width, textureRect.y + textureRect.height);
			Vector2 botLeft = new Vector2(textureRect.x, textureRect.y);
			Vector2 botRight = new Vector2(textureRect.x + textureRect.width, textureRect.y);
			
			int cornerBotLeft 	= 2,
				cornerBotRight 	= 1,
				cornerTopRight	= 0;
			
			if(textureMapping != null){
				cornerBotLeft = (cornerBotLeft + textureMapping.Rotation)%3;
				cornerBotRight = (cornerBotRight + textureMapping.Rotation)%3;
				cornerTopRight = (cornerTopRight + textureMapping.Rotation)%3;
			}
			
			uvs[cornerBotLeft] = botLeft;
			uvs[cornerBotRight] = botRight;
			uvs[cornerTopRight] = topRight;
			
			if(textureMapping != null){
				uvs[cornerBotLeft] += new Vector2(0,textureRect.height * (1f - (textureMapping.getYCorner() / (textureMapping.getTexture().height*1.0f))));
				uvs[cornerBotRight] -= new Vector2(textureRect.width * (1f - (textureMapping.getOppositeXCorner() / (textureMapping.getTexture().width*1.0f))), 0);
				uvs[cornerTopRight] -= new Vector2(0, textureRect.height * (textureMapping.getOppositeYCorner() / (textureMapping.getTexture().height*1.0f)));
			}
		}
	}

	/* ##############################################
	 * 					Bounds related
	 * */
	
	private bool boundsGenerated = false;
	private Bounds bounds;
	private void generateBounds(){
		if(sharedVertex == null)
			return;
		
		Mesh meshecilla = new Mesh();
		Vector3[] puntos = new Vector3[vertexIndex.Length];
		for(int i = 0; i< puntos.Length; i++){
			puntos[i] = sharedVertex[vertexIndex[i]];
		}
		int[] trianglecillos;
		if(vertexIndex.Length == 3){
			trianglecillos = new int[3];
			trianglecillos[0] = 0;
			trianglecillos[1] = 2;
			trianglecillos[2] = 1;
		}else{
			trianglecillos = new int[6];
			
			trianglecillos[0] = 0;
			trianglecillos[1] = 2;
			trianglecillos[2] = 1;
			trianglecillos[3] = 2;
			trianglecillos[4] = 0;
			trianglecillos[5] = 3;
		}
		
		meshecilla.vertices = puntos;
		meshecilla.triangles = trianglecillos; 
		meshecilla.RecalculateBounds();
		
		bounds = meshecilla.bounds;
		boundsGenerated = true;
		
		/*bounds = new Bounds();
		foreach(int i in vertexIndex)
			bounds.Encapsulate(sharedVertex[i]);*/
	}
	
	public bool contains(Vector3 point){
		if(!boundsGenerated)
			generateBounds();
		return bounds.Contains(point);
	}


    public int CompareTo(Face other)
    {
        return (this.texture == other.texture && this.textureMapping == other.textureMapping) ? 0 : 1;
    }

};

