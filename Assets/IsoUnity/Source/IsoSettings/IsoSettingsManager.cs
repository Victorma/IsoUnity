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

    private static bool ignore = false;

	public IsoSettingsManagerInstance(){

	}


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
        }

#if UNITY_EDITOR
        if (!instance.Configured && Application.isEditor)
        {
            if (!ignore)
                if (UnityEditor.EditorUtility.DisplayDialog("IsoSettings not configured", "IsoSettings seems to be not configured properly. ¿Go to IsoSettings? (Ignoring can cause unexpected bugs...)", "Show", "Ignore"))
                    UnityEditor.Selection.activeObject = instance;
                else
                    ignore = true;

            /*Type isoSettingsPopup = Type.GetType("IsoSettingsPopup");

            if (isoSettingsPopup != null)
            {
                System.Reflection.MethodInfo showAgain = isoSettingsPopup.GetMethod("IsShowAgain");
                System.Reflection.MethodInfo createPopup = isoSettingsPopup.GetMethod("ShowIsoSettingsPopup");
                // Only if called in OnGUI
                if ((bool)showAgain.Invoke(null, null))
                {
                    createPopup.Invoke(null, null);
                }
            }*/
        }
#endif

		return instance;
	}
}
