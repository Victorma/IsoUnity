using UnityEngine;
using System.Collections;

public class Player : EntityScript {
	public float maxJumpSize;
	private int giro = 0;
	public override void eventHappened (GameEvent ge)
	{
		//Debug.Log((guapo)?"Pero que guapo que soy": "Nah soy feucho");

		if(ge != null){
			if(ge.Name.Equals("Controller")){
				/*Debug.Log("Hola don pepito");

				Game g = GameObject.FindGameObjectWithTag("Game").GetComponent<Game>();

				GameEvent newGe = new GameEvent();
				newGe.Name = "Talked";
				newGe.Args = new object[1]{this};
				g.enqueueEvent(newGe);*/

				int to = 0;
				switch(((string)ge.Args[0]).ToLower()){
					case "up": to = 1; break;
					case "down": to = 3; break;
					case "left": to = 0; break;
					case "right": to = 2; break;
				}

				Cell destino = entity.Position.Map.getNeightbours(entity.Position)[to];
				if(entity.canMoveTo(destino))
					if(destino != null && Mathf.Abs(entity.Position.WalkingHeight - destino.WalkingHeight) <=maxJumpSize)
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
			Entity.MovementType type = Entity.MovementType.Lineal;
			if(e.Position.WalkingHeight != d.WalkingHeight)
				type = Entity.MovementType.Parabolic;


			e.moveTo(d,type);
			/*GameEvent ge = new GameEvent();
			ge.Name = "Player moved";
			ge.Args = new object[1]{e};
			Game.main.enqueueEvent(ge);*/
		}
	}

	public override void tick(){
		
	}
	
	public override void update(){
		
	}
}
