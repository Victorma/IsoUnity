using UnityEngine;
using System.Collections;

public class Player : EntityScript {
	
	public override void eventHappened (GameEvent ge)
	{
		if(movement!=null)
			if(ge.Name.ToLower() == "event finished")
				if(ge.getParameter("event") == movement){
					movement = null;
				}
	}

	private GameEvent movement;
	private GameEvent toLaunch;

	public void onControllerEvent(ControllerEventArgs args){
		if(toLaunch == null){
			if( args.options != null){
				if(args.options.Length==1){
					if(args.options[0].Action!=null)
						args.options[0].Action.setParameter("Executer", this.Entity);

					if(args.options[0].HasToMove){
						GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
						ge.setParameter("entity", this.Entity);
						ge.setParameter("cell", args.cell);
						ge.setParameter("synchronous", true);
						ge.setParameter("distance", args.options[0].Distance);
						ge.Name = "move";
						movement = ge;
						Game.main.enqueueEvent(ge);	
					}

					toLaunch = args.options[0].Action;
				}else if(args.options.Length > 1){
					OptionsGUI gui = ScriptableObject.CreateInstance<OptionsGUI>();
					gui.init(args, Camera.main.WorldToScreenPoint(args.entity.transform.position), args.options);
					GUIManager.addGUI(gui, 100);
				}

			}else if(args.cell != null){
				GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
				ge.setParameter("entity", this.Entity);
				ge.setParameter("cell", args.cell);
				ge.Name = "move";
				Game.main.enqueueEvent(ge);			
			}else {				
				int to = -1;
				if(args.LEFT){ to = 0; }
				else if(args.UP){ to = 1;}
				else if(args.RIGHT){ to = 2;}
				else if(args.DOWN){ to = 3; }

				if(to > -1){
					if(Entity == null)
						Debug.Log ("Null!");
					Cell destino = Entity.Position.Map.getNeightbours(Entity.Position)[to];
					GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
					ge.setParameter("entity", this.Entity);
					ge.setParameter("cell", destino);
					ge.Name = "move";
					Game.main.enqueueEvent(ge);
				}
			} 
		}
	}

	private bool registered = false;

	public override void tick(){

		if(toLaunch != null){
			if(movement == null){
				Game.main.enqueueEvent(toLaunch);
				toLaunch = null;
			}
		}

		if(!registered){
			ControllerManager.onControllerEvent+=this.onControllerEvent;
			registered = true;
		}

	}

	public override Option[] getOptions ()
	{
		GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
		ge.Name = "Hola!";
		Option o = new Option("Opcion 1",ge,false,0);

		GameEvent ge2 = ScriptableObject.CreateInstance<GameEvent>();
		ge.Name = "Hola!";
		Option o2 = new Option("Opcion 2",ge,false,0);

		Option o3 = new Option("Opcion 3",ge,false,0);

		Option o4 = new Option("Opcion 4",ge,false,0);

		Option o5 = new Option("Opcion 5",ge,false,0);

		Option o6 = new Option("Opcion 6",ge,false,0);

		Option o7 = new Option("Opcion 7",ge,false,0);

		return new Option[]{o,o2,o3,o4,o5,o6,o7};
	}

	public override void Update(){
		
	}
}
