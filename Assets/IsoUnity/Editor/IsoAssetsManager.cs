using UnityEngine;
using UnityEditor;
using System.Collections;

namespace IsoUnity {
	public class IsoAssetsManager  {

	    public static T CreateAssetOf<T>(string ruta) where T : ScriptableObject
	    {
	        T so = ScriptableObject.CreateInstance<T>();
	        AssetDatabase.CreateAsset(so, ruta);
	        Selection.activeObject = so;

	        return so;
	    }

	    public static ScriptableObject CreateAssetOf(string name, string ruta){

			ScriptableObject so = ScriptableObject.CreateInstance (name);
			AssetDatabase.CreateAsset(so, ruta);
			Selection.activeObject = so;  

			return so;
		}

		public static ScriptableObject CreateAssetInCurrentPathOf(string name){
			
			ScriptableObject so = ScriptableObject.CreateInstance (name);
            AssetDatabase.CreateAsset(so,"Assets/" + name+".asset");
            AssetDatabase.SaveAssets();
			Selection.activeObject = so;  
			
			return so;
		}
	}
}