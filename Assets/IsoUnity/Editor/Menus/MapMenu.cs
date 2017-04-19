using UnityEngine;
using UnityEditor;

public class MapMenu {

	[MenuItem("GameObject/IsoUnity/IsoUnity Map", false, 10)]
	public static void createMap(MenuCommand menuCommand){

		#if UNITY_EDITOR
		GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(IsoSettingsManager.getInstance().getIsoSettings().defaultMapPrefab) as GameObject;
		#else
		GameObject go = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultMapPrefab);
		#endif

		Selection.activeObject = go;    
	}

}
