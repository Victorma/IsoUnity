using IsoUnity.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsoUnity.Sequences {
    [RequireComponent(typeof(SequenceLauncher))]
    public class MacroSequence : EventedEventManager {

        private const string MACRO_EVENT = "start macro";

        public Sequence Sequence { get { return sequence; } }

        [SerializeField]
        private string macroName;
        [SerializeField]
        private Sequence sequence;

        private SequenceLauncher sl;

        public void Init(){
            Awake();
        }

        void Awake(){
            if(sequence == null)
                sequence = ScriptableObject.CreateInstance<Sequence>();

            sl = GetComponent<SequenceLauncher>();
            sl.Sequence = this.Sequence;
        }

        private IGameEvent toFinish = null;

        #region implemented abstract members of EventManager

        [GameEvent(false, false)]
        public IEnumerator StartMacro(string macro)
        {
            var ge = Current;
            // If the macro name matches with me i should finish the event
            if(macroName == macro)
            {
                sl.Launch();
                yield return new WaitWhile(() => sl.Running);

                Game.main.eventFinished(ge);
            }
        }

        [GameEvent(false, false)]
        public IEnumerator StopMacro(string macro, bool instant = false)
        {
            var ge = Current;
            // If the macro name matches with me i should finish the event
            if (macroName == macro)
            {
                sl.Abort(instant);
                yield return new WaitWhile(() => sl.Running);

                Game.main.eventFinished(ge);
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