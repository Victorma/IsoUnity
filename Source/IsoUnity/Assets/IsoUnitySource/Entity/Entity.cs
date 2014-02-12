using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Entity : MonoBehaviour {

	private bool canBlockMe;
	private Cell position;
	public Cell Position {
		get{
			return position;
		}
		set {
			position = value;
			this.transform.parent = position.transform;
		}
	}

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

	public void tick(){
		this.eventHappened(null);
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
	Transform my_transform;
	// Update is called once per frame
	void Update () {
		if(my_transform ==null)
			my_transform = this.transform;


		Transform parent = my_transform.parent;
		Transform actual = null;
		if(position != null)
			actual = position.transform;

		if(parent != actual){
			Cell probablyParent = parent.GetComponent<Cell>();
			if(probablyParent!=null)
				position = probablyParent;
			else if(actual!=null)
				my_transform.parent = actual;

		}

		if(this.position != null)
			my_transform.position = position.transform.position + new Vector3(0, position.Height + my_transform.localScale.y/2f, 0);
	}
}
