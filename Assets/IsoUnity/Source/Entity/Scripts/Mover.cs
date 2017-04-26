using UnityEngine;
using System.Collections.Generic;
using IsoUnity;
using System;

namespace IsoUnity.Entities
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Decoration))]
    public class Mover : EntityScript, SolidBody
    {
        [SerializeField]
        private bool useAnimator = false;

        /***************************
        * Directions Enum & Utils
        **************************/
        public enum Direction
        {
            North, East, South, West
        }
        public static int getDirectionIndex(Direction d)
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

        public static Direction getDirectionFromTo(Transform from, Transform to)
        {
            Vector3 fromTo = to.position - from.position;
            //fromTo = new Vector3(fromTo.x, 0, fromTo.z);

            float angle = Vector3.Angle(fromTo, Vector3.right) * Mathf.Deg2Rad;

            if (fromTo.z < 0) angle = -angle;
            if (angle < 0) angle += Mathf.PI * 2f;

            float Pi4 = Mathf.PI / 4f;

            if (angle < Pi4 || angle > 7 * Pi4)
            {
                return Direction.East;
            }
            else if (angle < 3 * Pi4)
            {
                return Direction.North;
            }
            else if (angle < 5 * Pi4)
            {
                return Direction.West;
            }
            else
            {
                return Direction.South;
            }
        }
        /****************
        * End Directions
        *****************/

        /****************
         * Begin Attributes
         * ****************/

        /// <summary>
        /// Used to know if you can be blocked in paths
        /// </summary>
        public bool canBlockMe = true;
        /// <summary>
        /// Used to know if this blocks
        /// </summary>
        public bool blocks = true;
        /// <summary>
        /// Max cell difference jump
        /// </summary>
        public float maxJumpSize = 1.5f;
        public IsoDecoration normalSprite;
        public IsoDecoration jumpingSprite;
        public Direction direction;

        // Move
        private Cell moveToCell;
        private Entity follow;
        private bool move = false;
        private bool movementFinished = false;
        private int distanceToMove = 0;
        private IGameEvent bcEvent;
        private IGameEvent movementEvent;

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

        /******************
         * MOVEMENT CONTROL
         * *****************/

        public bool CanMoveTo(Cell c) { return CanMoveTo(this.Entity.Position, c); }
        public bool CanMoveTo(Cell from, Cell to)
        {
            bool canMove = false;

            // Destination not null and jumpsize
            // TODO si retorno el tipo de movimiento puedo meter teletransportes entre medias
            if (to != null && Mathf.Abs(from.WalkingHeight - to.WalkingHeight) <= maxJumpSize)
                canMove = canBlockMe ? to.isAccesibleBy(this.Entity) : true;

            return canMove;
        }


        public bool LetsPass(SolidBody sb)
        {
            return !blocks;
        }

        public bool CanGoThrough(SolidBody sb)
        {
            return !canBlockMe;
        }

        /*******************
         * PUBLIC OPERATIONS
         * ******************/
        
        public void moveTo(Cell c)
        {
            RoutePlanifier.planifyRoute(this, c, distanceToMove);
        }
        public void teleportTo(Cell c)
        {
            RoutePlanifier.cancelRoute(this);
            this.movement = Movement.createMovement(MovementType.Instant,
                this.Entity, this, dec, normalSprite, this.transform.position,
                c.transform.position, this.Entity.Position, c, null);
        }

        public void switchDirection(Direction direction)
        {
            RoutePlanifier.cancelRoute(this);
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
        public override void eventHappened(IGameEvent ge)
        {
            if (!ge.belongsTo(this))
                return;

            switch (ge.Name.ToLower())
            {
                case "turn":
                    {
                        if (!this.IsMoving) // If it's moving, just ignore the turn
                        {
                            object dir = ge.getParameter("direction");
                            if (dir != null && dir is Direction)
                                this.turnDirection = (Direction)dir;

                            this.turn = true;
                        }
                    }
                    break;
                case "move":
                    {
                        this.follow = (Entity)ge.getParameter("follow");
                        if(follow)
                            this.moveToCell = follow.Position;
                        else
                            this.moveToCell = (Cell)ge.getParameter("cell");

                        this.move = moveToCell || follow;

                        distanceToMove = (ge.getParameter("distance") != null) ? (int)ge.getParameter("distance") : 0;
                        object dir = ge.getParameter("direction");
                        if (dir != null && dir is Direction)
                        {
                            this.turnDirection = (Direction)dir;
                            this.turnAfterMove = true;
                        }

                        this.bcEvent = ge;
                    }
                    break;

                case "teleport":
                    {
                        teleport = true;
                        teleportToCell = ge.getParameter("Cell") as Cell;
                    }
                    break;

                case "stop":
                    {
                        stop = true;
                    }
                    break;
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

        public override void tick()
        {
            if (this.dec == null)
                this.dec = Entity.decoration;

            if (movementFinished)
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
            else if (teleport)
            {
                if (IsMoving)
                    finalizeMovementEvent();

                teleportTo(teleportToCell);
                teleportToCell = null;
                teleport = false;
            }
            else if (move)
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

        public override Option[] getOptions()
        {
            return new Option[] { };
        }

        public bool IsMoving
        {
            get
            {
                return movement != null && !movement.Ended;
            }
        }
        private Cell next;
        private Movement movement;

        private bool paso = false;
        private Decoration dec;

        void Awake()
        {
            if (Application.isEditor)
            {
                this.normalSprite = this.GetComponent<Decoration>().IsoDec;
                this.jumpingSprite = this.GetComponent<Decoration>().IsoDec;
            }
        }

        void OnValidate()
        {
            tick();
            this.Update();
        }

        public override void Update()
        {
            if (this.dec == null)
                this.dec = GetComponent<Decoration>();

            // Force initial direction update
            if (this.movement == null)
            {
                movement = createTurnMovement(direction);
                movement.addProgress(1f); // Move to End
                movement.Update(Time.deltaTime); // Assure finalization
                movement.UpdateTextures();
            }

            // If we're not moving right now
            if (!IsMoving && RoutePlanifier.hasRoute(this))
            {
                if(follow)
                    RoutePlanifier.planifyRoute(this, follow.Position, distanceToMove);

                // Let's see if we have something to do...
                next = RoutePlanifier.next(this);
                //Then, let's move :D
                if (next != null)
                {
                    if (next.isAccesibleBy(this.Entity))
                    {
                        Vector3 myPosition = this.Entity.Position.transform.localPosition,
                             otherPosition = next.transform.localPosition;
                        Dictionary<string, object> p = new Dictionary<string, object>();

                        // Define the movement type and set the tilesheet
                        MovementType type = getMovementTypeTo(next, p);
                        Vector3 offset = this.transform.localPosition - new Vector3(0, this.Entity.Position.WalkingHeight + transform.localScale.y / 2, 0);
                        this.movement = Movement.createMovement(type, // type
                            this.Entity,                              // Entity
                            this,                                     // Mover
                            dec,                                      // Decoration 
                            getSpritesheetForMovementType(type),      // Sheet
                            transform.position,                       // Origin
                            next.transform.position + new Vector3(0, next.WalkingHeight + transform.localScale.y / 2, 0) + offset, // Destination
                            Entity.Position,                          // Cell Origin
                            next,                                     // Cell Destination
                            p);                                    // Extra Params
                    }
                    else
                    {
                        RoutePlanifier.cancelRoute(this);
                    }
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

            if (IsMoving)
            {
                // Update the progress
                if (!Application.isPlaying)
                {
                    this.movement.Update(1f);
                }
                this.movement.Update(Time.deltaTime);
                this.movement.UpdateTextures();
                // If the movement has ended
                if (this.movement.Ended)
                {
                    paso = !paso;
                }
            }
        }

        private MovementType getMovementTypeTo(Cell next, Dictionary<string, object> par)
        {
            var vn = Entity.Position.VirtualNeighbors.FindLast(v => v.Destination == next);
            if (vn != null)
            {
                par.Add("output", vn.OutputDirection);
                par.Add("input", vn.InputDirection);
                return MovementType.Fade;
            }

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
                case MovementType.Fade:
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

        public enum MovementType { Lineal, Parabolic, Instant, Turn, Fade }

        private abstract class Movement
        {

            //Attributes
            protected Entity entity;
            protected Mover mover;
            protected Decoration dec;
            protected IsoDecoration sheet;
            protected Vector3 from, to;
            protected Cell origin, destination;
            //protected MovementType type;
            protected float progress;

            //Property Readers
            public Vector3 From { get { return from; } }
            public Vector3 To { get { return to; } }
            //public MovementType MovementType { get { return type; } }
            public float Progress { get { return progress; } }
            public void addProgress(float time) { this.progress += time; }
            public bool Ended { get { return Progress >= Duration; } }
            public abstract float Duration { get; }

            //Extra param input
            protected virtual void setParams(Dictionary<string, object> mParams) { }

            /*************************
             * GENERIC UPDATES
             * ************************/
            public virtual void Update(float progress)
            {
                this.addProgress(progress);
                if (!destination.Influences.Contains(entity))
                    destination.Influences.Add(entity);

                Vector3 myPosition = getPosition(),
                        otherPosition = To;

                // Update Direction (Only works if diferent origin than destination)
                mover.direction = getDirection();

                entity.transform.position = this.getPosition();
                if (this.Ended && this.entity.Position != destination)
                {
                    this.entity.Position.Influences.Remove(entity);
                    this.entity.Position = destination;
                }
            }

            public virtual void UpdateTextures()
            {
                if (this.dec == null)
                    return;

                // Change the spritesheet
                this.setSpritesheet(sheet);

                if (mover.useAnimator)
                {
                    // Update tile
                    Animator anim = mover.GetComponent<Animator>();
                    anim.SetInteger("direction", (int)mover.direction);
                    anim.SetFloat("speed", Ended ? 0f : 1f);
                }
                else
                {
                    // Step Addition
                    int stepAdition = 0;
                    if (dec.IsoDec.nCols > 1) // TODO: Wider spritesheets with more than 1 prite per step.
                        if (Progress / Duration >= 0.15 && Progress / Duration <= 0.85)
                            stepAdition = ((mover.paso) ? 1 : 2);

                    dec.Tile = getDirectionIndex(mover.direction) * dec.IsoDec.nCols + stepAdition;
                }
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

                switch (type)
                {
                    case MovementType.Lineal: movement = new LinealMovement(); break;
                    case MovementType.Parabolic: movement = new ParabolicMovement(); break;
                    case MovementType.Instant: movement = new InstantMovement(); break;
                    case MovementType.Turn: movement = new TurnMovement(); break;
                    case MovementType.Fade: movement = new FadeMovement(); break;
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
            public Vector3 getPosition() { return getPositionAt(Progress / Duration); }
            public abstract Vector3 getPositionAt(float moment);

            protected virtual Direction getDirection() { return getDirectionAt(Progress / Duration); }
            protected virtual Direction getDirectionAt(float moment)
            {

                Vector3 myPosition = getPosition(),
                        otherPosition = To;

                Direction direction = mover.direction;
                if (myPosition.z < otherPosition.z) { direction = Direction.North; }
                else if (myPosition.z > otherPosition.z) { direction = Direction.South; }
                else if (myPosition.x < otherPosition.x) { direction = Direction.East; }
                else if (myPosition.x > otherPosition.x) { direction = Direction.West; }

                return direction;
            }

            /*******************
             * LINEAR MOVEMENT
             * *******************/

            private class LinealMovement : Movement
            {
                public override float Duration { get { return 0.3f; } }

                public override Vector3 getPositionAt(float moment)
                {
                    if (moment >= 1) return to;
                    if (moment <= 0) return from;

                    return from + (to - from) * moment;
                }
            }

            /****************************
             * PARABOLIC (JUMP) MOVEMENT
             * ***************************/

            private class ParabolicMovement : Movement
            {
                private Vector3 vectorHeight;
                private bool setVectorHeight = true;

                public override float Duration { get { return 0.3f; } } // TODO: Make parabolic movement longer if jumping up

                public override Vector3 getPositionAt(float moment)
                {
                    if (setVectorHeight)
                    {
                        setVectorHeight = false;
                        float jumpHeight = 0.25f;
                        vectorHeight = new Vector3(0, Mathf.Abs(to.y - from.y) / 2f + jumpHeight, 0);
                    }

                    if (moment >= 1) return to;
                    if (moment <= 0) return from;

                    return from + (to - from) * moment + vectorHeight * (1f - 4f * Mathf.Pow(moment - 0.5f, 2));
                }
            }

            /*****************************
             * INSTANT (TELEPORT) MOVEMENT
             * ****************************/

            private class InstantMovement : Movement
            {
                public override float Duration { get { return 0.00001f; } }
                public override Vector3 getPositionAt(float moment)
                {
                    if (moment == 0) return from;
                    else return to;
                }

                public override void Update(float progress)
                {
                    this.addProgress(progress);
                    var positionToCell = entity.transform.position - entity.Position.transform.position;

                    if (entity.Position.Map == destination.Map)
                    {
                        entity.Position = destination;
                        entity.transform.localPosition = new Vector3(positionToCell.x, destination.Height + destination.WalkingHeight, positionToCell.z);
                    }
                    else
                    {
                        //TODO Dont like the register calls made here...
                        entity.Position.Map.unRegisterEntity(entity);
                        entity.Position = destination;
                        entity.Position.Map.registerEntity(entity);
                        entity.transform.localPosition = new Vector3(positionToCell.x, destination.Height + destination.WalkingHeight, positionToCell.z);

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

                public override float Duration { get { return 0.00001f; } }
                public override Vector3 getPositionAt(float moment) { return from; }
                protected override void setParams(Dictionary<string, object> mParams)
                {
                    base.setParams(mParams);
                    object dir;
                    mParams.TryGetValue("direction", out dir);
                    if (dir != null && dir is Direction) direction = (Direction)dir;
                }

                public override void Update(float progress)
                {
                    base.Update(progress);
                    mover.lastDirection = direction;
                    mover.direction = direction;
                }
            }

            private class FadeMovement : Movement
            {
                private Direction outputDirection;
                private Direction inputDirection;

                public override float Duration { get { return .6f; } }
                protected override void setParams(Dictionary<string, object> mParams)
                {
                    base.setParams(mParams);
                    object dir;
                    mParams.TryGetValue("output", out dir);
                    if (dir != null && dir is Direction) outputDirection = (Direction)dir;
                    mParams.TryGetValue("input", out dir);
                    if (dir != null && dir is Direction) inputDirection = (Direction)dir;
                }

                public override Vector3 getPositionAt(float moment)
                {
                    Vector3 position = Vector3.zero;
                    if(moment < 0.5f)
                    {
                        // Salida
                        position = from + DirectionToVector(outputDirection) * (moment);
                    }
                    else
                    {
                        // Entrada
                        position = to - DirectionToVector(inputDirection) * (1f-moment);
                    }
                    return position;
                }

                public override void Update(float progress)
                {
                    var prevPos = entity.Position;

                    base.Update(progress);
                    Renderer renderer = mover.GetComponent<Renderer>();
                    Color c = mover.GetComponent<Renderer>().material.color;
                       
                    renderer.material.color = new Color(Mathf.Abs((progress * 2f) - 1f), Mathf.Abs((progress * 2f) - 1f), Mathf.Abs((progress * 2f) - 1f), Mathf.Abs((progress * 2f) - 1f));

                    if (Progress / Duration > .5f && prevPos.Map != destination.Map) {
                        //TODO Dont like the register calls made here...
                        prevPos.Map.unRegisterEntity(entity);
                        entity.Position.Map.registerEntity(entity);

                        //TODO maybe this should be checked in player script
                        if (entity.GetComponent<Player>() != null)
                        {
                            entity.GetComponent<Renderer>().enabled = true;
                            foreach (Renderer r in entity.GetComponentsInChildren<Renderer>()) r.enabled = true;
                            MapManager.getInstance().setActiveMap(destination.Map);
                        }
                        else
                        {
                            entity.GetComponent<Renderer>().enabled = false;
                            foreach (Renderer r in entity.GetComponentsInChildren<Renderer>()) r.enabled = false;
                        }
                    }
                }

                protected override Direction getDirectionAt(float moment)
                {
                    return moment < 0.5f ? outputDirection : inputDirection;
                }
            }
        }

        public static Vector3 DirectionToVector(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                    return Vector3.forward;
                case Direction.East:
                    return Vector3.right;
                case Direction.South:
                    return Vector3.back;
                case Direction.West:
                    return Vector3.left;
            }

            return Vector3.zero;
        }

        private void OnDestroy()
        {
            if (this.Entity.Position)
                this.Entity.Position.Influences.Remove(this.Entity);
        }
    }
}