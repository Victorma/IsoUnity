using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System;

namespace IsoUnity.Sequences {
	public class DialogNodeEditor : NodeEditor {

		private SequenceNode myNode;
	    private Editor editor;

	    public void draw(){

	        editor.OnInspectorGUI();
			
			/*if (Event.current.type != EventType.layout)
	        {
	            myNode.ChildSlots = 1; Mathf.Max(1, dialog.Options.Count);
	            var min = Mathf.Min(myNode.ChildSlots, dialog.Options.Count);
	            for (int i = 0; i < min; i++)
	            {
	                if (myNode.Childs[i] != null)
	                    myNode.Childs[i].Name = dialog.Options[i].Text;
	            }
	        }*/
	    }
		
		public SequenceNode Result { get{ return myNode; } }
		public string NodeName{ get { return "Dialog"; } }

	    public string[] ChildNames
	    {
	        get
	        {//dialog.Options.ConvertAll(o => o.Text).ToArray()
	            return new string[] { "end dialog" };
	        }
	    }

	    public NodeEditor clone(){ return new DialogNodeEditor(); }
		
		public bool manages(SequenceNode c) { return c.Content != null && c.Content is Dialog; }
		public void useNode(SequenceNode c) {
			if(c.Content == null || !(c.Content is Dialog))
				c.Content = ScriptableObject.CreateInstance<Dialog>();

			myNode = c;
	        editor = Editor.CreateEditor(c.Content as Dialog) as DialogEditor;

	        // This could be used aswell, but I only advise this your class inherrits from UnityEngine.Object or has a CustomPropertyDrawer
	        // Since you'll find your item using: serializedObject.FindProperty("list").GetArrayElementAtIndex(index).objectReferenceValue
	        // which is a UnityEngine.Object
	        // reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);
		}
	}
}