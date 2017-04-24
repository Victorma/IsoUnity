using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	internal class SequenceNodeAsset : SequenceNode {

	    public override object Content
	    {
	        get
	        {
	            return base.Content;
	        }

	        set
	        {
				#if UNITY_EDITOR
				if (value != null && value is Object)
	            {
	                var objectVal = value as Object;
	                if (value != Content)
	                {
	                    if (objectVal is IAssetSerializable)
	                    {
	                        (objectVal as IAssetSerializable).SerializeInside(this);
	                        UnityEditor.AssetDatabase.SaveAssets();
	                    }
						else if (!UnityEditor.AssetDatabase.IsMainAsset(objectVal) && !UnityEditor.AssetDatabase.IsSubAsset(objectVal))
	                    {
							UnityEditor.AssetDatabase.AddObjectToAsset(objectVal, this);
							UnityEditor.AssetDatabase.SaveAssets();
	                    }
	                }
	            }
	                

				if (isUnityObject && value != Content && Content != null && UnityEditor.AssetDatabase.IsSubAsset(objectContent))
	            {
	                ScriptableObject.DestroyImmediate(objectContent, true);
					UnityEditor.AssetDatabase.SaveAssets();
				}
				#endif

	            base.Content = value;
	        }
	    }

	    protected virtual void OnDestroy()
	    {
			if (Application.isEditor && !Application.isPlaying && isUnityObject)
	            ScriptableObject.DestroyImmediate(objectContent, true);
	    }
	}
}