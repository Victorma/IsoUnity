using UnityEngine;
using System.Collections;
using System;

namespace IsoUnity
{
	[System.Serializable]
	public class ISwitch : ScriptableObject {

		void Awake(){
			if(state == null){
				state = ScriptableObject.CreateInstance<IsoUnityBasicType>();

	        }
	    }

	    public void Persist()
	    {
	#if UNITY_EDITOR
	        if (Application.isEditor && !Application.isPlaying)
	        {
	            //state.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
	            UnityEditor.AssetDatabase.AddObjectToAsset(state, this);
	        }
	#endif
	    }

	    void OnDestroy()
	    {
	        if (Application.isEditor && !Application.isPlaying)
	        {
	            ScriptableObject.DestroyImmediate(state, true);
	        }
	        else
	        {
	            ScriptableObject.DestroyImmediate(state);
	        }
	    }

	    [SerializeField]
		public string id = "";

		[SerializeField]
		private IsoUnityBasicType state;
		public object State {
			get{ return state.Value;}
			set{ state.Value = value;}
		}

	    internal ISwitch Clone()
	    {
	        var r = ScriptableObject.CreateInstance<ISwitch>();
	        r.id = id;
	        r.state.Value = State;
	        return r;
	    }
	}
}
