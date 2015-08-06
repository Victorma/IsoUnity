using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Entity : MonoBehaviour {

	public bool canBlockMe = true;
	public bool isBlackList = true;
	public float maxJumpSize = 1.5f;
	public IsoDecoration normalSprite;
	public IsoDecoration jumpingSprite;
	public Texture2D face;
	public List<EntityScript> list;

	[SerializeField]
	private Cell position;
	public Cell Position {
		get{
			return position;
		}
		set {
			position = value;
			this.transform.parent = position.transform;
			my_transform.position = position.transform.position + new Vector3(0, position.WalkingHeight + my_transform.localScale.y/2f, 0);
		}
	}

    public bool canMoveTo(Cell from, Cell to)
    {
        //canAccedTo(c);

        bool canMove = false;

        if (to != null && Mathf.Abs(from.WalkingHeight - to.WalkingHeight) <= maxJumpSize)
        {
            if (canBlockMe)
                canMove = to.isAccesibleBy(this);
            else
                canMove = true;
        }

        //canGoThroughEntities(c);
        return canMove;
    }

	public bool canMoveTo(Cell c){
		return canMoveTo(position,c);
	}

	public bool letPass(Entity e){
		/*foreach(EntityScript en in list){
			foreach(EntityScript hisEn in e.GetComponents<EntityScript>()){
				if(hisEn == en)
					return !isBlackList;
			}
		}*/
		return isBlackList;
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

	public Option[] getOptions(){
		EntityScript[] scripts = this.GetComponents<EntityScript>();
		List<Option> options = new List<Option>();
		
		foreach(EntityScript es in scripts)
			options.AddRange (es.getOptions());

		return options.ToArray() as Option[];
	}

	// Use this for initialization
	void Start () {
		if(Application.isPlaying){
			Mover mover = this.gameObject.AddComponent<Mover>();
			mover.normalSprite = normalSprite;
			mover.jumpingSprite = jumpingSprite;
		}
	}


	Transform my_transform;
	public Decoration decoration {
		get{
			return this.GetComponent<Decoration>();
		}
	}

	public Mover mover {
		get{
			return this.GetComponent<Mover>();
		}
	}

	// Update is called once per frame
	void Update () {
		if(my_transform ==null)
			my_transform = this.transform;

		if(!Application.isPlaying && Application.isEditor){

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


	}

	public void onDestroy(){

	}
}
