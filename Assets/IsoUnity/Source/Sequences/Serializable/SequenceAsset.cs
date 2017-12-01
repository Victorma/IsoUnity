using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace IsoUnity.Sequences {
	public class SequenceAsset : Sequence {

	    [SerializeField]
	    private bool assetinited = false;

	    /*void OnEnable()
	    {
	        if (!assetinited)
	        {
	            InitAsset();
	        }
	    }*/

	    public void InitAsset()
	    {
	        if(this.localVariables == null){
	            this.localVariables = ScriptableObject.CreateInstance<IsoSwitches>();
	        }

			#if UNITY_EDITOR
	        if (!UnityEditor.AssetDatabase.IsSubAsset(this.localVariables))
	        {
				UnityEditor.AssetDatabase.AddObjectToAsset(this.localVariables, this);
				UnityEditor.AssetDatabase.SaveAssets();
	        }
			#endif
	        assetinited = true;
	    }

	    public override SequenceNode CreateNode(string id, object content = null, int childSlots = 0)
		{
			#if UNITY_EDITOR
			var node = CreateInstance<SequenceNodeAsset>();
			UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
			#else
			var node = CreateInstance<SequenceNode>();
			#endif

	        node.init(this);
	        this.nodeDict.Add(id, node);
	        node.Content = content;

			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.SaveAssets();
			#endif

	        return node;
	    }

	    public override SequenceNode CreateNode(object content = null, int childSlots = 0)
		{
			#if UNITY_EDITOR
			var node = CreateInstance<SequenceNodeAsset>();
			UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
			#else
			var node = CreateInstance<SequenceNode>();
			#endif

	        node.init(this);
	        this.nodeDict.Add(node.GetInstanceID().ToString(), node);
	        node.Content = content;

			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.SaveAssets();
			#endif

	        return node;
	    }
	    
	    public override bool RemoveNode(SequenceNode node)
	    {
			var r = base.RemoveNode(node);

			#if UNITY_EDITOR
			if (r)
				UnityEditor.AssetDatabase.SaveAssets();
			#endif

	        return r;
	    }

		#if UNITY_EDITOR
	    public static Sequence FindSequenceOf(Object content)
	    {
	        Sequence r = null;
			var sequences = UnityEditor.AssetDatabase.FindAssets("t:Sequence").ToList().ConvertAll(o => UnityEditor.AssetDatabase.GUIDToAssetPath(o));

	        foreach (var s in sequences)
	        {
				Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(s);
	            for (int i = 0; i < assets.Length; i++)
	            {
	                Object asset = assets[i];
	                if (asset == content)
	                {
						return UnityEditor.AssetDatabase.LoadAssetAtPath(s, typeof(Sequence)) as Sequence;
	                }
	            }
	        }

	        return r;
		}
		#endif
	}
}