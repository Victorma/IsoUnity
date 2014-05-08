using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	Queue<GameEvent> events;
	//Queue<Command> commands;
	public GameObject look;
	public Map map;

	public static Game main;

	// Use this for initialization
	void Start () {
		main  = this;
		events = new Queue<GameEvent>();
		//commands = new Queue<Command>();
		CameraManager.initialize();
		CameraManager.lookTo (look);
		MapManager.getInstance().setActiveMap(map);
		ControllerManager.Enabled = true;
		IsoSwitchesManager.getInstance ().getIsoSwitches ();
	}
	
	// Update is called once per frame
	void Update () {
		this.tick();
	}

	void OnGUI(){
		GUIManager.tick();
	}

	public void enqueueEvent(GameEvent ge){
		this.events.Enqueue(ge);
	}

	public void eventFinished(GameEvent ge){
		object sync = ge.getParameter("synchronous");
		if(sync!=null && ((bool)sync)){
			GameEvent f = ScriptableObject.CreateInstance<GameEvent>();
			f.Name = "Event Finished";
			f.setParameter("event", ge);
			this.enqueueEvent(f);
		}
	}

	/*public void enqueueCommand(Command c){
		this.commands.Enqueue(c);
	}*/

	private float timeToController = 100/1000;
	private float currentTimeToController = 0;

	public void tick(){

		CameraManager.Update();

		currentTimeToController+=Time.deltaTime;
		if(currentTimeToController > timeToController){
			ControllerManager.tick();
			currentTimeToController-=timeToController;
		}
		while(events.Count>0)
		{
			GameEvent ge = events.Dequeue();
			if(ge.Name == "ChangeSwitch"){
				Debug.Log ("Seteando: " + ge.getParameter("switch") + " con " + ge.getParameter("value"));
				IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch((string)ge.getParameter("switch")).State = ge.getParameter("value");
			}
			broadcastEvent(ge);
		}

		foreach(Map map in MapManager.getInstance().getMapList())
		{
			map.tick();
		}
	}

	private void broadcastEvent(GameEvent ge){
		foreach(Map map in MapManager.getInstance().getMapList())
		{
			map.broadcastEvent(ge);
		}
	}
}
