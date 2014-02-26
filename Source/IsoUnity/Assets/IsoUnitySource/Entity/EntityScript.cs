using UnityEngine;
using System.Collections;

public abstract class EntityScript : MonoBehaviour {
	protected Entity entity;
	// Use this for initialization
	void Start () {
		this.entity = GetComponent<Entity>();
	}

	//Abstract Methods
	public abstract void eventHappened(GameEvent ge);
	public abstract void tick();
	public abstract void update();
}
