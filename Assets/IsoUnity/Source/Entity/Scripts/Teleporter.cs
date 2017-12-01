using UnityEngine;
using System.Collections;
using IsoUnity;
using IsoUnity.Sequences;

namespace IsoUnity.Entities
{
    public class Teleporter : EntityScript
    {
        public Cell destination;
        public int mode = 0;
        public SerializableGameEvent sge;
        public Checkable checkable;

        private bool start = true;

        public override Option[] getOptions()
        {
            return new Option[] { new Option("Teleport", null, 0) };
        }

        public override void tick()
        {

            if (mode == 2)
                enabled = checkable.check();

            if (destination == null || !enabled)
                return;

            var entities = Entity.Position.Entities;
            foreach (Entity e in entities)
            {
                if (e != this.Entity)
                {
                    GameEvent ge = new GameEvent();
                    ge.Name = "teleport";
                    ge.setParameter("mover", e.mover);
                    ge.setParameter("cell", destination);
                    Game.main.enqueueEvent(ge);
                }
            }
        }

        public override void eventHappened(IGameEvent ge)
        {
            if (mode == 1 && this.sge == ge)
                enabled = true;
        }

        public override void Update()
        {
            if (start)
            {
                enabled = mode == 0;
            }
        }
    }
}