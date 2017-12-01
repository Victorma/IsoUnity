using System;
using UnityEngine;
using System.Collections.Generic;

namespace IsoUnity
{
	[System.Serializable]
	public class IsoSwitches : ScriptableObject
	{
		[SerializeField]
		public List<ISwitch> switches = new List<ISwitch>();
	    
		public IsoSwitches (){
		}

		public ISwitch addSwitch(){
			ISwitch iss = ScriptableObject.CreateInstance<ISwitch>();
	        this.switches.Add(iss);
	#if UNITY_EDITOR
	        if (Application.isEditor && !Application.isPlaying 
                && (UnityEditor.AssetDatabase.IsMainAsset(this) || UnityEditor.AssetDatabase.IsSubAsset(this)))
	        {
	            UnityEditor.AssetDatabase.AddObjectToAsset(iss, this);
	            iss.Persist();
	            UnityEditor.AssetDatabase.SaveAssets();
	        }
	#endif
			return iss;
		}

		public void removeSwitch(ISwitch swt){
			if(this.switches.Contains(swt))
			   this.switches.Remove (swt);

	#if UNITY_EDITOR
	        if (Application.isEditor && !Application.isPlaying
                && (UnityEditor.AssetDatabase.IsMainAsset(this) || UnityEditor.AssetDatabase.IsSubAsset(this)))
            {
	            ScriptableObject.DestroyImmediate(swt, true);
	            UnityEditor.AssetDatabase.SaveAssets();
	        }
	#endif
	        //ScriptableObject.Destroy (swt);
	    }

	    private ISwitch findSwitch(string id)
	    {
	        ISwitch r = null;
	        foreach (ISwitch isw in this.switches)
	        {
	            if (!string.IsNullOrEmpty(isw.id) && isw.id.Equals(id))
	            {
	                r = isw;
	                break;
	            }
	        }
	        return r;
	    }

	    public bool containsSwitch(string id)
	    {
	        return findSwitch(id) != null;
	    }

	    public ISwitch getSwitch(string id){

	        var r = findSwitch(id);
			if(r == null){
				r = addSwitch();
				r.id = id;
			}
			return r;
		}

		public ISwitch[] getList(){
			return this.switches.ToArray() as ISwitch[];
		}

		public object consultSwitch(string id){
			return getSwitch (id).State;
		}

	    public IsoSwitches Clone()
	    {
	        var r = ScriptableObject.CreateInstance<IsoSwitches>();
	        r.switches = switches.ConvertAll(s => s.Clone());
	        return r;
	    }
	}
}