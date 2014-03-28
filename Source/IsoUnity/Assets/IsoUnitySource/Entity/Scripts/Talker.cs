using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Talker : EntityScript {

	public Texture2D background;
	public string msg;
	public string displaymsg;

	public Dialog dialog;

	private bool openDialog = false;
	private bool next = false;
	private List<Item> itemsToUse = new List<Item>();

	private float delay = 0;
	private int fragment = 0;

	
	public override void eventHappened (GameEvent ge)
	{
		if(ge.getParameter("Talker") == this)
		{
			switch (ge.Name.ToLower()) 
			{
			case "open dialog": 
				openDialog = true;
				break;
			case "ended fragment": 
				next = true;
				break;
			}
		}
	}
	
	public override Option[] getOptions ()
	{
			GameEvent ge = new GameEvent ();
			ge.Name = "open dialog";
			ge.setParameter ("Talker", this);
			Option option = new Option ("Talk", ge, false); 
			return new Option[]{option};
	}
	public override void tick ()
	{
		if (openDialog) {
			Debug.Log ("Abriendo Dialoguer");
			GUIManager.addGUI (new DialogGUI (this, dialog.getFragments()[fragment]));
			this.reset();
			openDialog = false;
		}	
		if (next) {
			if(fragment<dialog.getFragments().Length-1){
				fragment++;
				GUIManager.addGUI (new DialogGUI (this, dialog.getFragments()[fragment]));
			}else{
				this.reset ();
			}
			next = false;
		}
	}
	
	public override void Update (){
		/*this.delay += Time.deltaTime;

		if (this.delay > 0.05) {
			if (word < this.msg.Length) {
				this.displaymsg = this.displaymsg + this.msg[word];
				this.word++;
			}
			this.delay = 0;
		}*/
	}
	
	public Entity getEntity(){
		return this.entity;
	}

	public string getMsg(){
		return this.displaymsg;
	}

	private void reset(){
		this.displaymsg = "";
		this.fragment = 0;
		this.delay = 0;
	}

}

