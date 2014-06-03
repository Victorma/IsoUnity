using UnityEngine;
using UnityEditor;

public class MapMenu {

	[MenuItem("GameObject/Create Other/IsoUnity Map")]
	public static void createMap(){
		GameObject go = GameObject.Instantiate(IsoSettingsManager.getInstance().getIsoSettings().defaultMapPrefab) as GameObject;
		Selection.activeObject = go;    
	}
}
