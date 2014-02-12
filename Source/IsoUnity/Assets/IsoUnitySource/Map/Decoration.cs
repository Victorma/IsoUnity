using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[ExecuteInEditMode]
public class Decoration : MonoBehaviour{
	/*******************************
	 * BEGIN ATRIBS
	 *******************************/

	// The cell face that is asigned to
	private IsoDecoration isoDec;

	public IsoDecoration IsoDec {
		get { return isoDec;}
		set { isoDec = value; }
	}

	private Cell father;

	public Cell Father {
		get { return father;}
		set { father = value; }
	}

	private float x;
	
	public float X {
		get {return x;}
		set {x = value;}
	}

	private float y;
	
	public float Y {
		get {return y;}
		set {y = value;}
	}
	
	// The face where the decoration object is represented
	private Face decorationFace;
	
	public Face DecorationFace {
		get {
			return decorationFace;
		}
		set {
			decorationFace = value;
		}
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
		float scale = this.isoDec.getTexture().width / (IsoSettingsManager.getInstance ().getIsoSettings ().defautTextureScale.width*1.0f);

		// TODO MULTIPLICAR POR EL ANCHO DE LA CELDA
		//this.transform.localScale = (new Vector3 (scale,scale, 1);
		this.transform.localScale = new Vector3(isoDec.getTexture().width * scale /10,isoDec.getTexture ().height * scale/10,1);
		this.renderer.material.SetTexture("_MainTex",isoDec.getTexture());
	}

	public void setPosition(Vector3 v){
		//x = v.y - father.transform.localPosition.y;
		this.transform.localPosition = new Vector3(v.x, v.y+(this.transform.localScale.y/2), v.z);
	}
}

