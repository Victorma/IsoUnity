using UnityEngine;
using UnityEditor;

public class TextureMenu {

	[MenuItem("Assets/Create/IsoTexture")]
	public static void createIsoTextureAsset(){
		IsoAssetsManager.CreateAssetInCurrentPathOf ("IsoTexture");
	}
}
