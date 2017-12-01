using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace IsoUnity.Sequences {
	[CustomEditor(typeof(FormulaSetter))]
	public class FormulaSetterEditor : NodeContentEditor
	{
	    
	    protected override void NodeContentInspectorGUI()
	    {
	        var isoSwitches = IsoSwitchesManager.getInstance().getIsoSwitches();
	        var fs = target as FormulaSetter;
	        
	        EditorGUILayout.BeginHorizontal();
	        fs.iswitch = EditorGUILayout.TextField(fs.iswitch, GUILayout.Width(100));
	        if(GUILayout.Button("v", GUILayout.Width(15), GUILayout.Height(15)))
	        {
	            var menu = new GenericMenu();
				var switches = new List<ISwitch> ();
	            if(Sequence.current)
	                switches.AddRange (Sequence.current.LocalVariables.getList ());
				switches.AddRange (isoSwitches.getList ());

				var possibles = switches.ConvertAll (s => s.id);

	            if (!string.IsNullOrEmpty(fs.iswitch))
					possibles = switches.FindAll(s => s.id.Contains(fs.iswitch)).ConvertAll(s => s.id);

	            possibles.Sort();

	            foreach (var p in possibles)
	            {
	                menu.AddItem(new GUIContent(p), false, (n) => fs.iswitch = (string)n, p);
	            }

	            menu.ShowAsContext();
	        }

			if (!string.IsNullOrEmpty(fs.iswitch) 
				&& (Sequence.current && Sequence.current.ContainsVariable (fs.iswitch) 
					|| isoSwitches.containsSwitch(fs.iswitch)))
	        {
	            fs.Formula = EditorGUILayout.TextField(fs.Formula);
	            EditorGUILayout.EndHorizontal();
	            if (!fs.SequenceFormula.IsValidExpression)
	            {
	                EditorGUILayout.LabelField(fs.SequenceFormula.Error);
	            }
	        }
	        else
	        {
	            EditorGUILayout.LabelField("Variable is not a valid Switch");
	            EditorGUILayout.EndHorizontal();
	        }
	    }
	}
}