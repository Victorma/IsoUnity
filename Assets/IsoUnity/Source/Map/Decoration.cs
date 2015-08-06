using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[ExecuteInEditMode]
public class Decoration : MonoBehaviour{
	/*******************************
	 * BEGIN ATRIBS
	 *******************************/

	// The cell face that is asigned to
	[SerializeField]
	private IsoDecoration isoDec;

	public IsoDecoration IsoDec {
		get { return isoDec;}
		set { isoDec = value; }
	}
	[SerializeField]
	private Object father;

	public Object Father {
		get { return father;}
		set { father = value; }
	}

	[SerializeField]
	private Vector3 center;
	
	public Vector3 Center {
		get { return center;}
		set { center = value; }
	}

	[SerializeField]
	private int angle;

	public int Angle {
		get { return angle;}
		set { angle = value; }
	}

	[SerializeField]
	private bool parallel;

	public bool Parallel {
		get { return parallel;}
		set { parallel = value; }
	}

	[SerializeField]
	private bool centered;
	
	public bool Centered {
		get { return centered;}
		set { centered = value; }
	}

	private Vector3[] quadVertices;

	/** ********************
	 * END ATRIBS
	 * *********************/

	public Decoration (){
	}

	public void setParameters(Vector3 center, int angle, bool parallel, bool centered){
		this.center = center;
		this.angle = angle;
		this.parallel = parallel;
		this.centered = centered;

		adaptate ();
	}

	public void adaptate(){
		this.updateTextures ();
		this.colocate ();
		this.setRotation ();
	}

	public void updateTextures(){
		float scale = Mathf.Sqrt (2f) / IsoSettingsManager.getInstance ().getIsoSettings ().defautTextureScale.width;

		this.transform.localScale = new Vector3(isoDec.getTexture().width * scale / ((float)isoDec.nCols),(isoDec.getTexture().height * scale) / ((float)isoDec.nRows),1);

		if (!parallel)
			this.transform.localScale = new Vector3 (this.transform.localScale.x,this.transform.localScale.y / Mathf.Sin (45f),1);
		else {
			if(this.angle==0)
				this.transform.localScale = new Vector3 (this.transform.localScale.x,this.transform.localScale.y*2f,1);
			else{
				this.transform.localScale = new Vector3 (Mathf.Sqrt(2f*(this.transform.localScale.x*this.transform.localScale.x)),this.transform.localScale.y / Mathf.Sin(45f),1);

				Mesh mesh = this.GetComponent<MeshFilter>().mesh;


				if(quadVertices==null){
					quadVertices = new Vector3[mesh.vertices.Length];
					for(int i=0; i<mesh.vertices.Length; i++) 
						quadVertices[i] = new Vector3(mesh.vertices[i].x,mesh.vertices[i].y,mesh.vertices[i].z); 
				}

				Vector3[] vertices = new Vector3[quadVertices.Length];
				for(int i=0; i<quadVertices.Length; i++) 
					vertices[i] = new Vector3(quadVertices[i].x,quadVertices[i].y,quadVertices[i].z); 

				float xprima = this.transform.localScale.x;
				float omega = xprima*0.57735026f;
				float gamma = omega/(this.transform.localScale.y*Mathf.Sqrt(2));

				Vector3 bajada = new Vector3(0,gamma,0);

				if(this.angle==2){
					vertices[0] -= bajada;
					vertices[3] -= bajada;
				}else if(this.angle==1){
					vertices[1] -= bajada;
					vertices[2] -= bajada;
				}

				mesh.vertices = vertices;

				this.GetComponent<MeshFilter>().sharedMesh = mesh;
			}
		}

		Material myMat = new Material(this.GetComponent<Renderer>().sharedMaterial);
		myMat.mainTextureScale = new Vector2 (1f/((float)isoDec.nCols), 1f/((float)isoDec.nRows));
		myMat.mainTextureOffset = new Vector2 (0, 1- 1f/((float)isoDec.nRows));
		myMat.SetTexture("_MainTex",isoDec.getTexture());
		this.GetComponent<Renderer>().sharedMaterial = myMat;


	}

