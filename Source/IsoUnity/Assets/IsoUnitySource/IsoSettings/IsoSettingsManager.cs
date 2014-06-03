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
	private IsoSettings instance;

	public IsoSettingsManagerInstance(){
		ruta = "Assets/Resources/IsoSettings.asset";
	}


	public override IsoSettings getIsoSettings(){

		instance = Resources.Load<IsoSettings> ("IsoSettings");

		if (instance == null) {
			if (Application.isEditor) {
				System.Reflection.MethodInfo mi = Type.GetType ("IsoAssetsManager")
										.GetMethod ("CreateAssetOf");
				instance = (IsoSettings)mi.Invoke (null, new object[]{"IsoSettings", ruta});
			} else {
				instance = ScriptableObject.CreateInstance<IsoSettings> ();
			}

		}

		return instance;
	}
}
