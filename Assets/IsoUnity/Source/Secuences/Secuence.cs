using UnityEngine;
using System.Collections;

[System.Serializable]
public class Secuence : ScriptableObject {
	[SerializeField]
	private SecuenceNode root;

	public void init(){
		Debug.Log ("Root created");
		root = ScriptableObject.CreateInstance<SecuenceNode>();
		Debug.Log (root);
		root.init (); 
		DontDestroyOnLoad(this);
	}
	public SecuenceNode Root{
		get{ return root;}
		set{ root = value;}
	}

}
