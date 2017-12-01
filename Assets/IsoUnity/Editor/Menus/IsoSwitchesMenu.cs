using UnityEngine;
using UnityEditor;

namespace IsoUnity {
	public class IsoSwitchesMenu  {
		
		[MenuItem("Assets/Create/IsoSwitches")]
		public static IsoSwitches createSwitches(){
			string ruta = "Assets/Resources/IsoSwitches.asset";
			return IsoAssetsManager.CreateAssetOf<IsoSwitches>(ruta);
		}
	}
}