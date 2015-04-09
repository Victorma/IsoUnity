using UnityEngine;
using System.Collections;

public class ItemScript : EntityScript {

	public Item item;

	private bool picked = false;
	private bool destroy = false;
	private Inventory by;
	private GameEvent addItemEvent;

	public override void eventHappened (GameEvent ge)
	{
		switch(ge.Name.ToLower()){
		case "pick":
			if(ge.getParameter("Executer") != null && ge.getParameter ("Item") == this){
				Entity executor = ge.getParameter("Executer") as Entity;
				by = executor.GetComponent<Inventory>();
				picked = by != null;
			}
			
			break;
		case "event finished":
			if(ge.getParameter("event") == addItemEvent)
				destroy = true;
			break;
		}

	}

	public override Option[] getOptions ()
	{
		GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
		ge.Name = "Pick";
		ge.setParameter ("Item", this);

		return new Option[]{new Option ("Pick", ge, 1)};

	}

	public override void tick ()
	{
		if (picked) {
			GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
			ge.name = "add item";
			ge.setParameter("item", this.item);
			ge.setParameter("inventory", by);
			ge.setParameter("synchronous", true);
			Game.main.enqueueEvent(ge);
			addItemEvent = ge;
			picked = false;
		}
	}

	public override void Update ()
	{
		if(destroy){
			GameObject.DestroyImmediate(this.gameObject);
		}
	}
}
