using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class IsoSwitches : ScriptableObject
{
	[SerializeField]
	public List<ISwitch> switches = new List<ISwitch>();


	public IsoSwitches (){
	}

	public ISwitch addSwitch(){
		ISwitch iss = ScriptableObject.CreateInstance<ISwitch>();
		this.switches.Add(iss);
		return iss;
	}

	public void removeSwitch(ISwitch swt){
		if(this.switches.Contains(swt))
		   this.switches.Remove (swt);
		//ScriptableObject.Destroy (swt);
	}

	public ISwitch getSwitch(string id){
		ISwitch r = null;
		foreach (ISwitch isw in this.switches) {
			if(isw.id.Equals(id)){
				r = isw;
				break;
			}
		}
		if(r == null){
			r = addSwitch();
			r.id = id;
		}
		return r;
	}

	public ISwitch[] getList(){
		return this.switches.ToArray() as ISwitch[];
	}

	public object consultSwitch(string id){
		return getSwitch (id).State;
	}
}
