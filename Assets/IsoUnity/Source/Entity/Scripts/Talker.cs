using UnityEngine;
using System.Collections.Generic;
using IsoUnity;
using IsoUnity.Sequences;

namespace IsoUnity.Entities
{
    [ExecuteInEditMode]
    public class Talker : EntityScript
    {

        private bool start = false;
        private Mover.Direction talkerDirection;

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

        public override void eventHappened(IGameEvent ge)
        {
            if (ge.getParameter("Talker") == this)
            {
                switch (ge.Name.ToLower())
                {
                    case "talk":
                        start = true;
                        talkerDirection = ((Entity)ge.getParameter("Executer")).GetComponent<Mover>().direction;
                        break;
                }
            }
        }

        public override Option[] getOptions()
        {
            GameEvent ge = new GameEvent();
            ge.Name = "talk";
            ge.setParameter("Talker", this);
            Option option = new Option("Talk", ge, true, 1);
            return new Option[] { option };
        }

        public override void tick()
        {
            if (start)
            {
                GameEvent ge = new GameEvent();
                ge.Name = "start sequence";
                ge.setParameter("sequence", sequence);
                Game.main.enqueueEvent(ge);
                start = false;
                int dir = (((int)talkerDirection) + 2) % 4;
                this.GetComponent<Mover>().switchDirection((Mover.Direction)dir);
            }
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