using UnityEngine;
using System.Collections.Generic;
using System;

namespace IsoUnity.Sequences {
	public abstract class ForkGroup : Checkable {

	    public static T Create<T>(params Checkable[] forks) where T : ForkGroup
	    {
	        return Create<T>(new List<Checkable>(forks));
	    }

	    public static T Create<T>(List<Checkable> forks) where T : ForkGroup
	    {
	        var r = ScriptableObject.CreateInstance<T>();
	        r.forks = forks;
	        return r;
	    }

	    [SerializeField]
	    protected List<Checkable> forks = new List<Checkable>();

	    public List<Checkable> List { get { return forks; } }

	    public void AddFork(Checkable condition)
	    {
	        this.forks.Add(condition);

#if UNITY_EDITOR
	        if(Application.isEditor && !Application.isPlaying)
	        {
	            // If this is an asset and the condition isnt
	            if((UnityEditor.AssetDatabase.IsMainAsset(this) 
	                || UnityEditor.AssetDatabase.IsSubAsset(this)) 
	                && !UnityEditor.AssetDatabase.IsMainAsset(condition) 
	                && !UnityEditor.AssetDatabase.IsSubAsset(condition))
	            {
	                if(condition is IAssetSerializable)
	                {
	                    (condition as IAssetSerializable).SerializeInside(this);
	                }
	                else
	                {
	                    // Capture it inside me
	                    UnityEditor.AssetDatabase.AddObjectToAsset(condition, this);
	                    UnityEditor.AssetDatabase.SaveAssets();
	                }
	            }
	        }
#endif
	    }

	    public void RemoveFork(Checkable fork)
	    {
	        this.forks.Remove(fork);
#if UNITY_EDITOR
	        if (Application.isEditor && !Application.isPlaying)
	        {
	            // If this is an asset and the condition isnt
	            if (UnityEditor.AssetDatabase.IsSubAsset(fork))
	            {
	                // Capture it inside me
	                ScriptableObject.DestroyImmediate(fork, true);
	                UnityEditor.AssetDatabase.SaveAssets();
	            }
	        }
#endif
	    }

	    void OnDestroy()
	    {
	        new List<Checkable>(List).ForEach(f => RemoveFork(f));
	    }
	}
}