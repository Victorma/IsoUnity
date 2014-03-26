using UnityEngine;
using System.Collections;

public abstract class Item  {

	protected Inventory container;
	public Inventory Container { get {return container;} set{this.container = value;} }
	public abstract string Name{ get; set; }
	public abstract IsoDecoration representation{ get; set; }
	public abstract Texture2D image{ get; set; }
	public abstract void use();

}
