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

	}

    public Rect windowRect = new Rect(20, 20, 120, 50);
    /*void OnGUI()
    {
        
    }
    void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
            print("Got a click");

    }*/

	public override IsoSettings getIsoSettings(){

		if (instance == null) {
			instance = Resources.Load<IsoSettings> ("IsoSettings");

			if (instance == null) {
				if (Application.isEditor) {
					#if UNITY_EDITOR
					System.Reflection.MethodInfo mi = Type.GetType ("IsoSettingsMenu").GetMethod ("createSettings");
					instance = mi.Invoke(null, null) as IsoSettings;
					#endif
				} else {
					//To prevent crashing
					Debug.LogWarning("Iso settings not found in /Resources. Runtime instance created. Consider to create the Asset.");
					instance = ScriptableObject.CreateInstance<IsoSettings> ();
				}
			}

            #if UNITY_EDITOR
            /*if (!instance.Configured)
            {
                // Only if called in OnGUI
                if (Event.current.type == EventType.Repaint)
                {
                    if (showPopup)
                    {
                        Rect windowRect = GUI.Window(0, windowRect, DoMyWindow, "My Window");
                    }
                }

            }*/
            #endif
        }

		return instance;
	}
}
