using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Talker : EntityScript {

	public Texture2D background;
	public string msg;
	public string displaymsg;
	public int chosenOption = -1;
	

	void OnEnable (){
		if (secuence == null) {
			Debug.Log ("Secuence created");
			secuence = ScriptableObject.CreateInstance<Secuence> ();
			secuence.init ();
			DontDestroyOnLoad(secuence);
		}	
	}

	[SerializeField]
	private Secuence secuence;
	public Secuence Secuence{
		get{ return secuence; }
		set{ secuence = value; }
	}

	private bool started = false;
	private bool start = false;
	private bool next = false;
	private bool chosen = false;
	private List<Item> itemsToUse = new List<Item>();

	
	public override void eventHappened (GameEvent ge)
	{
		if(ge.getParameter("Talker") == this)
		{
			switch (ge.Name.ToLower()) 
			{
			case "talk": 
				if(!started)
					start = true;
				break;
			case "ended fragment": 
				next = true;
				break;
			
			case "chosen option": 
				chosen = true;
				break;
			}
		}
	}
	
	public override Option[] getOptions ()
	{
			GameEvent ge = ScriptableObject.CreateInstance<GameEvent> ();
			ge.Name = "talk";
			ge.setParameter ("Talker", this);
			Option option = new Option ("Talk", ge, false); 
			return new Option[]{option};
	}

	private SecuenceNode currentNode;
	private Queue<Dialog.Fragment> fragments;
	public override void tick ()
	{
		if (start){
			start = false; started = true;
			currentNode = secuence.Root;
			next = true;
			Debug.Log ("Iniciando secuencia");
		}	
		if (started && next) {
			next = false;
			if(currentNode == null || currentNode.Content == null){
				started = false;
			}else if(currentNode.Content is GameEvent){
				Game.main.enqueueEvent((GameEvent)currentNode.Content);
				currentNode = currentNode.Childs[0];
				next = true;
			}else if(currentNode.Content is Dialog){
				Dialog dialog = currentNode.Content as Dialog;
				if(fragments == null){
					fragments = new Queue<Dialog.Fragment>(dialog.getFragments());
				}
				if(fragments.Count > 0){
					GUIManager.addGUI (new DialogGUI (this, fragments.Dequeue()));
				}else{
					if(dialog.getOptions() != null && dialog.getOptions().Length>1){
						GUIManager.addGUI (new DialogGUI (this, dialog.getOptions()));
					}else{
						fragments = null;
						next = true;
						currentNode = currentNode.Childs[0];
					}
				}
			}else if(currentNode.Content is Checkable){
				fragments = null;
				next = true;
				if(((Checkable) currentNode.Content).check()){
					currentNode = currentNode.Childs[0];
				}else{
					currentNode = currentNode.Childs[1];
				}
			}
		}
		if (started && chosen) {
			chosen = false;
			fragments = null;
			next = true;
			currentNode = currentNode.Childs[this.chosenOption];
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
}

