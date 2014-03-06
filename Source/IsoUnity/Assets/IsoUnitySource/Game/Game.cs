using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	Queue<GameEvent> events;
	//Queue<Command> commands;
	public GameObject look;

	public static Game main;

	// Use this for initialization
	void Start () {
		main  = this;
		events = new Queue<GameEvent>();
		//commands = new Queue<Command>();
		CameraManager.initialize();
		ControllerManager.Enabled = true;
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

	/*public void enqueueCommand(Command c){
		this.commands.Enqueue(c);
	}*/

	private float timeToController = 100/1000;
	private float currentTimeToController = 0;

	public void tick(){

		CameraManager.lookTo(look);

		currentTimeToController+=Time.deltaTime;
		if(currentTimeToController > timeToController){
			ControllerManager.tick();
			currentTimeToController-=timeToController;
		}
		while(events.Count>0)
		{
			GameEvent ge = events.Dequeue();
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
