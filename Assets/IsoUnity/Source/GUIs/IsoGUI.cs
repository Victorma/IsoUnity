using UnityEngine;

public abstract class IsoGUI : ScriptableObject
{
	public IsoGUI (){

	}

	public virtual bool captureEvent(ControllerEventArgs args){
		return true;
	}

	public abstract void draw ();
	public abstract void fillControllerEvent(ControllerEventArgs args);
}


