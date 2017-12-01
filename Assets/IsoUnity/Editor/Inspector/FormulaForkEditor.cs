using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using NCalc;
using System.Linq;

namespace IsoUnity.Sequences {
	[CustomEditor(typeof(FormulaFork))]
	public class FormulaForkEditor : NodeContentEditor {
	    
	    private object lastValue;

	    public string Name { get; set; }

	    protected override void NodeContentInspectorGUI()
	    {
	        var f = target as FormulaFork;

	        EditorGUI.BeginChangeCheck();
	        f.Formula = EditorGUILayout.TextField(f.Formula);

	        if (!f.SequenceFormula.IsValidExpression)
	        {
	            EditorGUILayout.LabelField(f.SequenceFormula.Error);
	        } 
	    }
	}
}
