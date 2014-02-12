using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	private bool canBlockMe;

	public bool canMoveTo(Cell c){
		/*canAccedTo(c);
		if(canBlockMe)
			entitiesLetMePass(c);

		canGoThroughEntities(c);*/
		return false;
	}

	private bool entitiesLetMePass(Cell c){
		//foreach(Entity e in c.getEntities())
		return false;
	}

	public bool canGoThrough(Entity e){

		return false;
	}

	public void eventHappened(GameEvent ge){
		EntityScript[] scripts = this.GetComponents<EntityScript>();

		//TODO Preference system

		foreach(EntityScript es in scripts)
			es.eventHappened(ge);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
