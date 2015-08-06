using UnityEngine;
using UnityEditor;
using System.Collections;

public class IsoAssetsManager  {

	public static ScriptableObject CreateAssetOf(string name, string ruta){

		ScriptableObject so = ScriptableObject.CreateInstance (name);
		AssetDatabase.CreateAsset(so, ruta);
		Selection.activeObject = so;  

		return so;
	}

	public static ScriptableObject CreateAssetInCurrentPathOf(string name){
		
		ScriptableObject so = ScriptableObject.CreateInstance (name);
		ProjectWindowUtil.CreateAsset(so, name+".asset");
		Selection.activeObject = so;  
		
		return so;
	}
}
