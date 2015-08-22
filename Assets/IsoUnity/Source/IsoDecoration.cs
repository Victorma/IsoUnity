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

    public Rect getRealRectFor(int row, int col)
    {
        Rect r = new Rect(0, 0, 0, 0);
        if (this.realTexture != null)
        {
            float W = this.realTexture.width / ncols;
            float H = this.realTexture.height / nrows;

            r = new Rect(col * W, row * H, W, H);
        }

        return r;
    }

    public Rect getRealRectFor(int tile)
    {
        tile = tile % (ncols * nrows);

        return(getRealRectFor(tile/ncols, tile%ncols));
    }

    public Rect getTextCoordsFor(int tile)
    {
        Rect r = getRealRectFor(tile);

        return new Rect(r.x / realTexture.width, r.y / realTexture.height, r.width / realTexture.width, r.height / realTexture.height);
    }
}


