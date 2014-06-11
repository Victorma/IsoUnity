using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Talker : EntityScript {

	private bool start = false;

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
	
	public override void eventHappened (GameEvent ge)
	{
		if(ge.getParameter("Talker") == this){
			switch (ge.Name.ToLower()) {
			case "talk": 
				start = true;
				break;
			}
		}
	}
	
	public override Option[] getOptions ()
	{
		GameEvent ge = ScriptableObject.CreateInstance<GameEvent> ();
		ge.Name = "talk";
		ge.setParameter ("Talker", this);
		Option option = new Option ("Talk", ge, true, 1); 
		return new Option[]{option};
	}
	
	public override void tick ()
	{
		if (start){
			GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
			ge.Name = "start secuence";
			ge.setParameter("secuence", secuence);
			Game.main.enqueueEvent(ge);
			start = false;
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

