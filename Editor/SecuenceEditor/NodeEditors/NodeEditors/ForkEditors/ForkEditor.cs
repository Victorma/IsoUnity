using UnityEngine;
using System.Collections;

public interface ForkEditor  {

	void draw();
	Checkable Result { get; }
	string ForkName{ get; }
	ForkEditor clone();
	bool manages(Checkable c);
	void useFork(Checkable c);
}
