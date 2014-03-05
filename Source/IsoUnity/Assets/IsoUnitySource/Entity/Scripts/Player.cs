using UnityEngine;
using System.Collections;

public class Player : EntityScript {
	private int giro = 0;
	public override void eventHappened (GameEvent ge)
	{


	}

	public void onControllerEvent(ControllerEventArgs args){
		if(args.isKeyboardEvent){				
			int to = -1, tile = -1;
			if(args.LEFT){ to = 0; tile = 0;}
			else if(args.UP){ to = 1; tile = 3;}
			else if(args.RIGHT){ to = 2; tile = 6;}
			else if(args.DOWN){ to = 3; tile = 9;}

			if(to > -1){
				Cell destino = entity.Position.Map.getNeightbours(entity.Position)[to];
				if(entity.canMoveTo(destino) && !entity.IsMoving){
					Decoration dec = this.GetComponent<Decoration>();
					if(dec!=null){
						dec.Tile = tile;
					}
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
}