	public void colocate(){
		if (this.father is Cell) {
			Cell celdapadre = this.father as Cell;
			this.transform.parent = celdapadre.transform;
			Vector3 invfather = celdapadre.transform.InverseTransformPoint (this.center);

			//###################
			//        / \
			//       /   \
			//      /     \
			//     /   0   \
			//    |\       /|
			//    | \     / |
			//    |  \   /  |
			//    | 2 \_/ 1 |
			//     \   |   /
			//      \  |  /
			//       \ | /
			//        \|/
			//####################

			Vector3 position = new Vector3 ();
			this.transform.localRotation = celdapadre.transform.rotation;

			// Segun la zona de actuacion, definiremos la posicion de una manera u otra.
			switch (this.angle) {
			case 0:
				{
					if (this.centered)
						if (!this.parallel)
							position = new Vector3 (-0.5f, (celdapadre.Height * celdapadre.Width) + this.transform.localScale.y / 2, -0.5f);
						else
                            position = new Vector3(-0.5f, (celdapadre.Height * celdapadre.Width + 0.01f), -0.5f);
					else
						if (!this.parallel)
                            position = new Vector3(invfather.x, (celdapadre.Height * celdapadre.Width) + this.transform.localScale.y / 2, invfather.z);
						else
                            position = new Vector3(invfather.x, (celdapadre.Height * celdapadre.Width) + 0.01f, invfather.z);
					break;
				}
			case 1:
				{
					if (this.centered)
						if (!this.parallel)
                            position = new Vector3(-celdapadre.Width / 2 + ((this.transform.localScale.x / 2) * Mathf.Cos(45 * Mathf.Deg2Rad)), invfather.y - (invfather.y % celdapadre.Width) + 1, -celdapadre.Width / 2 - ((this.transform.localScale.x / 2) * Mathf.Cos(45 * Mathf.Deg2Rad)));
						else
                            position = new Vector3(-celdapadre.Width / 2, invfather.y - (invfather.y % celdapadre.Width) + 1 + (this.transform.localScale.y / 2), -0.01f - celdapadre.Width / 2);
					else
						if (!this.parallel)
							position = new Vector3 (invfather.x + ((this.transform.localScale.x / 2) * Mathf.Cos (45 * Mathf.Deg2Rad)), invfather.y, invfather.z - ((this.transform.localScale.x / 2) * Mathf.Cos (45 * Mathf.Deg2Rad)));
						else
                            position = new Vector3(invfather.x, invfather.y + (this.transform.localScale.y / 2), -0.01f - celdapadre.Width / 2);
					break;
				}
			case 2:
				{
					if (this.centered)
						if (!this.parallel)
                            position = new Vector3(-celdapadre.Width / 2 - ((this.transform.localScale.x / 2) * Mathf.Cos(45 * Mathf.Deg2Rad)), invfather.y - (invfather.y % celdapadre.Width) + 1, -celdapadre.Width / 2 + ((this.transform.localScale.x / 2) * Mathf.Cos(45 * Mathf.Deg2Rad)));
						else
                            position = new Vector3(-0.01f - celdapadre.Width / 2, invfather.y - (invfather.y % celdapadre.Width) + 1 + (this.transform.localScale.y / 2), -celdapadre.Width / 2);
					else
						if (!this.parallel)
							position = new Vector3 (invfather.x - ((this.transform.localScale.x / 2) * Mathf.Cos (45 * Mathf.Deg2Rad)), invfather.y, invfather.z + ((this.transform.localScale.x / 2) * Mathf.Cos (45 * Mathf.Deg2Rad)));
						else
                            position = new Vector3(-0.01f - celdapadre.Width / 2, invfather.y + (this.transform.localScale.y / 2), invfather.z);	
					break;
				}
			}

			if (!this.centered)
					this.transform.localPosition = invfather;

			this.transform.localPosition = position;
		}else if(this.father is Decoration){
			Decoration decorationpadre = this.father as Decoration;
			this.transform.parent = decorationpadre.transform;

			Vector3 position = new Vector3 ();

			position = new Vector3 (0f, this.transform.localScale.y, 0f);

			this.transform.localPosition = position;
		}
	}

	[SerializeField]
	private int tile =0;
	public int Tile {
		get{
			return tile;
		}
		set{
			tile = value;
			int x = tile % (isoDec.nCols);
			int y = Mathf.FloorToInt(tile/isoDec.nCols);
			
			this.GetComponent<Renderer>().material.mainTextureOffset = new Vector2 ( (x/((float)isoDec.nCols)),  (y/((float)isoDec.nRows)));
		}
	}

	public void setRotation(){
		float x = 0, y = 45, z = 0;

		if (this.father is Cell) {
			this.transform.localRotation = (this.father as Cell).transform.rotation;

			if (this.parallel) {
				switch (this.angle) {
				case 0:{x=90; y=45; break;}
				case 1:{y=0; break;}
				case 2:{y=90;break;}
				}
			}
		}

		else if (this.father is Decoration){
			this.transform.localRotation = (this.father as Decoration).transform.rotation;
			y = -45;
		}

		this.transform.Rotate (x, y, z);
	}
}

