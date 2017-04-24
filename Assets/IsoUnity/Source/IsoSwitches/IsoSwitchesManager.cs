using System;
using System.Collections.Generic;
using UnityEngine;

namespace IsoUnity {
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
		//private String ruta;
		private IsoSwitches resource;
	    private IsoSwitches instance;

		public IsoSwitchesManagerInstance(){
			//ruta = "Assets/Resources/IsoSwitches.asset";
		}
		
		
		public override IsoSwitches getIsoSwitches(){

			if (resource == null) {
	            resource = Resources.Load<IsoSwitches> ("IsoSwitches");

				if(resource == null){
					if (Application.isEditor) {
						#if UNITY_EDITOR
						System.Reflection.MethodInfo mi = Type.GetType ("IsoSwitchesMenu").GetMethod ("createSwitches");
						instance = mi.Invoke(null, null) as IsoSwitches;
						#endif
					} else {
						//To prevent crashing
						Debug.LogWarning("Iso switches not found in /Resources. Runtime instance created. Consider to create the Asset.");
	                    resource = ScriptableObject.CreateInstance<IsoSwitches> ();
					}
				}

	            instance = resource.Clone();
			}
			
			return Application.isPlaying ? instance : resource;
		}
	}
}