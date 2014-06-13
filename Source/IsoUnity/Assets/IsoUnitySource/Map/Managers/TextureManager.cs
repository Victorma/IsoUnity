using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TextureManager {
	
	private static TextureManager instance;
	public static TextureManager getInstance(){
		if(instance == null){
			instance = new TextureManagerInstance();
		}
		return instance;
	}
	
	public abstract IsoTexture newTexture();
	public abstract IsoTexture[] textureList();
	public abstract IsoTexture[] textureList (Texture match);
	public abstract void deleteTexture(IsoTexture texture);
	public abstract void update (IsoTexture texture);
	
}

public class TextureManagerInstance : TextureManager {

	private Dictionary<Texture, List<IsoTexture>> lists;
	
	public TextureManagerInstance(){

		regenerate (null);
	}

	public override void update(IsoTexture it){
		regenerate (it.getTexture ());
	}

	private void regenerate(Texture referenced){

		if (referenced == null) {

			lists = new Dictionary<Texture, List<IsoTexture>> ();
			foreach (IsoTexture it in textureList()){
				if(it.getTexture()!=null){
					if(!lists.ContainsKey(it.getTexture()))
						lists.Add(it.getTexture(), new List<IsoTexture>());

					lists[it.getTexture()].Add(it);
				}
			}

		} else {

			List<IsoTexture> n = new List<IsoTexture> ();

			foreach (IsoTexture it in textureList())
					if (it.getTexture () == referenced)
							n.Add (it);

			lists [referenced] = n;
		}
	}
	
	public override IsoTexture newTexture(){
//		createIsoTextureAsset();
		return null;
	}
	public override IsoTexture[] textureList(){
		return Resources.FindObjectsOfTypeAll(typeof(IsoTexture)) as IsoTexture[];
	}

	public override IsoTexture[] textureList(Texture match){

		if (!lists.ContainsKey (match))
			regenerate (match);

		return lists [match].ToArray ();
	}

	public override void deleteTexture(IsoTexture texture){
		//AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(texture));
	}
}
