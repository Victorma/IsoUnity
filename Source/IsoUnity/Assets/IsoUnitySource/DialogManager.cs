using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public abstract class DialogManager {
	
	private static DialogManager instance;
	public static DialogManager getInstance(){
		if(instance == null){
			Debug.Log("creating singleton");
			instance = new DialogManagerInstance();
		}
		return instance;
	}
	
	[MenuItem("Assets/Create/Dialog")]
	public static void createDialogAsset(){
		Dialog asset = new Dialog();  //scriptable object
		ProjectWindowUtil.CreateAsset(asset, "Dialog.asset");
		Selection.activeObject = asset;    
	}

	public abstract Dialog[] dialogList();
}

public class DialogManagerInstance : DialogManager {
	
	public DialogManagerInstance(){

	}

	public override Dialog[] dialogList(){
		return Resources.FindObjectsOfTypeAll(typeof(Dialog)) as Dialog[];
	}
}
