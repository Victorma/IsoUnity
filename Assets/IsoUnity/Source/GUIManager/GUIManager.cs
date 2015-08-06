using UnityEngine;
using System.Collections.Generic;

public class GUIManager {

	public static int DefaultPriority = 1;
	private static bool isInTick = false;

	private static Dictionary<IsoGUI, int> priorities = new Dictionary<IsoGUI, int>();
	private static List<IsoGUI> guis = new List<IsoGUI>();

	private static List<IsoGUI> toRemove = new List<IsoGUI>();
	private static List<IsoGUI> toAdd = new List<IsoGUI>();

	public static void tick(){
		//GUISkin skin = GUI.skin;
		isInTick = true;
		int depth = GUI.depth;
		GUI.depth = guis.Count;

		guis.Sort(new InversePrioOrder(priorities));
		foreach(IsoGUI gui in guis){
			gui.draw();
			GUI.depth = GUI.depth-1;
		}
		GUI.depth = depth;
		guis.Sort(new PrioOrder(priorities));
		isInTick = false;
		foreach(IsoGUI gui in toRemove) removeGUI(gui);
		toRemove.Clear ();
		foreach(IsoGUI gui in toAdd) addGUI(gui);
		toAdd.Clear ();
		//GUI.skin = skin;
	}

	public static void addGUI(IsoGUI newGUI){
		addGUI(newGUI, DefaultPriority);
	}

	public static void addGUI(IsoGUI newGUI, int priority){
		if(!priorities.ContainsKey(newGUI))
			priorities.Add(newGUI,priority);
		if(!isInTick){
			guis.Add(newGUI);
			guis.Sort(new PrioOrder(priorities));
		}else{
			toAdd.Add(newGUI);
		}


	}

	private class PrioOrder : IComparer<IsoGUI> {

		private Dictionary<IsoGUI, int> prios;
		public PrioOrder(Dictionary<IsoGUI, int> prios){
			this.prios = prios;
		}

		public int Compare(IsoGUI i1, IsoGUI i2)
		{
			return prios[i2] - prios[i1];
		}

	}

	private class InversePrioOrder : IComparer<IsoGUI> {
		
		private Dictionary<IsoGUI, int> prios;
		public InversePrioOrder(Dictionary<IsoGUI, int> prios){
			this.prios = prios;
		}
		
		public int Compare(IsoGUI i1, IsoGUI i2)
		{
			return prios[i1] - prios[i2];
		}
		
	}

	public static void removeGUI(IsoGUI gui){
		if(!isInTick){
			guis.Remove(gui);
			priorities.Remove(gui);
		}else{
			toRemove.Add(gui);
		}
	}

	public static IsoGUI getGUICapturing(ControllerEventArgs args){
		isInTick = true;
		foreach(IsoGUI gui in guis){
			if(gui.captureEvent(args))
				return gui;
		}
		isInTick = false;
		// Normally this things should not happen, but for prevent...
		foreach(IsoGUI gui in toRemove) removeGUI(gui);
		toRemove.Clear ();
		foreach(IsoGUI gui in toAdd) addGUI(gui);
		toAdd.Clear ();
		return null;
	}
	
}
