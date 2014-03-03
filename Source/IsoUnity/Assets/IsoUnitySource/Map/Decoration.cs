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

	private Vector3 Position;

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

		this.transform.localScale = new Vector3(isoDec.getTexture().width * scale,
		                                        (isoDec.getTexture().height * scale)/*/(Mathf.Cos(45 * Mathf.Deg2Rad))*/,1);
		this.renderer.material.SetTexture("_MainTex",isoDec.getTexture());
	}

	public void colocate(Vector3 v, int angle, bool parallel, bool centered){
		
		this.transform.parent = this.father.transform;
		Vector3 invfather = this.father.transform.InverseTransformPoint (v);



		if (centered)
			switch (angle) {
				case 0:{
					this.transform.localPosition = new Vector3(-0.5f,this.transform.localScale.y,-0.5f);	
					break;}
				case 1:{
				this.transform.localPosition = new Vector3(this.transform.localScale.x,this.transform.localScale.y,-0.5f);
					break;}
				case 2:{
				this.transform.localPosition = new Vector3(this.transform.localScale.x,this.transform.localScale.y,-0.5f);
					break;}
			}
		else
			this.transform.localPosition = invfather;

		setRotation (angle, parallel);

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

