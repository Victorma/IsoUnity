using UnityEngine;
using System.Collections;

public class SecuenceManager : EventManager {
	
	private ControllerManager.ControllerDelegate de;
	private SecuenceInterpreter secuenceInterpreter;

	public override void ReceiveEvent (GameEvent ev)
	{
		if(secuenceInterpreter == null){
			if(ev.Name.ToLower() == "start secuence"){
				Secuence secuence = (ev.getParameter("Secuence") as Secuence);
				secuenceInterpreter = new SecuenceInterpreter(secuence);
				de = ControllerManager.onControllerEvent;
				ControllerManager.onControllerEvent = null;
				ControllerManager.onControllerEvent += this.onControllerEvent;
			}
		}else secuenceInterpreter.EventHappened(ev);
	}

	public override void Tick(){
		if(secuenceInterpreter != null){
			secuenceInterpreter.Tick();
			if(secuenceInterpreter.SecuenceFinished){
				Debug.Log("Secuence finished");
				this.secuenceInterpreter = null;
				ControllerManager.onControllerEvent = de;
			}
		}
	}

	private void onControllerEvent(ControllerEventArgs args){

	}
}
