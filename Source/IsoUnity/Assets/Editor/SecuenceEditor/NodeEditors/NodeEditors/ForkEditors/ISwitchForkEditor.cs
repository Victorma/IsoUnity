using UnityEngine;
using UnityEditor;
using System.Collections;

public class ISwitchForkEditor : ForkEditor {

	private ISwitchFork isf;

	public ISwitchForkEditor(){
		isf = ScriptableObject.CreateInstance<ISwitchFork>();
	}

	public Checkable Result { 
		get{ return isf;} 
	}

	public string ForkName{ 
		get{ return "Iso Switch Fork"; } 
	}

	public bool manages(Checkable c){
		return c!=null && c is ISwitchFork;
	}

	public ForkEditor clone(){
		return new ISwitchForkEditor();
	}

	public void useFork(Checkable c){
		if(c is ISwitchFork)
			isf = c as ISwitchFork;
	}

	public void draw(){
		isf.id = EditorGUILayout.TextField("ID", isf.id);
		isf.comparationType = (ISwitchFork.ComparationType) EditorGUILayout.EnumPopup("Comparation Type", isf.comparationType);
		isf.Value = ParamEditor.editorFor("Value", isf.Value, false);
	}
}
