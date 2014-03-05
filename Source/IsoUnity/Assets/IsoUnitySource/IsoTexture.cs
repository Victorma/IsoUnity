using UnityEngine;
using System.Collections;

public class IsoTexture : ScriptableObject
{
	[SerializeField]
	Texture2D realTexture;
	[SerializeField]
	int xCorner;
	[SerializeField]
	int yCorner;
	[SerializeField]
	bool isXSimetric;
	[SerializeField]
	bool isYSimetric;
	//Rotation has 4 values what means rotation*90 as que real rotation
	[SerializeField]
	int rotation;

	public int Rotation {
		get {
			return rotation;
		}
		set {
			rotation = (int)Mathf.Abs(value%4);
		}
	}

	public IsoTexture ()
	{
		rotation = 0;
	}


	public void setTexture(Texture2D texture){
		this.realTexture = texture;
		TextureManager.getInstance ().update (this);
	}

	public Texture2D getTexture(){
		return realTexture;
	}

	public void setXCorner(int xCorner){
		this.xCorner = xCorner;
	}

	public int getXCorner(){
		return xCorner;
	}

	public int getOppositeXCorner(){
		if(realTexture == null)
			return 0;

		return realTexture.width - xCorner;
	}

	public void setYCorner(int yCorner){
		this.yCorner = realTexture.height - yCorner;
	}

	public int getYCorner(){
		if(realTexture == null)
			return 0;

		return realTexture.height - yCorner;
	}
	
	public int getOppositeYCorner(){
		return yCorner;
	}

	public Rect getTextureRect(){
		Rect rect = new Rect(
			(1f - (getYCorner() / (getTexture().height*1.0f))),
			(1f - (getOppositeXCorner() / (getTexture().width*1.0f))),
		    (getOppositeYCorner() / (getTexture().height*1.0f)),
			(getXCorner() / (getTexture().width*1.0f)));
		return rect;
	}

}


