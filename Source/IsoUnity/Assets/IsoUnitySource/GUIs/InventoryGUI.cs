using UnityEngine;
using System.Collections;

public class InventoryGUI : IsoGUI {

	private Inventory inventory;
	public InventoryGUI(Inventory inventory){
		this.inventory = inventory;
	}

	public override void draw ()
	{
		GUI.Box (new Rect (0, 0, Screen.width, Screen.height),"Inventory");
		GUIStyle style = new GUIStyle ();

		style.padding = new RectOffset(15,15,15,15);
		GUI.BeginGroup (new Rect (0, 0, Screen.width, 150));
			GUI.Label ( new Rect(0,0, Screen.width, 150),new GUIContent ("Inventory"));
		GUI.EndGroup ();
		if (GUI.Button (new Rect (0, Screen.height - 100, Screen.width, 100), new GUIContent ("Close")))
			GUIManager.removeGUI (this);
	}

	public override void fillControllerEvent (ControllerEventArgs args)
	{

	}

}
