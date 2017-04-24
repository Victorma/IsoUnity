using UnityEngine;
using System.Collections;
using System;

namespace IsoUnity.Sequences {
	public class SimpleContentInterpreter : ISequenceInterpreter {

	    SequenceNode node;
	    ISimpleContent content;
	    int selected = -1;

	    public bool CanHandle(SequenceNode node)
	    {
	        return node.Content is ISimpleContent;
	    }

	    public ISequenceInterpreter Clone()
	    {
	        return new SimpleContentInterpreter();
	    }

	    public void EventHappened(IGameEvent ge){}

	    public bool HasFinishedInterpretation()
	    {
	        return selected != -1;
	    }

	    public SequenceNode NextNode()
	    {
	        return node.Childs[selected];
	    }

	    public void Tick()
	    {
	        if (selected == -1)
	            selected = content.Execute();
	    }

	    public void UseNode(SequenceNode node)
	    {
	        this.node = node;
	        this.content = node.Content as ISimpleContent;
	    }
	}
}