using UnityEngine;
using System.Collections;

public class Pepito : EntityScript {

	public override void eventHappened (GameEvent ge)
	{
		/*if(ge!=null)
			if(ge.Name == "Talked"){
				EntityScript en = ge.Args[0] as EntityScript;
				if(en is Player){
					Debug.Log("Hola don Jose!");
				}else{
					Debug.Log("No te conozco");
				}
		}else if(ge.Name=="Player moved"){
			Debug.Log("Hey a donde vas!");

		}*/
	}

	public override Option[] getOptions ()
	{
		throw new System.NotImplementedException ();
	}

	public override void tick(){

	}

	public override void Update(){

	}
}
