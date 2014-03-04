using UnityEngine;
using System.Collections;

public class Player : EntityScript {
	private int giro = 0;
	public override void eventHappened (GameEvent ge)
	{


	}

	public void onControllerEvent(ControllerEventArgs args){
		if(args.isKeyboardEvent){				
			int to = -1;
			if(args.LEFT) to = 0;
			else if(args.UP) to = 1;
			else if(args.RIGHT) to = 2;
			else if(args.DOWN) to = 3;

			if(to > -1){
				Cell destino = entity.Position.Map.getNeightbours(entity.Position)[to];
				if(entity.canMoveTo(destino) && !entity.IsMoving){
					entity.moveTo(destino);
				}
			}
		}else if(!args.isEntityEvent){


		}
	}

	private bool registered = false;

	public override void tick(){
		if(!registered)
			ControllerManager.onControllerEvent+=this.onControllerEvent;

	}
	
	public override void update(){
		
	}

	
	/*private class CommandMove : Command {

		private Entity e;
		private Cell d;
		public CommandMove(Entity e, Cell destiny){
			this.e = e;
			this.d = destiny;
		}

		public void run(){
		}
	}*/
}
