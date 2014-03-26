using UnityEngine;
using System.Collections;

public class Player : EntityScript {
	
	public override void eventHappened (GameEvent ge)
	{

	}

	public void onControllerEvent(ControllerEventArgs args){
		if( args.options != null){
			if(args.options.Length==1){
				args.options[0].Action.setParameter("Executer", this.entity);
				if(!args.options[0].HasToMove)
					Game.main.enqueueEvent(args.options[0].Action);
			}else if(args.options.Length > 1){
				OptionsGUI gui = new OptionsGUI(Camera.main.WorldToScreenPoint(entity.transform.position), entity.getOptions());
				GUIManager.addGUI(gui, 100);
			}

		}else if(args.cell != null){
			GameEvent ge = new GameEvent();
			ge.setParameter("entity", this.entity);
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
				Cell destino = entity.Position.Map.getNeightbours(entity.Position)[to];
				GameEvent ge = new GameEvent();
				ge.setParameter("entity", this.entity);
				ge.setParameter("cell", destino);
				ge.Name = "move";
				Game.main.enqueueEvent(ge);
			}
		} 
	}

	private bool registered = false;

	public override void tick(){

		if(!registered){
			ControllerManager.onControllerEvent+=this.onControllerEvent;
			registered = true;
		}

	}

	public override Option[] getOptions ()
	{
		GameEvent ge = new GameEvent();
		ge.Name = "Hola!";
		Option o = new Option("Hola mundo!",ge);

		return new Option[]{o,o,o,o,o,o,o,o,o};
	}

	public override void Update(){
		
	}
}
