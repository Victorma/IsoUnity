using UnityEngine;
using System.Collections;
using System;

namespace IsoUnity.Sequences {
	[NodeContent("Fork/Single/Switch Fork", 2)]
	public class ISwitchFork : Checkable, IAssetSerializable
	{
	    public static ISwitchFork Create(string id, ComparationType comparation, object value)
	    {
	        var isf = ScriptableObject.CreateInstance<ISwitchFork>();
	        isf.comparationType = comparation;
	        isf.id = id;
	        isf.Value = value;

	        return isf;
	    }

		void Awake(){
			if(value == null)
	        {
	            value = ScriptableObject.CreateInstance<IsoUnityBasicType>();
	        }
		}

	    void OnDestroy()
	    {
	#if UNITY_EDITOR
	        if (value != null)
	        {
	            if (Application.isEditor && !Application.isPlaying)
	            {
	                ScriptableObject.DestroyImmediate(value, true);
	            }
	        }
	#endif
	    }

	    public enum ComparationType {Equal, Greather, Less, Distinct, GreatherEqual, LessEqual};
		[SerializeField]
		public ComparationType comparationType = ComparationType.Equal;
		[SerializeField]
		public string id = "";
		[SerializeField]
		private IsoUnityBasicType value;
		public object Value {
			get { return value.Value; }
			set { this.value.Value = value; } 
		}

		public override bool check(){

			System.IComparable chk = (System.IComparable) IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch(id);
			if(chk == null)
				return false;

			bool c = false;
			switch(comparationType){
				case ComparationType.Equal:			c = chk.CompareTo(value.Value) == 0; break;
				case ComparationType.Greather: 		c = chk.CompareTo(value.Value)  > 0; break;
				case ComparationType.Less: 			c = chk.CompareTo(value.Value)  < 0; break;
				case ComparationType.Distinct: 		c = chk.CompareTo(value.Value) != 0; break;
				case ComparationType.GreatherEqual: c = chk.CompareTo(value.Value) >= 0; break;
				case ComparationType.LessEqual: 	c = chk.CompareTo(value.Value) <= 0; break;
			}

			return c;
		}

	    public void SerializeInside(UnityEngine.Object assetObject)
	    {
	#if UNITY_EDITOR
	        if (Application.isEditor && !Application.isPlaying)
	        {
	            if (!UnityEditor.AssetDatabase.IsSubAsset(this))
	            {
	                UnityEditor.AssetDatabase.AddObjectToAsset(this, assetObject);
	                value.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
	                UnityEditor.AssetDatabase.AddObjectToAsset(value, this);
	                UnityEditor.AssetDatabase.SaveAssets();
	            }
	        }
	#endif
	    }

	    public override string ToString()
	    {
	        return this.name;
	    }
	}
}