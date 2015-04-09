using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MapManager
{
	private static MapManager instance = null;
	
	public static MapManager getInstance(){
		if(instance == null)
			instance = new MapManagerInstance();
		return instance;
	}
	
	public abstract Map[] getMapList();

	public abstract void fillControllerEvent(ControllerEventArgs args);

	public abstract void setActiveMap(Map map);

	public abstract void hideAllMaps();
}



public class MapManagerInstance : MapManager 
{
	private List<Map> activeMaps;
	
	public MapManagerInstance(){
		activeMaps = new List<Map>();
	}

	private void activate(Map map)
	{
		map.setVisible(true);
	}

	private void deActivate(Map map)
	{
		map.setVisible(false);
	}

	public override void setActiveMap(Map map)
	{// TODO Multiple active maps
		if(activeMaps.Count>0){
			deActivate(activeMaps[0]);
			activeMaps.Clear();
		}
		activeMaps.Add(map);
		activate(map);
	}


	public override Map[] getMapList()
	{
		return GameObject.FindObjectsOfType<Map>();
	}
	
	public override void fillControllerEvent(ControllerEventArgs args){
		foreach(Map map in activeMaps){
			map.fillControllerEvent(args);
		}
	}

	public override void hideAllMaps(){
		foreach(Map map in GameObject.FindObjectsOfType<Map>())
			map.setVisible(false);
	}

}