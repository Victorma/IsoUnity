using UnityEngine;
using System.Collections.Generic;
using IsoUnity;
using IsoUnity.Sequences;
using System.Collections;

namespace IsoUnity.Entities
{
    [ExecuteInEditMode]
    public class Talker : EventedEntityScript
    {
        private void Awake()
        {
            if (sequence == null)
                sequence = ScriptableObject.CreateInstance<Sequence>();
        }

        [SerializeField]
        private Sequence sequence;
        public Sequence Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }


        public override Option[] getOptions()
        {
            GameEvent ge = new GameEvent();
            ge.Name = "talk";
            ge.setParameter("talker", this);
            Option option = new Option("Talk", ge, true, 1);
            return new Option[] { option };
        }

        [GameEvent]
        public IEnumerator Talk(Entity executer = null)
        {
            Entity.mover.PushDestination();

            if (executer)
            {
                GameEvent turn = new GameEvent("turn");
                turn.setParameter("mover", Entity.mover);
                turn.setParameter("direction", Mover.getDirectionFromTo(transform, executer.transform));
                Game.main.enqueueEvent(turn);

            }

            GameEvent ge = new GameEvent();
            ge.Name = "start sequence";
            ge.setParameter("sequence", sequence);
            ge.setParameter("synchronous", true);
            Game.main.enqueueEvent(ge);

            yield return new WaitForEventFinished(ge);

            Entity.mover.PopDestination();
        }

        public override void Update()
        {
            /*this.delay += Time.deltaTime;

            if (this.delay > 0.05) {
                if (word < this.msg.Length) {
                    this.displaymsg = this.displaymsg + this.msg[word];
                    this.word++;
                }
                this.delay = 0;
            }*/
        }
    }
}