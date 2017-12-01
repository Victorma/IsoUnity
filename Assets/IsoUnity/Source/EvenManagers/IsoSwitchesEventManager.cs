using UnityEngine;
using System.Collections;
using IsoUnity.Sequences;

namespace IsoUnity {
	public class IsoSwitchesEventManager : EventManager {
		
		public override void ReceiveEvent (IGameEvent ev)
		{
			if(ev.Name == "ChangeSwitch"){
				
				object p = ev.getParameter("value");
				string iswitch = (string)ev.getParameter("switch");

				// When there is a sequence we try to save it as local var but if not, we save it as global
				if (Sequence.current != null 
					&& (Sequence.current.ContainsVariable (iswitch) || !IsoSwitchesManager.getInstance ().getIsoSwitches ().containsSwitch (iswitch))) {
					// Save as local
					Sequence.current.SetVariable (iswitch, p);
				} else {
					// Save as global
					IsoSwitchesManager.getInstance ().getIsoSwitches ().getSwitch (iswitch).State = p;
				}
			}
		}

		public override void Tick(){}
	}
}