using UnityEngine;
using System.Collections;
using UnityEditor;

namespace IsoUnity.Sequences {
	[CustomEditor(typeof(SequenceAsset))]
	public class SequenceEditor : Editor {

	    SequenceWindow editor = null;
	    public override void OnInspectorGUI()
	    {

	        if (GUILayout.Button("Open editor"))
	        {
	            if (editor == null)
	            {
	                editor = EditorWindow.GetWindow<SequenceWindow>();
	                editor.Sequence = (target as Sequence);
	            }
	        }
	        if (GUILayout.Button("Close editor"))
	        {
	            if (editor != null)
	            {
	                editor.Close();

	                AssetDatabase.SaveAssets();
	            }
	        }

	        DrawDefaultInspector();

	    }
	}
}