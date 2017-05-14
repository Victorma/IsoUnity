using UnityEngine;
using System.Collections;

using IsoUnity;
using IsoUnity.Sequences;

namespace IsoUnity.Entities
{
    public class RandomMovement : EntityScript
    {

       

        public float probability;

        public override Option[] getOptions()
        {
            return new Option[] { };
        }

        public override void tick()
        {
            float t = probability * Time.deltaTime;
            if (Random.Range(0f, 1f) < t)
            {
                var talker = this.GetComponent<Talker>();
                if(talker && Game.main.GetComponent<SequenceManager>().Executing.Contains(talker.Sequence))
                {
                    Cell c = Entity.Position.Map.getNeightbours(Entity.Position)[Random.Range(0, 4)];
                    if (c != null)
                    {
                        GameEvent ge = new GameEvent();
                        ge.Name = "move";
                        ge.setParameter("mover", this.Entity.mover);
                        ge.setParameter("cell", c);
                        Game.main.enqueueEvent(ge);
                    }
                }
            }
        }

        public override void eventHappened(IGameEvent ge)
        {
        }

        public override void Update()
        {
        }


    }
}