using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	public class CheckableInterpreter : ISequenceInterpreter
	{

		private bool finished = false;
		private SequenceNode node;
		private bool evalResult;
		
		public bool CanHandle(SequenceNode node)
		{
			return node!= null && node.Content != null && node.Content is Checkable;
		}
		
		public void UseNode(SequenceNode node){
			this.node = node;
		}
		
		public bool HasFinishedInterpretation()
		{
			return finished;
		}
		
		public SequenceNode NextNode()
		{
			return (evalResult)?this.node.Childs[0]:this.node.Childs[1];
		}
		
		public void Tick()
		{
			evalResult = (node.Content as Checkable).check();
			finished = true;
		}

		public ISequenceInterpreter Clone(){
			return new CheckableInterpreter();
		}
			
		public void EventHappened(IGameEvent ge){}
	}
}