using UnityEngine;
using System.Collections.Generic;

public class Mover : EntityScript {

    /***************************
    * Directions Enum & Utils
    **************************/
    public enum Direction
    {
        North, East, West, South
    }
    private static int getDirectionIndex(Direction d)
    {
        switch (d)
        {
            case Direction.North: return 0;
            case Direction.East: return 1;
            case Direction.South: return 2;
            case Direction.West: return 3;
        }

        return -1;
    }
    /****************
    * End Directions
    *****************/

    /****************
     * Begin Attributes
     * ****************/
	public IsoDecoration normalSprite;
	public IsoDecoration jumpingSprite;
    public Direction direction;

    // Move
	private Cell moveToCell;
    private bool move = false;
    private bool movementFinished = false;
    private int distanceToMove = 0;
    private GameEvent bcEvent;
    private GameEvent movementEvent;

    //Teleport
	private Cell teleportToCell;
	private bool teleport = false;

    //Turn
    private Direction turnDirection;
    private bool turnAfterMove = false;
    private bool turn = false;

    //Stop
    private bool stop = false;

    /****************
     * End Attributes
     * ***************/

    /*******************
     * PUBLIC OPERATIONS
     * ******************/

    public void moveTo(Cell c)
    {
        RoutePlanifier.planifyRoute(this.Entity, c, distanceToMove);
    }
    public void teleportTo(Cell c)
    {
        RoutePlanifier.cancelRoute(this.Entity);
        this.movement = Movement.createMovement(MovementType.Instant,
            this.Entity, this, dec, normalSprite, this.transform.position,
            c.transform.position, this.Entity.Position, c, null);
    }

    public void switchDirection(Direction direction)
    {
        RoutePlanifier.cancelRoute(this.Entity);
        this.movement = createTurnMovement(direction);
    }

    private Movement createTurnMovement(Direction dir)
    {
        Dictionary<string, object> mParams = new Dictionary<string, object>();
        mParams["direction"] = dir;

        return Movement.createMovement(MovementType.Turn,
            this.Entity, this, dec, normalSprite, this.transform.position,
            this.transform.position, this.Entity.Position, this.Entity.Position, mParams);
    }

    /**************
     * Event Control
     * *************/
	public override void eventHappened (GameEvent ge)
	{
        if (!ge.belongsTo(this))
            return;

		switch(ge.Name.ToLower()){
        case "turn":{
            if (!this.IsMoving) // If it's moving, just ignore the turn
            {
                object dir = ge.getParameter("direction");
                if (dir != null && dir is Direction)
                    this.turnDirection = (Direction)dir;

                this.turn = true;
            }
            }break;
		case "move": {
			    this.moveToCell = (Cell) ge.getParameter("cell");
			    this.move  = moveToCell!=null;

			    distanceToMove = (ge.getParameter("distance")!=null)? (int) ge.getParameter("distance"): 0;
                object dir = ge.getParameter("direction");
                if (dir != null && dir is Direction)
                {
                    this.turnDirection = (Direction)dir;
                    this.turnAfterMove = true;
                }

			    this.bcEvent = ge;
			}break;

		case "teleport": {
				teleport = true;
				teleportToCell = ge.getParameter("Cell") as Cell;
		    }break;

        case "stop":
            {
                stop = true;
            } break;
		}
	}

    private Direction lastDirection = Direction.West;

    private void finalizeMovementEvent()
    {
        if (movementEvent != null)
        {
            Game.main.eventFinished(movementEvent);
            movementFinished = false;
            movementEvent = null;
        }
    }

	public override void tick(){
        if (this.dec == null)
            this.dec = Entity.decoration;

		if(movementFinished)
            finalizeMovementEvent();

        if (stop)
        {
            if (IsMoving)
                finalizeMovementEvent();

            // Cancel all pending operations
            this.move = false;
            this.teleport = false;
            this.turn = false;
        }
        else if(teleport)
        {
            if (IsMoving)
                finalizeMovementEvent();

            teleportTo(teleportToCell);
			teleportToCell = null;
			teleport = false;
		}
        else if(move)
        {
            if (IsMoving)
                finalizeMovementEvent();

			movementEvent = bcEvent;
			moveTo(moveToCell);

			this.move = false;
        }
        else if (turn)
        {
            if (IsMoving)
                finalizeMovementEvent();

            switchDirection(turnDirection);
            turn = false;
        }

        // Direction change responsive update
        if (!IsMoving && movementEvent == null)
            if (lastDirection != direction)
                switchDirection(direction);

        // Direction change ignore
        lastDirection = direction;
	}
	
