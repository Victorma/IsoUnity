using UnityEngine;
using System.Collections;

public abstract class EventManager : ScriptableObject{

	public abstract void ReceiveEvent (GameEvent ev);
	public abstract void Tick ();

}
