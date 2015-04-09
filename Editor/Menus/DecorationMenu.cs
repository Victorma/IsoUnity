using UnityEngine;
using UnityEditor;

public class DecorationMenu  {

	[MenuItem("Assets/Create/IsoDecoration")]
	public static void createIsoTextureAsset(){
		IsoAssetsManager.CreateAssetInCurrentPathOf ("IsoDecoration");   
	}
}
