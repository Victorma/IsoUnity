using UnityEngine;
using System.Collections;

public class CheckableInterpreter : ISecuenceInterpreter {

	private bool finished = false;
	private SecuenceNode node;
	private bool evalResult;
	
	public bool CanHandle(SecuenceNode node)
	{
		return node!= null && node.Content != null && node.Content is Checkable;
	}
	
	public void UseNode(SecuenceNode node){
		this.node = node;
	}
	
	public bool HasFinishedInterpretation()
	{
		return finished;
	}
	
	public SecuenceNode NextNode()
	{
		return (evalResult)?this.node.Childs[0]:this.node.Childs[1];
	}
	
	public void Tick()
	{
		evalResult = (node.Content as Checkable).check();
		finished = true;
	}

	public ISecuenceInterpreter Clone(){
		return new CheckableInterpreter();
	}
		
	public void EventHappened(GameEvent ge){}
}
