using UnityEngine;
using System.Collections;

public class Teleporter : EntityScript {

	public bool enabled = false;
	public Cell destination;
	public int mode = 0;
	public GameEvent ge;
	public Checkable checkable;

	private bool start = true;

	public override Option[] getOptions ()
	{
		return new Option[]{new Option("Teleport",null,0)};
	}

	public override void tick ()
	{

		if(mode == 2)
			enabled = checkable.check();

		if(destination == null || !enabled)
			return;

		Entity[] entities = Entity.Position.getEntities();
		foreach(Entity e in entities){
			if(e!=this.Entity){
				GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
				ge.Name = "Teleport";
				ge.setParameter("Entity", e);
				ge.setParameter("Cell", destination);
				Game.main.enqueueEvent(ge);
			}
		}
	}

	public override void eventHappened (GameEvent ge)
	{
		if(mode == 1 && this.ge == ge)
			enabled = true;
	}

	public override void Update ()
	{
		if(start){
			enabled = mode == 0;
		}
	}
}
