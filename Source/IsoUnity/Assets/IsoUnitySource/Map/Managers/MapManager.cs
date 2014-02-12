using UnityEngine;
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
	
	public abstract Map createNewMap();
	
	public abstract void removeMap(Map map);

	public abstract void setActiveMap(Map map);
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
	
	
	public override Map createNewMap()
	{
		/*Map newMap = new Map();
		mapas.Add (newMap);*/
		
		return null;
		
	}
	
	public override void removeMap(Map map)
	{
		//mapas.Remove(map);
	}
	
}