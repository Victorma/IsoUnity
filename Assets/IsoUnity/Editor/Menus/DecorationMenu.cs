using UnityEngine;
using UnityEditor;

namespace IsoUnity {
	public class DecorationMenu  {

		[MenuItem("Assets/Create/IsoDecoration")]
		public static void createIsoTextureAsset(){
			IsoAssetsManager.CreateAssetInCurrentPathOf ("IsoDecoration");   
		}
	}
}