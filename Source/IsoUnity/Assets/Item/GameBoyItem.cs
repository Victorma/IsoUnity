using UnityEngine;
using System.Collections;

public class GameBoyItem : Item {

	public string description;
	public Texture2D image;
	public IsoDecoration decoration;

	public override string Name{get{return name;} set{name = value;} }
	public override string Description{get{return description;} set{description = value;} }
	public override IsoDecoration Representation{get{return decoration;} set{decoration = value;} }
	public override Texture2D Image{get{return image;} set{image = value;} }
	public override void use(){}
	public override bool isEqualThan(Item other){
		return other is GameBoyItem;
	}
}
