using UnityEngine;
using System.Collections;

public abstract class EntityScript : MonoBehaviour {
	protected Entity entity;
	// Use this for initialization
	void Start () {
		this.entity = GetComponent<Entity>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Abstract Methods
	public abstract void eventHappened(GameEvent ge);
}
