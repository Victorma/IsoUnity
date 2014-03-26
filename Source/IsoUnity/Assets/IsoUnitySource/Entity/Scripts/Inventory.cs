using UnityEngine;
using System.Collections.Generic;

public class Inventory : EntityScript {

	private List<Item> itemsToAdd = new List<Item>();
	private List<Item> itemsToRemove = new List<Item>();
	private bool openInventory = false;
	private List<Item> itemsToUse = new List<Item>();

	public override void eventHappened (GameEvent ge)
	{
		if(ge.getParameter("Inventory") == this)
		{
			Item item = (Item)ge.getParameter("Item");
			switch (ge.Name.ToLower()) 
			{
			case "open inventory": 
				openInventory = true;
				break;
			case "add item": 
				itemsToAdd.Add (item);
				break;
			case "remove item": 
				itemsToRemove.Add(item);
				break;
			case "use item": 
				itemsToUse.Add(item);
				break;
			}
		}
	}

	public override Option[] getOptions ()
	{
		if (this.entity.GetComponent<Player> () != null) {
			GameEvent ge = new GameEvent ();
			ge.Name = "Open Inventory";
			ge.setParameter ("Inventory", this);
			Option option = new Option ("Inventory", ge, false); 
			return new Option[]{option};
		} else
			return null;
	}

	public override void tick ()
	{
		if (openInventory) {
			Debug.Log ("Abriendo inventario");
			GUIManager.addGUI (new InventoryGUI (this));
			openInventory = false;
		}	
		//ADDS
		while (itemsToAdd.Count > 0) { 
			if (!items.Contains (itemsToAdd [0])) items.Add (itemsToAdd [0]);
			itemsToAdd.RemoveAt (0);
		}
		//USES
		while (itemsToUse.Count > 0) { 
			if (items.Contains (itemsToUse [0])) itemsToUse[0].use();
			itemsToUse.RemoveAt (0);
		}
		//REMOVES
		while (itemsToRemove.Count > 0) { 
			if (!items.Contains (itemsToAdd [0])) items.Remove (itemsToAdd [0]);
			itemsToUse.RemoveAt (0);
		}
	}

	public override void Update (){}

	private List<Item> items = new List<Item> ();
	public Item[] Items{
		get{return items.ToArray() as Item[];}
	}

}
