using UnityEngine;
using UnityEditor;
using System.Collections;

namespace IsoUnity.Sequences {
[CustomEditor(typeof(ISwitchFork))]
	public class ISwitchForkEditor : NodeContentEditor {

	    protected override void NodeContentInspectorGUI()
	    {
	        var isf = target as ISwitchFork;
	        isf.id = EditorGUILayout.TextField("ID", isf.id);
			isf.comparationType = (ISwitchFork.ComparationType) EditorGUILayout.EnumPopup("Comparation Type", isf.comparationType);
			isf.Value = ParamEditor.LayoutEditorFor("Value", isf.Value, false);
	        
	        isf.name = isf.id + GetComparationString(isf.comparationType) + isf.Value;
	    }

	    private string GetComparationString(ISwitchFork.ComparationType comparationType)
	    {
	        switch (comparationType)
	        {
	            case ISwitchFork.ComparationType.Equal: return "=";
	            case ISwitchFork.ComparationType.Greather: return ">";
	            case ISwitchFork.ComparationType.Less: return "<";
	            case ISwitchFork.ComparationType.Distinct: return "!=";
	            case ISwitchFork.ComparationType.GreatherEqual: return ">=";
	            case ISwitchFork.ComparationType.LessEqual: return "<=";
	        }

	        return string.Empty;
	    }
	}
}