	public override Option[] getOptions ()
	{
		return new Option[]{};
	}

	public bool IsMoving{
		get{
			return movement != null && !movement.Ended;
		}
	}
	private Cell next;
	private Movement movement;

	private bool paso = false;
	private Decoration dec;


	public override void Update () {
        if (this.dec == null)
            this.dec = Entity.decoration;

        // Force initial direction update
        if (this.movement == null)
        {
            movement = createTurnMovement(direction);
            movement.addProgress(1f); // Move to End
            movement.Update(); // Assure finalization
            movement.UpdateTextures();
        }

        // If we're not moving right now
		if(!IsMoving){
            // Let's see if we have something to do...
			next = RoutePlanifier.next(this.Entity);
            //Then, let's move :D
			if(next != null){
				Vector3 myPosition = this.Entity.Position.transform.localPosition,
				otherPosition = next.transform.localPosition;
				
                // Define the movement type and set the tilesheet
                MovementType type = getMovementTypeTo(next);
				this.movement = Movement.createMovement(type, // type
                    this.Entity,                              // Entity
                    this,                                     // Mover
                    dec,                                      // Decoration 
                    getSpritesheetForMovementType(type),      // Sheet
                    transform.position,                       // Origin
                    next.transform.position + new Vector3(0,next.WalkingHeight+transform.localScale.y / 2,0), // Destination
                    Entity.Position,                          // Cell Origin
                    next,                                     // Cell Destination
                    null);                                    // Extra Params

            }
            else if (movementEvent != null)
            {
                movementFinished = true;
                if (turnAfterMove)
                {
                    switchDirection(turnDirection);
                    turnAfterMove = false;
                }
            }
		}
		
		if(IsMoving){
            // Update the progress
            this.movement.Update();
            this.movement.UpdateTextures();
            // If the movement has ended
			if(this.movement.Ended){
				paso = !paso;
			}
		}        
	}

    private MovementType getMovementTypeTo(Cell next)
    {
        MovementType type = MovementType.Lineal;
        if (Entity.Position.WalkingHeight != next.WalkingHeight)
            type = MovementType.Parabolic;

        return type;
    }

    /**
     * Sprite Management
     * */

    private IsoDecoration getSpritesheetForMovementType(MovementType type)
    {
        switch (type)
        {
            case MovementType.Lineal:
                return normalSprite;
            case MovementType.Parabolic:
                return jumpingSprite;
            case MovementType.Instant:
                return normalSprite;
            case MovementType.Turn:
                return normalSprite;
        }

        return normalSprite;
    }

	/**
	 * Movements
	 * */

	public enum MovementType { Lineal, Parabolic, Instant, Turn }
	
	private abstract class Movement {
		
        //Attributes
        protected Entity entity;
        protected Mover mover;
        protected Decoration dec;
        protected IsoDecoration sheet;
		protected Vector3 from,to;
        protected Cell origin, destination;
        protected MovementType type;
        protected float progress;

        //Property Readers
        public Vector3 From { get { return from; } }
        public Vector3 To { get { return to; } }
        public MovementType MovementType { get { return type; } }
        public float Progress { get { return progress; } }
        public void addProgress(float time){ this.progress += time; }
        public bool Ended { get { return Progress >= Duration; } }
        public abstract float Duration { get; }

        //Extra param input
        protected virtual void setParams(Dictionary<string, object> mParams){}

        /*************************
         * GENERIC UPDATES
         * ************************/
        public virtual void Update()
        {
            this.addProgress(Time.deltaTime);

            Vector3 myPosition = getPosition(),
                    otherPosition = To;

            // Update Direction (Only works if diferent origin than destination)
            if (myPosition.z < otherPosition.z) { mover.direction = Direction.North; }
            else if (myPosition.z > otherPosition.z) { mover.direction = Direction.South; }
            else if (myPosition.x < otherPosition.x) { mover.direction = Direction.East; }
            else if (myPosition.x > otherPosition.x) { mover.direction = Direction.West; }

            entity.transform.position = this.getPosition();
            if(this.Ended && this.entity.Position != destination){
                this.entity.Position = destination;
            }
        }

