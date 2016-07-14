using UnityEngine;
using System.Collections;

public abstract class EventManager : MonoBehaviour{

	void OnEnable(){
		Game.main.RegisterEventManager (this);
	}

	void OnDisable(){
		Game.main.DeRegisterEventManager (this);
	}

	public abstract void ReceiveEvent (IGameEvent ev);
	public abstract void Tick ();

}
