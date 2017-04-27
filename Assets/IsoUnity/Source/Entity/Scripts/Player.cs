using UnityEngine;
using System.Collections;
using IsoUnity;

namespace IsoUnity.Entities
{
    public class Player : EntityScript
    {

        public override void eventHappened(IGameEvent ge)
        {
            // If we're waiting for the event finished
            if (movement != null)
                if (ge.Name.ToLower() == "event finished")
                    // Let's check it's our event:
                    if (ge.getParameter("event") == movement)
                    {
                        // Ok it's done
                        movement = null;
                    }
        }

        private GameEvent movement;
        private Cell actionCell;
        private Entity actionEntity;
        private IGameEvent toLaunch;
        private bool movingArrow;

        [SerializeField]
        private bool active = true;
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                this.active = value;
            }
        }

        public void onControllerEvent(ControllerEventArgs args)
        {
            // # Avoid responding controller event when inactive
            if (!active)
                return;

            // # Normal threatment
            // Multiple controller events only give one launch result per tick
            if (toLaunch == null)
            {
                // If options received (from entities)
                if (args.options != null)
                {
                    // If there's only one, proceed to launch
                    if (args.options.Length == 1)
                    {
                        // The action is the event we have to launch, 
                        // so in order to know who's launching, we put a mark
                        if (args.options[0].Action != null)
                            args.options[0].Action.setParameter("Executer", this.Entity);

                        actionCell = args.cell;
                        actionEntity = args.entity;

                        // If we've to move to perform the action
                        if (args.options[0].HasToMove)
                        {
                            GameEvent ge = new GameEvent();
                            ge.setParameter("mover", this.Entity.mover);
                            ge.setParameter("follow", args.entity);
                            ge.setParameter("distance", args.options[0].Distance);
                            ge.setParameter("synchronous", true);
                            ge.Name = "follow";
                            movement = ge;
                            movingArrow = false;
                            Game.main.enqueueEvent(ge);

                            // Here we've launched the movement. As it's synchronous, we'll receive 
                            // the movement finished when the Mover considers it's done.
                        }

                        toLaunch = args.options[0].Action;
                    }
                    // If there're multiple actions we have to display a menu so player can choose
                    else if (args.options.Length > 1)
                    {
                        OptionsGUI gui = ScriptableObject.CreateInstance<OptionsGUI>();
                        gui.init(args, Camera.main.WorldToScreenPoint(args.entity.transform.position), args.options);
                        GUIManager.addGUI(gui, 100);
                    }
                }
                // If the argument doesn't contain options but it has a cell, we'll try to move over there
                else if (args.cell != null)
                {
                    var cell = args.cell;
                    if(args.cell.VirtualNeighbors.Count > 0)
                    {
                        cell = args.cell.VirtualNeighbors[0].Destination;
                    }

                    GameEvent ge = new GameEvent();
                    ge.setParameter("mover", this.Entity.mover);
                    ge.setParameter("cell", cell);
                    ge.Name = "move";
                    Game.main.enqueueEvent(ge);
                }
                // Otherwise, the controller event should contain keys pressed
                else
                {

                    int to = -1;
                    if (args.LEFT) { to = 0; }
                    else if (args.UP) { to = 1; }
                    else if (args.RIGHT) { to = 2; }
                    else if (args.DOWN) { to = 3; }

                    if (to > -1)
                    {
                        if (movement == null || !movingArrow)
                        {
                            if (Entity == null)
                                Debug.Log("Null!");
                            Cell destination = Entity.Position.Map.getNeightbours(Entity.Position)[to];
                            // Can move to checks if the entity can DIRECT move to this cells.
                            // This should solve bug #29
                            Mover em = this.Entity.GetComponent<Mover>();
                            if (em != null && em.CanMoveTo(destination))
                            {
                                GameEvent ge = new GameEvent();
                                ge.setParameter("mover", this.Entity.mover);
                                ge.setParameter("cell", destination);
                                ge.setParameter("synchronous", true);
                                ge.Name = "move";
                                Game.main.enqueueEvent(ge);
                                movement = ge;
                            }
                            else
                            {
                                GameEvent ge = new GameEvent();
                                ge.setParameter("mover", this.Entity.mover);
                                ge.setParameter("direction", fromIndex(to));
                                ge.setParameter("synchronous", true);
                                ge.Name = "turn";
                                Game.main.enqueueEvent(ge);
                                movement = ge;
                            }
                            movingArrow = true;
                        }
                    }
                }
            }
        }

        private Mover.Direction fromIndex(int i)
        {
            switch (i)
            {
                case 0: return Mover.Direction.North;
                case 1: return Mover.Direction.East;
                case 2: return Mover.Direction.South;
                case 3: return Mover.Direction.West;
            }

            return Mover.Direction.North;
        }

        private bool registered = false;

        public override void tick()
        {

            if (toLaunch != null)
            {
                // If we're not waiting to receive event finished from Mover.
                if (movement == null)
                {
                    // That means that, we're in the right position so...
                    Game.main.enqueueEvent(toLaunch);
                    toLaunch = null;

                    // Look to the action
                    if(actionCell || actionEntity)
                    {
                        Mover.Direction toLook = Mover.getDirectionFromTo(Entity.transform, actionEntity ? actionEntity.transform : actionCell.transform);
                        GameEvent ge = new GameEvent("turn");
                        ge.setParameter("mover", Entity.mover);
                        ge.setParameter("direction", toLook);
                        Game.main.enqueueEvent(ge);
                    }

                    actionCell = null;
                    actionEntity = null;
                }
            }

            if (!registered)
            {
                ControllerManager.onControllerEvent += this.onControllerEvent;
                registered = true;
            }

        }

        public override Option[] getOptions()
        {
            return new Option[] { };
        }

        public override void Update()
        {

        }
    }
}