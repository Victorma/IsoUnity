using UnityEngine;
using UnityEditor;
using System.Collections;

namespace IsoUnity.Sequences
{
    [CustomEditor(typeof(MacroSequence))]
    public class MacroEditor : Editor
    {

        bool inited = false;

        void OnEnable()
        {
        }

        SequenceWindow editor = null;
        public override void OnInspectorGUI()
        {
            if (!inited)
            {
                (target as MacroSequence).Init();
                inited = true;
            }

        	this.DrawDefaultInspector();
            if (GUILayout.Button("Open editor"))
            {
                if (editor == null)
                {
                    editor = EditorWindow.GetWindow<SequenceWindow>();

                    editor.Sequence = (target as MacroSequence).Sequence;
                }
            }
            if (GUILayout.Button("Close editor"))
            {
                if (editor != null)
                    editor.Close();
            }

        }


    }
}