using UnityEngine;
using System.Collections;

public class Mover : EntityScript {

	public IsoDecoration normalSprite;
	public IsoDecoration jumpingSprite;

	private Cell moveToCell;
	private Cell teleportToCell;
	private bool move = false;
	private bool teleport = false;
	private bool movementFinished = false;
	private int distanceToMove = 0;
	private GameEvent bcEvent;
	private GameEvent movementEvent;
	public override void eventHappened (GameEvent ge)
	{
		switch(ge.Name.ToLower()){
		case "move": {
			if(ge.getParameter("entity") == this.Entity || ge.getParameter("entity") == this.gameObject){
				this.moveToCell = (Cell) ge.getParameter("cell");
				this.move  = moveToCell!=null;

				distanceToMove = (ge.getParameter("distance")!=null)? (int) ge.getParameter("distance"): 0;

				this.bcEvent = ge;
			} 
			}break;

		case "teleport": {
			if(ge.getParameter("entity") == this.Entity || ge.getParameter("entity") == this.gameObject){
				teleport = true;
				teleportToCell = ge.getParameter("Cell") as Cell;
			}
		}break;
		}
	}

	public override void tick(){
		if(movementFinished){
			Game.main.eventFinished(movementEvent);
			movementFinished = false;
			movementEvent = null;
		}

		if(teleport){
			if(Entity.Position.Map == teleportToCell.Map){
				Entity.Position = teleportToCell;
			}else{
				//TODO Dont like the register calls made here...
				Entity.Position.Map.unRegisterEntity(Entity);
				Entity.Position = teleportToCell;
				Entity.Position.Map.registerEntity(Entity);

				this.GetComponent<Renderer>().enabled = false;
				foreach(Renderer r in this.GetComponentsInChildren<Renderer>())r.enabled = false;
				//TODO maybe this should be checked in player script
				if(this.GetComponent<Player>()!=null)
					MapManager.getInstance().setActiveMap(teleportToCell.Map);


			}
			teleport = false;
			move = false;
		}else if(move){
			if(!IsMoving){
				movementEvent = bcEvent;
				moveTo(moveToCell);
			}
			this.move = false;
		}


		
	}
	
	public override Option[] getOptions ()
	{
		return new Option[]{};
	}

	private bool isMoving;
	public bool IsMoving{
		get{
			return isMoving;
		}
	}
	private Cell next;
	private Movement movement;
	private float movementProgress;
	private float movementDuration;
	private int tile;
	private bool paso=false;
	private Decoration dec;
	public void moveTo(Cell c){
		RoutePlanifier.planifyRoute(this.Entity,c,distanceToMove);
	}
	public override void Update () {
		this.dec = Entity.decoration;
		if(!isMoving){
			next = RoutePlanifier.next(this.Entity);
			if(next != null){
				
				Vector3 myPosition = this.Entity.Position.transform.localPosition,
				otherPosition = next.transform.localPosition;
				
				MovementType type = MovementType.Lineal;
				if(Entity.Position.WalkingHeight != next.WalkingHeight){
					type = MovementType.Parabolic;
					dec.IsoDec = jumpingSprite;
				}
				dec.updateTextures();
				int row = 0;
				if(myPosition.z < otherPosition.z){ row = 0;}
				else if(myPosition.z > otherPosition.z){ row = 2;}
				else if(myPosition.x < otherPosition.x){  row = 1;}
				else if(myPosition.x > otherPosition.x){  row = 3;}
				dec.Tile = tile = row*dec.IsoDec.nCols;
				
				this.movement = Movement.createMovement(type, transform.position, next.transform.position + new Vector3(0,next.WalkingHeight+transform.localScale.y / 2,0));
				this.movementProgress = 0;
				this.movementDuration = 0.3f;
				isMoving = true;
			}else if(movementEvent!=null)
				movementFinished = true;
		}
		
		if(isMoving){
			this.movementProgress += Time.deltaTime;
			transform.position = this.movement.getPositionAt(this.movementProgress / this.movementDuration);
			
			
			if(dec.IsoDec.nCols>1){
				if(this.movementProgress / this.movementDuration <0.15){
					dec.Tile = tile;
				}else if (this.movementProgress / this.movementDuration < 0.85){
					dec.Tile = tile+((paso)?1:2);
				}else if (this.movementProgress / this.movementDuration < 1){
					dec.Tile = tile;
				}
			}
			
			
			if(this.movementProgress >= this.movementDuration){
				this.isMoving = false;
				this.Entity.Position = next;
				int lastRow = Mathf.FloorToInt(tile/dec.IsoDec.nCols);
				dec.IsoDec = normalSprite;
				dec.updateTextures();
				dec.Tile = lastRow*dec.IsoDec.nCols;
				paso = !paso;
				
			}
		}
	}


	/**
	 * Movements
	 * */

	public enum MovementType { Lineal, Parabolic }
	
	private abstract class Movement {
		
		protected Vector3 from,to;
		
		public static Movement createMovement(MovementType type, Vector3 from, Vector3 to){
			Movement movement = null;
			
			switch(type){
			case MovementType.Lineal: movement = new LinealMovement(); break;
			case MovementType.Parabolic: movement = new ParabolicMovement(); break;
			default: movement = new LinealMovement(); break;
			}
			
			movement.from = from;
			movement.to = to;
			
			return movement;
		}
		
		public abstract Vector3 getPositionAt(float moment);
		
		private class LinealMovement : Movement{
			public override Vector3 getPositionAt(float moment){
				if(moment >= 1)	return to;
				if(moment <= 0)	return from;
				
				return from + (to-from)*moment;
			}
		}
		
		private class ParabolicMovement : Movement{
			private Vector3 vectorHeight;
			private bool setVectorHeight = true;
			public override Vector3 getPositionAt(float moment){
				if(setVectorHeight){
					setVectorHeight = false;
					float jumpHeight = 0.25f;
					vectorHeight =  new Vector3(0,Mathf.Abs(to.y-from.y)/2f + jumpHeight, 0);
				}
				
				if(moment >= 1)	return to;
				if(moment <= 0)	return from;
				
				return from + (to-from)*moment + vectorHeight*(1f - 4f*Mathf.Pow(moment-0.5f,2));
			}
		}
	}
}
