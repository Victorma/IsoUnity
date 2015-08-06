using UnityEngine;
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

	public abstract Dialog[] dialogList();
}

public class DialogManagerInstance : DialogManager {
	
	public DialogManagerInstance(){

	}

	public override Dialog[] dialogList(){
		return Resources.FindObjectsOfTypeAll(typeof(Dialog)) as Dialog[];
	}
}
