using UnityEngine;
using System.Collections;

public abstract class EntityScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Abstract Methods
	public abstract void eventHappened(GameEvent ge);
}
