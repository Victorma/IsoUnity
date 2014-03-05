using UnityEngine;
using System.Collections;

public class IsoDecoration : ScriptableObject
{
	[SerializeField]
	Texture2D realTexture;
	[SerializeField]
	int nrows = 1;
	public int nRows {
		get { return nrows;}
		set { nrows = value; }
	}
	[SerializeField]
	int ncols = 1;
	public int nCols {
		get { return ncols;}
		set { ncols = value; }
	}


	public void setTexture(Texture2D texture){
		this.realTexture = texture;
		DecorationManager.getInstance ().update (this);
	}

	public Texture2D getTexture(){
		return realTexture;
	}
}


