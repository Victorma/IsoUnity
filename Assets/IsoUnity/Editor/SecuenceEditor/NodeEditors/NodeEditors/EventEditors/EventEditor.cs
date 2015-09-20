using UnityEngine;
using System.Collections;

public interface EventEditor  {

	void draw();
	GameEvent Result { get; }
	string EventName{ get; }
	EventEditor clone();
	void useEvent(GameEvent ge);
    void detachEvent(GameEvent ge);
}
