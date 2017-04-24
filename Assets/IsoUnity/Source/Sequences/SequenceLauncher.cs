using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	public class SequenceLauncher : EventManager {

		[SerializeField]
		public bool executeOnStart;

		[SerializeField]
		public bool loop;

		[SerializeField]
		public bool localExecution;

		[SerializeField]
		public bool launchOnTriggerEnter;

		[SerializeField]
		public bool launchOnMouseUpAsButton;

	    [SerializeField]
	    private Sequence sharedSequence;

	    [SerializeField]
		[HideInInspector]
	    private Sequence localSequence;

	    public Sequence Sequence {
	        get
	        {
	            return localSequence;
	        }
	        set
	        {
	            sharedSequence = value;
				localSequence = sharedSequence == null ? null : sharedSequence.Clone();
	        }
	    }

		void OnTriggerEnter(){
			if (launchOnTriggerEnter) {
				Launch ();
			}
		}

		void OnMouseUpAsButton(){
			if (localExecution) {
				Launch ();
			}
		}

		void Start(){
			this.Sequence = sharedSequence;

			if (executeOnStart) {
				Launch ();
			}
		}


		private SequenceInterpreter interpreter;
		void Update(){
			if (localExecution && interpreter != null) {
				interpreter.Tick ();
				if (interpreter.SequenceFinished) {
					interpreter = null;
					if (loop) {
						Launch ();
					}
				}
			}
		}


		private void Launch(){
			if (interpreter != null || localSequence == null)
				return;

			if (localExecution) {
				if (interpreter == null)
					interpreter = new SequenceInterpreter (localSequence);
			} else {

				// Remote start
				ge = new GameEvent ("start sequence", new System.Collections.Generic.Dictionary<string, object> () {
					{ "sequence", localSequence }
				});
				Game.main.enqueueEvent (ge);
			}
		}

		IGameEvent ge;
		public override void ReceiveEvent (IGameEvent ev)
		{
			if (interpreter != null)
				interpreter.EventHappened (ev);

			if (ev.Name == "event finished" && ev.getParameter ("event") == ge) {
				
				if (loop && !localExecution) {
					ge = null;
					Launch ();
				}
			}
		}

		public override void Tick (){}
	}
}