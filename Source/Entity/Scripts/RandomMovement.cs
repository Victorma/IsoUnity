using UnityEngine;
using System.Collections;

public class RandomMovement : EntityScript {

	public float probability;

	public override Option[] getOptions ()
	{
		return new Option[]{};
	}
	
	public override void tick ()
	{
		float t = probability * Time.deltaTime;
		if(Random.Range(0f,1f) < t){

			Cell c = Entity.Position.Map.getNeightbours(Entity.Position)[Random.Range(0,4)];
			if(c != null){
				GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
				ge.Name = "move";
				ge.setParameter("entity", this.Entity);
				ge.setParameter("cell", c);
				Game.main.enqueueEvent(ge);
			}
		}
	}
	
	public override void eventHappened (GameEvent ge)
	{
	}
	
	public override void Update ()
	{
	}


}
