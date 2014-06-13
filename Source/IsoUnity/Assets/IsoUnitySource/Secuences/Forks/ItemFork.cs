using UnityEngine;
using System.Collections;

public class ItemFork : Checkable {

	public Inventory inventory;
	public Item item;
	public bool contains;

	public override bool check()
	{
		bool find = false;
		foreach(Item item in inventory.Items)
			if(item.isEqualThan(this.item)){
				find = true; break;
			}
		return contains?find:!find;
	}
}
