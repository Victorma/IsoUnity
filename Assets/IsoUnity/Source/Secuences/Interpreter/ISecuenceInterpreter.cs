using UnityEngine;
using System.Collections;

public interface ISecuenceInterpreter {

	bool CanHandle(SecuenceNode node);
	bool HasFinishedInterpretation();
	SecuenceNode NextNode();
	void EventHappened(GameEvent ge);
	void UseNode(SecuenceNode node);
	void Tick();
	ISecuenceInterpreter Clone();

}
