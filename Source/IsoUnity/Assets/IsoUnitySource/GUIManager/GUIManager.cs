using UnityEngine;
using System.Collections.Generic;

public class GUIManager {

	public static int DefaultPriority = 1;

	private static bool drawingOptions = false;
	private static bool isInTick = false;
	public static bool IsDrawingOptions{
		get{
			return drawingOptions;
		}
	}
	private static Dictionary<IsoGUI, int> priorities = new Dictionary<IsoGUI, int>();
	private static List<IsoGUI> guis = new List<IsoGUI>();
	private static List<IsoGUI> toRemove = new List<IsoGUI>();

	public static void tick(){
		isInTick = true;
		foreach(IsoGUI gui in guis)	gui.draw();
		isInTick = false;
		foreach(IsoGUI gui in toRemove) removeGUI(gui);
	}

	public static void addGUI(IsoGUI newGUI){
		addGUI(newGUI, DefaultPriority);
	}

	public static void addGUI(IsoGUI newGUI, int priority){
		guis.Add(newGUI);
		priorities.Add(newGUI,priority);
		guis.Sort(new PrioOrder(priorities));
	}

	private class PrioOrder : IComparer<IsoGUI> {

		private Dictionary<IsoGUI, int> prios;
		public PrioOrder(Dictionary<IsoGUI, int> prios){
			this.prios = prios;
		}

		public int Compare(IsoGUI i1, IsoGUI i2)
		{
			return prios[i1] < prios[i2];
		}

	}

	public static void removeGUI(IsoGUI gui){
		if(!isInGUI){
			guis.Remove(gui);
			priorities.Remove(gui);
		}else{
			toRemove.add(gui);
		}
	}

	public static void drawOptions(Vector2 position, object[] options){

		GUIManager.drawingOptions = true;
		GUIManager.position = position;
		number = options.Length;
	}

	public static void stopDrawingOptions(){
		GUIManager.drawingOptions = false;
	}
}
