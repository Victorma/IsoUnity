using UnityEngine;
using System.Collections;

public interface EventEditor  {

	void draw();
	SerializableGameEvent Result { get; }
	string EventName{ get; }
	EventEditor clone();
	void useEvent(SerializableGameEvent ge);
	void detachEvent(SerializableGameEvent ge);
}
