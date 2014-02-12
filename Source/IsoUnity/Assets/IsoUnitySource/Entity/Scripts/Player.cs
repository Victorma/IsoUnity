using UnityEngine;
using System.Collections;

public class Player : EntityScript {
	public bool guapo;

	public override void eventHappened (GameEvent ge)
	{
		Debug.Log((guapo)?"Pero que guapo que soy": "Nah soy feucho");
	}
}