        public virtual void UpdateTextures()
        {
            if (this.dec == null)
                return;

            // Change the spritesheet
            this.setSpritesheet(sheet);

            // Step Addition
            int stepAdition = 0;
            if (dec.IsoDec.nCols > 1) // TODO: Wider spritesheets with more than 1 prite per step.
                if (Progress / Duration >= 0.15 && Progress / Duration <= 0.85)
                    stepAdition = ((mover.paso) ? 1 : 2);

            // Update tile
            dec.Tile = getDirectionIndex(mover.direction) * dec.IsoDec.nCols + stepAdition;
        }

        private void setSpritesheet(IsoDecoration isoDec)
        {
            if (this.dec.IsoDec != isoDec)
            {
                this.dec.IsoDec = isoDec;
                this.dec.updateTextures();
            }
        }

        /********************
         * Movement factory
         * ***************/
        public static Movement createMovement(MovementType type, Entity entity, Mover mover, Decoration dec, IsoDecoration sheet, 
            Vector3 from, Vector3 to, Cell origin, Cell destination, Dictionary<string, object> mParams)
        {
			Movement movement = null;
			
			switch(type){
            case MovementType.Lineal: movement = new LinealMovement(); break;
            case MovementType.Parabolic: movement = new ParabolicMovement(); break;
            case MovementType.Instant: movement = new InstantMovement(); break;
            case MovementType.Turn: movement = new TurnMovement(); break;
			default: movement = new LinealMovement(); break;
			}

            movement.setParams(mParams);

            movement.origin = origin;
            movement.destination = destination;
			
			movement.from = from;
			movement.to = to;
            movement.entity = entity;
            movement.dec = dec;
            movement.sheet = sheet;
            movement.mover = mover;
			
			return movement;
		}
        public Vector3 getPosition() { return getPositionAt(Progress/Duration); }
		public abstract Vector3 getPositionAt(float moment);

        /*******************
         * LINEAR MOVEMENT
         * *******************/
		
		private class LinealMovement : Movement{
            public override float Duration{ get { return 0.3f; }}

			public override Vector3 getPositionAt(float moment){
				if(moment >= 1)	return to;
				if(moment <= 0)	return from;
				
				return from + (to-from)*moment;
			}
		}

        /****************************
         * PARABOLIC (JUMP) MOVEMENT
         * ***************************/

		private class ParabolicMovement : Movement{
			private Vector3 vectorHeight;
			private bool setVectorHeight = true;

            public override float Duration{ get { return 0.3f; }} // TODO: Make parabolic movement longer if jumping up

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

        /*****************************
         * INSTANT (TELEPORT) MOVEMENT
         * ****************************/

        private class InstantMovement : Movement
        {
            public override float Duration{ get { return 0.00001f; } }
            public override Vector3 getPositionAt(float moment)
            {
                if (moment == 0) return from;
                else return to;
            }

            public override void Update()
            {
                this.addProgress(Time.deltaTime);

                if (entity.Position.Map == destination.Map)
                {
                    entity.Position = destination;
                }
                else
                {
                    //TODO Dont like the register calls made here...
                    entity.Position.Map.unRegisterEntity(entity);
                    entity.Position = destination;
                    entity.Position.Map.registerEntity(entity);

                    entity.GetComponent<Renderer>().enabled = false;
                    foreach (Renderer r in entity.GetComponentsInChildren<Renderer>()) r.enabled = false;
                    //TODO maybe this should be checked in player script
                    if (entity.GetComponent<Player>() != null)
                        MapManager.getInstance().setActiveMap(destination.Map);


                }
            }
        }

        /****************************
         * TURN (DIRECTION) MOVEMENT
         * ************************/
        private class TurnMovement : Movement
        {
            private Direction direction;

            public override float Duration{ get { return 0.00001f; } }
            public override Vector3 getPositionAt(float moment){ return from; }
            protected override void setParams(Dictionary<string, object> mParams)
            {
                base.setParams(mParams);
                object dir;
                mParams.TryGetValue("direction", out dir);
                if (dir != null && dir is Direction) direction = (Direction) dir;
            }

            public override void Update()
            {
                base.Update();
                mover.lastDirection = direction;
                mover.direction = direction;
            }
        }
	}
}
