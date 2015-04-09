using UnityEngine;
using System.Collections;

public abstract class EntityScript : MonoBehaviour {
	public Entity Entity {
		get { return this.GetComponent<Entity> ();}
	}
	// Use this for initialization
	void Start () {
		
	}

	//Abstract Methods
	public abstract void eventHappened(GameEvent ge);
	public abstract void tick();
	public abstract void Update();
	public abstract Option[] getOptions();
}
