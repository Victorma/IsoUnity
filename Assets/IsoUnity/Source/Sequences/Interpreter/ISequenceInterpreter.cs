using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	public interface ISequenceInterpreter {

		bool CanHandle(SequenceNode node);
		bool HasFinishedInterpretation();
	    SequenceNode NextNode();
		void EventHappened(IGameEvent ge);
		void UseNode(SequenceNode node);
		void Tick();
	    ISequenceInterpreter Clone();

	}
}