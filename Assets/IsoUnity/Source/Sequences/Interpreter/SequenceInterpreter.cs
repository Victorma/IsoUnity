using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	public class SequenceInterpreter  {

		private ISequenceInterpreter currentInterpreter;
		private SequenceNode currentNode;
	    private Sequence sequence;
        private bool abort = false;

		public SequenceInterpreter(Sequence sequence){
	        this.sequence = sequence;
	        currentNode = sequence.Root;
		}

		public bool SequenceFinished {
			get { return currentNode == null || currentNode.Content == null; }
		}

		public void EventHappened(IGameEvent ge){
			if(currentInterpreter!=null)
				currentInterpreter.EventHappened(ge);
		}

		public void Tick(){
			if(!SequenceFinished)
	        {
				if(currentInterpreter == null){
					currentInterpreter = SequenceInterpreterFactory.Intance.createSequenceInterpreterFor(currentNode);
					currentInterpreter.UseNode(currentNode);
				}

	            Sequence.current = sequence;
				currentInterpreter.Tick();
	            Sequence.current = null;

	            if (currentInterpreter.HasFinishedInterpretation()){
					currentNode = abort ? null : currentInterpreter.NextNode();
					if(currentInterpreter is Object)
						Object.DestroyImmediate(currentInterpreter as Object);
					currentInterpreter = null;
				}
			}
		}

        public void Abort(bool instant = false)
        {
            abort = true;
            if (instant)
            {
                currentNode = null;
                currentInterpreter = null;
            }
        }

	}
}