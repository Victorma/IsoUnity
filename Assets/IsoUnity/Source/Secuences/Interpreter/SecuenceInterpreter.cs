using UnityEngine;
using System.Collections;

public class SecuenceInterpreter  {

	private ISecuenceInterpreter currentInterpreter;
	private SecuenceNode currentNode;

	public SecuenceInterpreter(Secuence secuence){
		currentNode = secuence.Root;
	}

	public bool SecuenceFinished {
		get { return currentNode == null || currentNode.Content == null; }
	}

	public void EventHappened(GameEvent ge){
		if(currentInterpreter!=null)
			currentInterpreter.EventHappened(ge);
	}

	public void Tick(){
		if(!SecuenceFinished){
			if(currentInterpreter == null){
				currentInterpreter = SecuenceInterpreterFactory.Intance.createSecuenceInterpreterFor(currentNode);
				currentInterpreter.UseNode(currentNode);
			}

			currentInterpreter.Tick();

			if(currentInterpreter.HasFinishedInterpretation()){
				Debug.Log ("Finished interpretation");
				currentNode = currentInterpreter.NextNode();
				if(currentInterpreter is Object)
					Object.DestroyImmediate(currentInterpreter as Object);
				currentInterpreter = null;
			}
		}
	}

}
