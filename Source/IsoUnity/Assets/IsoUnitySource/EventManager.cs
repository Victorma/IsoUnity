using UnityEngine;
using System.Collections;

public interface EventManager {

	void ReceiveEvent (GameEvent ev);
	void Tick ();

}
