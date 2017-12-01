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

        private bool abort = false;

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

        public bool Running
        {
            get
            {
                return interpreter != null || ge != null;
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
					if (loop && !abort) {
						Launch ();
					}
				}
			}
		}


		public void Launch(){
			if (interpreter != null || localSequence == null)
				return;

			if (localExecution) {
                abort = false;
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
        public void Abort(bool instant)
        {
            if (localExecution)
            {
                abort = true;
                interpreter.Abort(instant);
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