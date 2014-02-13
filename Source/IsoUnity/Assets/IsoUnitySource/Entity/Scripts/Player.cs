using UnityEngine;
using System.Collections;

public class Player : EntityScript {
	public bool guapo;
	private int giro = 0;
	public override void eventHappened (GameEvent ge)
	{
		//Debug.Log((guapo)?"Pero que guapo que soy": "Nah soy feucho");

		if(ge != null){
			if(ge.Name.Equals("KeyPressed")){
				/*Debug.Log("Hola don pepito");

				Game g = GameObject.FindGameObjectWithTag("Game").GetComponent<Game>();

				GameEvent newGe = new GameEvent();
				newGe.Name = "Talked";
				newGe.Args = new object[1]{this};
				g.enqueueEvent(newGe);*/

				Cell destino =null;
				do{

					destino = entity.Position.Map.getNeightbours(entity.Position)[(1+giro)%4];
					if(destino == null)
						giro +=1;
				}while(destino == null && giro < 4);
				giro = giro%4;

				Game.main.enqueueCommand(new CommandMove(entity, destino));

			}
		}
	}

	private class CommandMove : Command {

		private Entity e;
		private Cell d;
		public CommandMove(Entity e, Cell destiny){
			this.e = e;
			this.d = destiny;
		}

		public void run(){


			e.Position = d;
			GameEvent ge = new GameEvent();
			ge.Name = "Player moved";
			ge.Args = new object[1]{e};
			Game.main.enqueueEvent(ge);
		}
	}
}
