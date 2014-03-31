using UnityEngine;
using System.Collections;

public abstract class Item : ScriptableObject {

	protected Inventory container;
	public Inventory Container { get {return container;} set{this.container = value;} }
	public abstract string Name{ get; set; }
	public abstract string Description{ get; set; }
	public abstract IsoDecoration Representation{ get; set; }
	public abstract Texture2D Image{ get; set; }
	public abstract void use();
	public abstract bool isEqualThan(Item other);

}
