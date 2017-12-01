using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	public class GameEventInterpreter : ISequenceInterpreter
	{

		private bool launched = false;
		private bool finished = false;
		private SequenceNode node;
		private bool waitTillEventFinished;

		public bool CanHandle(SequenceNode node)
		{
			return node!= null && node.Content != null && node.Content is IGameEvent;
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
			return (this.node.Childs.Length>0)?this.node.Childs[0]:null;
		}

		public void EventHappened(IGameEvent ge)
		{
			Debug.Log ("Something happened: " + ge.Name);
			if(waitTillEventFinished)
				if(ge.Name.ToLower() == "event finished" && ((IGameEvent)ge.getParameter("event")) == this.ge)
				    waitTillEventFinished = false;
		}

		private IGameEvent ge;
		public void Tick()
		{
			ge = (node.Content as IGameEvent);
			if(!launched){
				Game.main.enqueueEvent(ge);
				if(ge.getParameter("synchronous")!=null && (bool)ge.getParameter("synchronous") == true)
					waitTillEventFinished = true;
				launched = true;
			}
			if(!waitTillEventFinished)
				finished = true;
		}

		public ISequenceInterpreter Clone(){
			return new GameEventInterpreter();
		}
	}
}