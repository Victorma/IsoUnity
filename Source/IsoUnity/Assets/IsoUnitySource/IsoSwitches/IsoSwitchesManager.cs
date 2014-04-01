using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class IsoSwitchesManager
{
	private static IsoSwitchesManagerInstance instance;
	
	public static IsoSwitchesManager getInstance(){
		if(instance == null)
			instance = new IsoSwitchesManagerInstance();
		return instance;
	}
	
	public abstract IsoSwitches getIsoSwitches();
}

public class IsoSwitchesManagerInstance : IsoSwitchesManager
{
	private String ruta;
	
	public IsoSwitchesManagerInstance(){
		ruta = "Assets/Resources/IsoSwitches.asset";
	}
	
	
	public override IsoSwitches getIsoSwitches(){
		
		IsoSwitches isoSwitches = Resources.Load<IsoSwitches> ("IsoSwitches");
		
		if (isoSwitches == null) {
			isoSwitches = new IsoSwitches();  //scriptable object
			AssetDatabase.CreateAsset(isoSwitches, ruta);
			Selection.activeObject = isoSwitches;  
		}
		
		return isoSwitches;
	}
}