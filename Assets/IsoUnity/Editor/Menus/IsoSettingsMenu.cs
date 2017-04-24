using UnityEngine;
using UnityEditor;

namespace IsoUnity {
	public class IsoSettingsMenu  {
			
		[MenuItem("Assets/Create/IsoSettings")]
		public static IsoSettings createSettings(){
			string ruta = "Assets/Resources/IsoSettings.asset";
			return IsoAssetsManager.CreateAssetOf("IsoSettings", ruta) as IsoSettings;
		}
	}
}