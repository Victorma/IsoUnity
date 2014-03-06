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
	private Cell father;

	public Cell Father {
		get { return father;}
		set { father = value; }
	}
	

	private int Angle {
		get { return Angle;}
		set { Angle = value; }
	}


	/** ********************
	 * END ATRIBS
	 * *********************/

	public Decoration (){
	}

	public void refresh(){
		this.updateTextures();
		//this.updatePosition ();
		//transform.LookAt(Camera.main.transform.position, -Vector3.up);
	}

	private void updateTextures(){
		float scale = Mathf.Sqrt (2f) / IsoSettingsManager.getInstance ().getIsoSettings ().defautTextureScale.width;

		/*float angulo = Mathf.Rad2Deg * Mathf.Acos(IsoSettingsManager.getInstance().getIsoSettings().defautTextureScale.height / (IsoSettingsManager.getInstance ().getIsoSettings ().defautTextureScale.width*1f));
		angulo = 90 - Mathf.Abs(angulo);*/
		this.transform.localScale = new Vector3(isoDec.getTexture().width * scale / ((float)isoDec.nCols),
			                                        (isoDec.getTexture().height * scale) / ((float)isoDec.nRows),1);
		this.renderer.material.mainTextureScale = new Vector2 (1f/((float)isoDec.nCols), 1f/((float)isoDec.nRows));
		this.renderer.material.mainTextureOffset = new Vector2 (0, 1- 1f/((float)isoDec.nRows));
		this.renderer.material.SetTexture("_MainTex",isoDec.getTexture());

	}

	public void colocate(Vector3 v, int angle, bool rotate, bool parallel, bool centered){
		
		this.transform.parent = this.father.transform;
		Vector3 invfather = this.father.transform.InverseTransformPoint (v);

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

		// Segun la zona de actuacion, definiremos la posicion de una manera u otra.
		switch (angle) {
		case 0:{
				if(centered)
					position = new Vector3(-0.5f,(this.father.Height * this.father.getCellWidth()) + this.transform.localScale.y/2,-0.5f);
				else
					position = new Vector3(invfather.x,(this.father.Height * this.father.getCellWidth()) + this.transform.localScale.y/2,invfather.z);
				break;
			}
		case 1:{
			if(!rotate){
				if(centered)
					position = new Vector3(-this.father.getCellWidth()/2 +((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)), invfather.y-(invfather.y%this.father.getCellWidth())+1,-this.father.getCellWidth()/2 -((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)));
				else
					position = new Vector3(invfather.x +((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)), invfather.y, invfather.z -((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)));

			}
			break;
		}
		case 2:{
			if(!rotate){
				if(centered)
					position = new Vector3(-this.father.getCellWidth()/2 -((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)), invfather.y-(invfather.y%this.father.getCellWidth())+1,-this.father.getCellWidth()/2 +((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)));
				else
					position = new Vector3(invfather.x -((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)), invfather.y, invfather.z +((this.transform.localScale.x/2)*Mathf.Cos(45*Mathf.Deg2Rad)));
				
			}
			break;
		}
		}

		//Ponemos la altura en su sitio
		if (!centered) 
			this.transform.localPosition = invfather;


		this.transform.localPosition = position;
		//setRotation (angle, parallel);

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
			
			this.renderer.material.mainTextureOffset = new Vector2 ( (x/((float)isoDec.nCols)),  (y/((float)isoDec.nRows)));
		}

	}

	public void setRotation(int angle, bool parallel){
		//x = v.y - father.transform.localPosition.y;
		float x = 0, y = 45, z = 0;

		if (parallel) {
			switch (angle) {
			case 0:{z=0;	y=45;	x=90;	break;}
			case 1:{z=270;	y=0;	break;}
			case 2:{z=90;	y=90;	break;}
			}
		}else{
			switch (angle) {
			case 0:{z=0;
				break;}
			case 1:{z=270;break;}
			case 2:{z=90;break;}
			}
		}


		this.transform.localRotation = new Quaternion ();
		this.transform.Rotate (x, y, z);
	}
}

