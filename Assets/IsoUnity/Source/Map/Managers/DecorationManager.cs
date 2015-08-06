using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DecorationManager {
	
	private static DecorationManager instance;
	public static DecorationManager getInstance(){
		if(instance == null){
			instance = new DecorationManagerInstance();
		}
		return instance;
	}
	
	public abstract IsoDecoration newTexture();
	public abstract IsoDecoration[] textureList();
	public abstract IsoDecoration[] textureList (Texture match);
	public abstract void deleteTexture(IsoDecoration Decoration);
	public abstract void update (IsoDecoration Decoration);
	
}

public class DecorationManagerInstance : DecorationManager {

	private Dictionary<Texture, List<IsoDecoration>> lists;
	
	public DecorationManagerInstance(){

		regenerate (null);
	}

	public override void update(IsoDecoration it){
		regenerate (it.getTexture ());
	}

	private void regenerate(Texture referenced){

		if (referenced == null) {

			lists = new Dictionary<Texture, List<IsoDecoration>> ();
			foreach (IsoDecoration it in textureList()){
				if(!lists.ContainsKey(it.getTexture()))
					lists.Add(it.getTexture(), new List<IsoDecoration>());

				lists[it.getTexture()].Add(it);
			}

		} else {

			List<IsoDecoration> n = new List<IsoDecoration> ();

			foreach (IsoDecoration it in textureList())
					if (it.getTexture () == referenced)
							n.Add (it);

			lists [referenced] = n;
		}
	}
	
	public override IsoDecoration newTexture(){
		//createIsoTextureAsset();
		return null;
	}
	public override IsoDecoration[] textureList(){
		return Resources.FindObjectsOfTypeAll(typeof(IsoDecoration)) as IsoDecoration[];
	}

	public override IsoDecoration[] textureList(Texture match){

		if (!lists.ContainsKey (match))
			regenerate (match);

		return lists [match].ToArray ();
	}

	public override void deleteTexture(IsoDecoration texture){
		//AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(texture));
	}
}
