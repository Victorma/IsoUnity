using UnityEngine;
using System.Collections;

public class Teleporter : EntityScript {

	public bool enabled = false;
	public Cell destination;
	public int mode = 0;
	public SerializableGameEvent sge;
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
				GameEvent ge = new GameEvent();
				ge.Name = "Teleport";
				ge.setParameter("Entity", e);
				ge.setParameter("Cell", destination);
				Game.main.enqueueEvent(ge);
			}
		}
	}

	public override void eventHappened (IGameEvent ge)
	{
		if(mode == 1 && this.sge == ge)
			enabled = true;
	}

	public override void Update ()
	{
		if(start){
			enabled = mode == 0;
		}
	}
}
