using UnityEngine;
using System.Collections.Generic;

public class DialogInterpreter : ScriptableObject, ISecuenceInterpreter {

	private bool launched = false;
	private bool finished = false;
	private SecuenceNode node;
	private SecuenceNode nextNode;
	private GameObject wasLooking;
	private Queue<Dialog.Fragment> fragments;
	private int chosen;
	private bool next;

	
	public bool CanHandle(SecuenceNode node)
	{
		return node!= null && node.Content != null && node.Content is Dialog;
	}
	
	public void UseNode(SecuenceNode node){
		this.node = node;
	}
	
	public bool HasFinishedInterpretation()
	{
		return finished;
	}
	
	public SecuenceNode NextNode()
	{
		return nextNode;
	}
	
	public void EventHappened(GameEvent ge)
	{
		if(ge.getParameter("Launcher") == this)
		{
			switch (ge.Name.ToLower()) 
			{
			case "ended fragment": 
				next = true;
				break;
				
			case "chosen option": 
				chosen = (int)ge.getParameter("option");
				break;
			}
		}
	}
	
	public void Tick()
	{
		Dialog dialog = node.Content as Dialog;

		if(!launched){
			wasLooking = CameraManager.Target;
			fragments = new Queue<Dialog.Fragment>(dialog.getFragments());
			launched = true;
			next = true;
			chosen = -1;
		}

		if(next){
			if(fragments.Count > 0){
				if(fragments.Peek().Entity != null)
					CameraManager.smoothLookTo(fragments.Peek().Entity.gameObject);
				DialogGUI gui = ScriptableObject.CreateInstance<DialogGUI>();
				gui.init(this, fragments.Dequeue());
				GUIManager.addGUI (gui);
			}else{
				if(dialog.getOptions() != null && dialog.getOptions().Length>1){
					DialogGUI gui = ScriptableObject.CreateInstance<DialogGUI>();
					gui.init(this, dialog.getOptions());
					GUIManager.addGUI (gui);
				}
				else chosen = 0;
			}
			next = false;
		}

		if(chosen != -1){
			finished = true;
             CameraManager.lookTo(wasLooking);
            if (node.Childs.Length > chosen)
                nextNode = node.Childs[chosen];
			chosen = -1;
		}
	}

	public ISecuenceInterpreter Clone(){
		return ScriptableObject.CreateInstance<DialogInterpreter>();
	}
}
