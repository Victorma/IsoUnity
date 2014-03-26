using UnityEngine;
using System.Collections;

public class ItemScript : EntityScript {

	public Item item;

	private bool picked = false;
	private Inventory by;

	public override void eventHappened (GameEvent ge)
	{
		if (ge.getParameter ("Item") == this.item) {
			switch(ge.Name.ToLower()){
			case "pick":break;
			}
		}
	}

	public override Option[] getOptions ()
	{
		GameEvent ge = new GameEvent ();
		ge.Name = "Pick";
		ge.setParameter ("Item", this.item);

		return new Option[]{new Option ("Pick", ge)};

	}

	public override void tick ()
	{
		if (item == null) {
		
		}
		throw new System.NotImplementedException ();
	}

	public override void Update ()
	{
		throw new System.NotImplementedException ();
	}
}
