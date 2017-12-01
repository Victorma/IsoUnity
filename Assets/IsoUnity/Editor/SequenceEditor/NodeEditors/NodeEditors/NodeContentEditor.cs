using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IsoUnity.Sequences {
	public abstract class NodeContentEditor : Editor {

	    private Sequence parent;
	    private bool sequenceSetted;

	    protected virtual void OnEnable()
	    {
	        if (!parent)
	            parent = SequenceAsset.FindSequenceOf(target);
	    }

	    public override void OnInspectorGUI()
	    {
	        sequenceSetted = false;
	        if (Sequence.current == null)
	        {
	            Sequence.current = parent;
	            sequenceSetted = true;
	        }

	        NodeContentInspectorGUI();

	        if (sequenceSetted)
	            Sequence.current = null;
	    }

	    protected abstract void NodeContentInspectorGUI();

	}
}