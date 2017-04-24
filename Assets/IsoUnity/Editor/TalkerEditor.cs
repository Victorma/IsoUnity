using UnityEngine;
using UnityEditor;
using System.Collections;
using IsoUnity.Sequences;

namespace IsoUnity.Entities
{
    [CustomEditor(typeof(Talker))]
    public class TalkerEditor : Editor
    {

        void OnEnable()
        {
        }

        SequenceWindow editor = null;
        public override void OnInspectorGUI()
        {

            if (GUILayout.Button("Open editor"))
            {
                if (editor == null)
                {
                    editor = EditorWindow.GetWindow<SequenceWindow>();
                    editor.Sequence = (target as Talker).Sequence;
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