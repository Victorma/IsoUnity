using UnityEngine;
using UnityEditor;
using System.Collections;
using IsoUnity.Sequences;

namespace IsoUnity.Entities
{
    [CustomEditor(typeof(Teleporter))]
    public class TeleporterEditor : Editor
    {

        void OnEnable()
        {


        }
        Editor editor;
        //int selectedTexture;
        public override void OnInspectorGUI()
        {


            serializedObject.Update();

            SerializedProperty cell = serializedObject.FindProperty("destination");
            SerializedProperty modep = serializedObject.FindProperty("mode");
            SerializedProperty gep = serializedObject.FindProperty("sge");
            SerializedProperty checkablep = serializedObject.FindProperty("checkable");

            EditorGUILayout.PropertyField(cell);

            modep.intValue = GUILayout.Toolbar(modep.intValue, new string[] { "Always", "Event trigger", "Checkable" });

            switch (modep.intValue)
            {

                case 1:
                    {
                        SerializableGameEvent ge = gep.objectReferenceValue as SerializableGameEvent;

                        if (ge == null)
                        {
                            ge = ScriptableObject.CreateInstance<SerializableGameEvent>();
                        }

                        string[] editors = EventEditorFactory.Intance.CurrentEventEditors;
                        int editorSelected = 0;
                        if (ge.Name == null)
                            ge.Name = "";
                        for (int i = 1; i < editors.Length; i++)
                            if (editors[i].ToLower() == ge.Name.ToLower())
                                editorSelected = i;
                        int was = editorSelected;

                        editorSelected = EditorGUILayout.Popup(editorSelected, EventEditorFactory.Intance.CurrentEventEditors);
                        if (was != editorSelected && editorSelected == 0)
                            ge.Name = "";
                        EventEditor editor = EventEditorFactory.Intance.createEventEditorFor(editors[editorSelected]);
                        editor.useEvent(ge);

                        editor.draw();
                        ge = editor.Result;

                        gep.objectReferenceValue = ge;

                    }
                    break;
                case 2:
                    {

                        Checkable c = (Checkable)checkablep.objectReferenceValue;
                        string[] editors = ForkEditorFactory.Intance.CurrentForkEditors;

                        EditorGUI.BeginChangeCheck();

                        int editorSelected = EditorGUILayout.Popup(
                            ForkEditorFactory.Intance.ForkEditorIndex(c),
                            ForkEditorFactory.Intance.CurrentForkEditors
                            );

                        if (EditorGUI.EndChangeCheck())
                        {
                            c = ForkEditorFactory.Intance.createForkOf(ForkEditorFactory.Intance.CurrentForkEditors[editorSelected]);
                            checkablep.objectReferenceValue = c;
                        }
                        
                        if(c != null)
                            editor = ForkEditorFactory.Intance.createForkEditorFor(c as Checkable);

                        if (editor != null)
                            editor.OnInspectorGUI();
                    }
                    break;
            }



            serializedObject.ApplyModifiedProperties();

        }


        /*
        void OnGUI(Rect position, SerializedProperty property, GUIContent label){



        }*/
    }
}