using UnityEngine;
using System.Collections;

[System.Serializable]
public class ISwitch : ScriptableObject{

	void Awake(){
		if(state == null){
			state = ScriptableObject.CreateInstance<IsoUnityBasicType>();
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
}
