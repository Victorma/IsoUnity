using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Entity : MonoBehaviour {

	private bool canBlockMe;
	private bool blackWhite;
	private List<EntityScript> list;

	[SerializeField]
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
		//canAccedTo(c);
		if(canBlockMe)
			entitiesLetMePass(c);

		//canGoThroughEntities(c);
		return false;
	}

	private bool entitiesLetMePass(Cell c){
		//foreach(Entity e in c.getEntities())


		return false;
	}

	private bool letPass(Entity e){
		/*foreach(Entity en in list){
			en.
		}*/
		return false;
	}

	public bool canGoThrough(Entity e){

		return false;
	}

	public void tick(){
		foreach(EntityScript es in this.GetComponents<EntityScript>())
			es.tick();
	}

	public void eventHappened(GameEvent ge){
		EntityScript[] scripts = this.GetComponents<EntityScript>();

		//TODO Preference system

		foreach(EntityScript es in scripts)
			es.eventHappened(ge);
	}

	/**
	 * Movement things
	 * */

	public enum MovementType { Lineal, Parabolic }

	private abstract class Movement {

		protected Vector3 from,to;

		public static Movement createMovement(MovementType type, Vector3 from, Vector3 to){
			Movement movement = null;

			switch(type){
				case MovementType.Lineal: movement = new LinealMovement(); break;
				case MovementType.Parabolic: movement = new ParabolicMovement(); break;
				default: movement = new LinealMovement(); break;
			}

			movement.from = from;
			movement.to = to;

			return movement;
		}

		public abstract Vector3 getPositionAt(float moment);

		private class LinealMovement : Movement{
			public override Vector3 getPositionAt(float moment){
				if(moment >= 1)	return to;
				if(moment <= 0)	return from;

				return from + (to-from)*moment;
			}
		}

		private class ParabolicMovement : Movement{
			private Vector3 vectorHeight;
			private bool setVectorHeight = true;
			public override Vector3 getPositionAt(float moment){
				if(setVectorHeight){
					setVectorHeight = false;
					float jumpHeight = 0.25f;
					vectorHeight =  new Vector3(0,Mathf.Abs(to.y-from.y)/2f + jumpHeight, 0);
				}

				if(moment >= 1)	return to;
				if(moment <= 0)	return from;

				return from + (to-from)*moment + vectorHeight*(1f - 4f*Mathf.Pow(moment-0.5f,2));
			}
		}
	}

	private bool isMoving;
	public bool IsMoving{
		get{
			return isMoving;
		}
	}
	private Cell destiny;
	private Movement movement;
	private float movementProgress;
	private float movementDuration;

	public void moveTo(Cell c, MovementType movementType){
		if(!isMoving){
			Debug.Log("Ok me muevo!");
			if(my_transform == null)
				my_transform = this.transform;
			destiny = c;
			this.movement = Movement.createMovement(movementType, my_transform.position, c.transform.position + new Vector3(0,c.WalkingHeight+my_transform.localScale.y / 2,0));
			this.movementProgress = 0;
			this.movementDuration = 0.3f;
			this.isMoving = true;
		}
	}

	/**
	 * End Movement things
	 * */

	// Use this for initialization
	void Start () {
		
	}
	Transform my_transform;

	// Update is called once per frame
	void Update () {
		if(!Application.isPlaying && Application.isEditor){
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

			if(this.position != null){
				my_transform.position = position.transform.position + new Vector3(0, position.WalkingHeight + my_transform.localScale.y/2f, 0);
			}
		}
		if(isMoving){
			Debug.Log(movement);
			if(my_transform ==null)
				my_transform = this.transform;
			this.movementProgress += Time.deltaTime;
			my_transform.position = this.movement.getPositionAt(this.movementProgress / this.movementDuration);
			if(this.movementProgress >= this.movementDuration){
				this.isMoving = false;
				this.Position = destiny;
			}
		}
	}
}
