using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class IsoSettingsManager
{
	private static IsoSettingsManagerInstance instance;

	public static IsoSettingsManager getInstance(){
		if(instance == null)
			instance = new IsoSettingsManagerInstance();
		return instance;
	}

	public abstract IsoSettings getIsoSettings();
}

public class IsoSettingsManagerInstance : IsoSettingsManager
{
	private String ruta;

	public IsoSettingsManagerInstance(){
		ruta = "Assets/Resources/IsoSettings.asset";
	}


	public override IsoSettings getIsoSettings(){

		IsoSettings isoSettings = Resources.Load<IsoSettings> ("IsoSettings");

		/*if (isoSettings == null) {
			isoSettings = new IsoSettings();  //scriptable object
			AssetDatabase.CreateAsset(isoSettings, ruta);
			Selection.activeObject = isoSettings;  
		}*/

		return isoSettings;
	}
}
