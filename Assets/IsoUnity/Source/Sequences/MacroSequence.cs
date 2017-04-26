using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsoUnity.Sequences {
    [RequireComponent(typeof(SequenceLauncher))]
    public class MacroSequence : EventManager {

        private const string MACRO_EVENT = "start macro";
        private const string MACRO_NAME_PARAM = "macro";

        public Sequence Sequence { get { return sequence; } }

        [SerializeField]
        private string macroName;
        [SerializeField]
        private Sequence sequence;

        public void Init(){
            Awake();
        }

        void Awake(){
            if(sequence == null)
                sequence = ScriptableObject.CreateInstance<Sequence>();
            
            GetComponent<SequenceLauncher>().Sequence = this.Sequence;
        }

        private bool launch = false;
        private bool synchronous = false;
        private bool finished = true;
        private IGameEvent toFinish = null;

        #region implemented abstract members of EventManager

        public void Launch()
        {
            launch = true;
        }

        public override void ReceiveEvent(IGameEvent ev)
        {
            if (ev.Name == MACRO_EVENT)
            {
                if (macroName == (string)ev.getParameter(MACRO_NAME_PARAM))
                {
                    launch = true;
                    synchronous = (bool)ev.getParameter("synchronous");
                    toFinish = ev;
                }
            }

            if (ev.Name == "sequence finished")
            {
                if (sequence == (Sequence)ev.getParameter("sequence"))
                {
                    finished = true;
                }
            }
        }


        public override void Tick()
        {
            if (launch)
            {
                GetComponent<SequenceLauncher>().Launch();

                launch = false;
            }

            if (finished)
            {
                if (toFinish != null)
                {
                    Game.main.eventFinished(toFinish);
                }
                finished = false;
            }
        }
        #endregion


        /*void Update(){

            var ge = new GameEvent("start macro");
            ge.setParameter("macro", "name");
            Game.main.enqueueEvent(ge);
        }*/
    }

}