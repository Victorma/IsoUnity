using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IsoUnity.Sequences {
	public class SequenceManager : EventManager {
		
        public Sequence Executing { get; private set; }
		private List<SequenceInterpreter> sequenceInterpreter = new List<SequenceInterpreter>();
        private Dictionary<SequenceInterpreter, IGameEvent> toFinish = new Dictionary<SequenceInterpreter, IGameEvent>();

		public override void ReceiveEvent (IGameEvent ev)
        {
            if (ev.Name.ToLower() == "start sequence")
            {
                Sequence sequence = (ev.getParameter("Sequence") as Sequence);
                Executing = sequence;
                var i = new SequenceInterpreter(sequence);
                sequenceInterpreter.Add(i);
                toFinish.Add(i, ev);
            }

            sequenceInterpreter.ForEach(si => si.EventHappened(ev));
        }

		public override void Tick(){
            var toRemove = new List<SequenceInterpreter>();
            foreach(var si in sequenceInterpreter)
            {
                si.Tick();
                if (si.SequenceFinished)
                {
                    Debug.Log("Sequence finished");
                    toRemove.Add(si);
                    Game.main.eventFinished(toFinish[si]);
                    toFinish.Remove(si);
                    Executing = null;
                }
            }
            foreach (var si in toRemove)
                sequenceInterpreter.Remove(si);
	            
		}
	}
}
