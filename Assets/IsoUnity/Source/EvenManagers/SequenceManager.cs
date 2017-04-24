using UnityEngine;
using System.Collections;


namespace IsoUnity.Sequences {
	public class SequenceManager : EventManager {
		
        public Sequence Executing { get; private set; }
		private SequenceInterpreter sequenceInterpreter;

		public override void ReceiveEvent (IGameEvent ev)
        {
            if (sequenceInterpreter == null){
				if(ev.Name.ToLower() == "start sequence"){
					Sequence sequence = (ev.getParameter("Sequence") as Sequence);
                    Executing = sequence;
					sequenceInterpreter = new SequenceInterpreter(sequence);
				}
			}else sequenceInterpreter.EventHappened(ev);
		}

		public override void Tick(){
			if(sequenceInterpreter != null){
	            sequenceInterpreter.Tick();
				if(sequenceInterpreter.SequenceFinished){
					Debug.Log("Sequence finished");
					this.sequenceInterpreter = null;
                    Executing = null;
				}
			}
		}
	}
}
