using UnityEngine;
using System.Collections;

public class IsoSwitchesEventManager : EventManager {
	
	public override void ReceiveEvent (GameEvent ev)
	{
		if(ev.Name == "ChangeSwitch"){
			IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch((string)ev.getParameter("switch")).State = ev.getParameter("value");
		}
	}

	public override void Tick(){}
}
