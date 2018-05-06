using UnityEngine;
using System.Collections.Generic;
using IsoUnity;
using System;
using System.Collections;

namespace IsoUnity.Entities
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Decoration))]
    public class Mover : EventedEntityScript, SolidBody
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

        private Stack<IGameEvent> movementStack = new Stack<IGameEvent>();

        internal void PopDestination()
        {
            if(movementStack.Count > 0)
            {
                var toRestore = movementStack.Pop();
                Move(toRestore.getParameter("cell") as Cell, (int)toRestore.getParameter("distance"));
                movementEvent = toRestore;
            }
            
        }

        internal void PushDestination()
        {
            if(movementEvent != null)
            {
                movementStack.Push(movementEvent);
                RoutePlanifier.cancelRoute(this);
                movementEvent = null;
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
        private IGameEvent movementEvent;
       
     
        private Cell next;
        private Movement movement;

        private bool step = false;
        private Decoration dec;

        /****************
         * End Attributes
         * ***************/

         public bool Step { get { return step; } } 

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
        
        public void moveTo(Cell c, int distanceToMove = 0)
        {
            RoutePlanifier.planifyRoute(this, c, distanceToMove);
        }
        public void teleportTo(Cell c)
        {
            RoutePlanifier.cancelRoute(this);
            this.movement = Movement.createMovement(MovementType.Instant,
                this.Entity, this, this.transform.position,
                c.transform.position, this.Entity.Position, c, null);
        }

        private Movement createTurnMovement(Direction dir)
        {
            Dictionary<string, object> mParams = new Dictionary<string, object>();
            mParams["direction"] = dir;

            return Movement.createMovement(MovementType.Turn,
                this.Entity, this, this.transform.position,
                this.transform.position, this.Entity.Position, this.Entity.Position, mParams);
        }
         
        [GameEvent(false)]
        public void Move(Cell cell, int distance = 0, Direction direction = Direction.West)
        {
            FinalizeMove();
            var current = movementEvent = Current;
            RoutePlanifier.planifyRoute(this, cell, distance);
        }

        [GameEvent]
        public IEnumerator Follow(Entity follow, int distance = 1, bool stopOnArrive = true)
        {
            FinalizeMove();
            var current = movementEvent = Current;
            var followPosition = Entity.Position; // My position to force enter the while

            while (current == movementEvent && (!stopOnArrive || followPosition != follow.Position))
            {
                if(followPosition != follow.Position)
                {
                    followPosition = follow.Position;
                    RoutePlanifier.planifyRoute(this, followPosition, distance);
                }
                yield return new WaitWhile(() => (IsMoving || RoutePlanifier.hasRoute(this)) && followPosition == follow.Position && current == movementEvent);
            }
        }

        [GameEvent]
        public IEnumerator Turn(Direction direction)
        {
            // Cancel the current movement
            RoutePlanifier.cancelRoute(this);
            yield return new WaitWhile(() => IsMoving);
            // Do the turn
            FinalizeMove();
            var current = movementEvent = Current;
            movement = createTurnMovement(direction);
            yield return new WaitWhile(() => IsMoving);
        }

        [GameEvent]
        public IEnumerator Teleport(Cell cell)
        {
            // Cancel the current movement
            RoutePlanifier.cancelRoute(this);
            yield return new WaitWhile(() => IsMoving);
            // Do the teleport
            FinalizeMove();
            var current = movementEvent = Current;
            teleportTo(cell);
            yield return new WaitWhile(() => IsMoving && current == movementEvent);
        }

        [GameEvent]
        public IEnumerator Stop()
        {
            RoutePlanifier.cancelRoute(this);
            yield return new WaitWhile(() => IsMoving);
        }

        /**************
         * Event Control
         * *************/

        private Direction lastDirection = Direction.West;

        public override void tick()
        {
            base.tick();

            if (this.dec == null && Entity)
                this.dec = Entity.decoration;

            // Direction change responsive update
            if (!IsMoving && movementEvent == null)
                if (lastDirection != direction)
                    movement = createTurnMovement(direction);

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
                return (movement != null && !movement.Ended);
            }
        }

        private IsoAnimation.Frame frame(int column, float duration){
            return new IsoAnimation.Frame(){
                column = column,
                duration = duration
            };
        }

        void Awake()
        {
            if (Application.isEditor)
            {
                this.normalSprite = this.GetComponent<Decoration>().IsoDec;
                this.jumpingSprite = this.GetComponent<Decoration>().IsoDec;
            }
        }

        private void SetUpSheets(bool replace = false)
        {
            var decorationAnimator = Entity.decorationAnimator;
            if (decorationAnimator == null)
                return;

            var normal = Entity.decorationAnimator.sheets.Find(s => s.name == "normal");
            var jump = Entity.decorationAnimator.sheets.Find(s => s.name == "jump");

            if (replace)
            {
                decorationAnimator.sheets.Remove(normal);
                normal = null;
                decorationAnimator.sheets.Remove(jump);
                jump = null;
            }
            
            // Normal sheet
            if(normal == null)
            {
                decorationAnimator.sheets.Add(new DecorationAnimator.NameIsoDecoration()
                {
                    name = "normal",
                    isoDecoration = normalSprite
                });
            } 
            else
            {
                normal.isoDecoration = normalSprite;
            }

            // Jump sheet
            if(jump == null)
            {
                decorationAnimator.sheets.Add(new DecorationAnimator.NameIsoDecoration()
                {
                    name = "jump",
                    isoDecoration = jumpingSprite
                });
            } 
            else
            {
                jump.isoDecoration = jumpingSprite;
            }
        }

        private void SetUpAnimations(bool replace = false)
        {
            var decorationAnimator = Entity.decorationAnimator;
            if (decorationAnimator == null)
                return;

            var idle = Entity.decorationAnimator.isoAnimations.Find(i => i.name == "idle");
            var leftStep = Entity.decorationAnimator.isoAnimations.Find(i => i.name == "left step");
            var rightStep = Entity.decorationAnimator.isoAnimations.Find(i => i.name == "right step");
            var jump = Entity.decorationAnimator.isoAnimations.Find(i => i.name == "jump");

            if (replace)
            {
                decorationAnimator.isoAnimations.Remove(idle);
                idle = null;
                decorationAnimator.isoAnimations.Remove(leftStep);
                leftStep = null;
                decorationAnimator.isoAnimations.Remove(rightStep);
                rightStep = null;
                decorationAnimator.isoAnimations.Remove(jump);
                jump = null;
            }
            
            // Left step animation
            if(idle == null)
            {
                var idleAnimation = ScriptableObject.CreateInstance<IsoAnimation>();
                idleAnimation.sheet = "normal";
                idleAnimation.frames.Add(frame(0, 0)); // Instant
                idleAnimation.loop = true;

                decorationAnimator.isoAnimations.Add(new DecorationAnimator.NameIsoAnimation()
                {
                    name = "idle",
                    isoAnimation = idleAnimation
                });
            } 

            // Left step animation
            if(leftStep == null)
            {
                var leftStepAnim = ScriptableObject.CreateInstance<IsoAnimation>();
                leftStepAnim.sheet = "normal";
                leftStepAnim.frames = new List<IsoAnimation.Frame>()
                {
                    frame(0, 0.075f), 
                    frame(1, 0.15f), 
                    frame(0, 0.075f)
                };
                Entity.decorationAnimator.isoAnimations.Add(new DecorationAnimator.NameIsoAnimation()
                {
                    name = "left step",
                    isoAnimation = leftStepAnim
                });
            } 

            // Right step animation
            if(!Entity.decorationAnimator.isoAnimations.Exists(i => i.name == "right step"))
            {
                var rightStepAnim = ScriptableObject.CreateInstance<IsoAnimation>();
                rightStepAnim.sheet = "normal";
                rightStepAnim.frames = new List<IsoAnimation.Frame>()
                {
                    frame(0, 0.075f), 
                    frame(2, 0.15f), 
                    frame(0, 0.075f)
                };
                Entity.decorationAnimator.isoAnimations.Add(new DecorationAnimator.NameIsoAnimation()
                {
                    name = "right step",
                    isoAnimation = rightStepAnim
                });
            }

            // Jump animation
            if(!Entity.decorationAnimator.isoAnimations.Exists(i => i.name == "jump"))
            {
                var jumpAnim = ScriptableObject.CreateInstance<IsoAnimation>();
                jumpAnim.sheet = "jump";
                jumpAnim.frames = new List<IsoAnimation.Frame>()
                {
                    frame(0, 0.3f)
                };
                Entity.decorationAnimator.isoAnimations.Add(new DecorationAnimator.NameIsoAnimation()
                {
                    name = "jump",
                    isoAnimation = jumpAnim
                });
            }
        }
        private bool entityReady = false;
        private void OnValidate()
        {
            if(entityReady)
            {
                tick();
                this.Update();
                SetUpSheets(false);
                SetUpAnimations(false);
            }
        }

        public override void OnEntityReady() 
        {
            entityReady = true;
            tick();
            this.Update();
            SetUpSheets(false);
            SetUpAnimations(false);
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
            if (!IsMoving)
            {
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
                }else
                    FinalizeMove();
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
                    step = !step;
                }
            }
        }

        private void FinalizeMove()
        {
            if (movementEvent != null && movementEvent.Name == "move")
            {
                Game.main.eventFinished(movementEvent);
                movementEvent = null;
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
         * Movements
         * */

        public enum MovementType { Lineal, Parabolic, Instant, Turn, Fade }

        private abstract class Movement
        {

            //Attributes
            protected Entity entity;
            protected Mover mover;
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
            private bool started = false;
            private static int start=3;

            //Extra param input
            protected virtual void setParams(Dictionary<string, object> mParams) { }
            // Start
            protected virtual void Start() { }

            /*************************
             * GENERIC UPDATES
             * ************************/
            public virtual void Update(float progress)
            {
                if (!started)
                {
                    started = true;
                    Start();
                }

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
                if (mover.useAnimator)
                {
                    // Update tile
                    Animator anim = mover.GetComponent<Animator>();
                    anim.SetInteger("direction", (int)mover.direction);
                    if (start>0)
                    {
                        anim.SetFloat("speed", 0f);
                        start --;
                    }
                    else
                    {
                        anim.SetFloat("speed", 1f);
                    }

                }
            }

            /********************
             * Movement factory
             * ***************/
            public static Movement createMovement(MovementType type, Entity entity, Mover mover,
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
                bool sonido = false;

                protected override void Start() {
                    if (entity.mover.Step) entity.decorationAnimator.Play("left step", "idle");
                    else entity.decorationAnimator.Play("right step", "idle");
                }

                public override void Update(float progress)
                {
                   
                    base.Update(progress);

                    if(progress > 0.5 && !sonido)
                    {
                        sonido = true;
                        var audio = entity.gameObject.GetComponent<AudioSource>();
                        if (audio)
                            audio.Play();
                    }

                    if (this.Ended && mover.useAnimator)
                    {
                        Animator anim = mover.GetComponent<Animator>();
                        if (anim)
                            anim.SetFloat("speed", 0f);
                    }
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

                protected override void Start() {
                    entity.decorationAnimator.Play("jump", "idle");
                }

                public override void Update(float progress)
                {
                    base.Update(progress);
                    
                    if (this.Ended && mover.useAnimator)
                    {
                        Animator anim = mover.GetComponent<Animator>();
                        if (anim)
                            anim.SetFloat("speed", 0f);
                    }
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
                    positionToCell.y = positionToCell.y - entity.Position.WalkingHeight;

                    if (entity.Position.Map == destination.Map)
                    {
                        entity.Position = destination;
                        entity.transform.localPosition = new Vector3(positionToCell.x, destination.WalkingHeight + positionToCell.y, positionToCell.z);
                    }
                    else
                    {
                        //TODO Dont like the register calls made here...
                        entity.Position.Map.unRegisterEntity(entity);
                        entity.Position = destination;
                        entity.Position.Map.registerEntity(entity);
                        entity.transform.localPosition = new Vector3(positionToCell.x, destination.WalkingHeight + positionToCell.y, positionToCell.z);

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

                protected override void Start() {
                    entity.decorationAnimator.Play("idle");
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

                protected override void Start() {
                    if (entity.mover.Step) entity.decorationAnimator.Play("left step", "right step", "idle");
                    else entity.decorationAnimator.Play("right step", "left step", "idle");
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
                        CameraManager.Instance.Flush();
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