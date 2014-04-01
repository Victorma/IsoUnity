using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class IsoSwitches : ScriptableObject
{
	[System.Serializable]
	public class ISwitch{
		[SerializeField]
		public string id;
		[SerializeField]
		public bool state;

		public ISwitch(){
			this.id = "";
			this.state = false;
		}
	}
	[SerializeField]
	public ISwitch[] switches;


	public IsoSwitches (){
	}

	public void addSwitch(){
		if (this.switches != null) {
			ISwitch[] tmp = new ISwitch [this.switches.Length + 1];
			for (int i=0; i<this.switches.Length; i++) {
				tmp [i] = this.switches [i];
			}
			tmp [this.switches.Length] = new ISwitch ();
			this.switches = tmp;
		} else {
			this.switches = new ISwitch[1];
			this.switches[0] = new ISwitch();
		}
	}

	public void removeSwitch(ISwitch swt){
		int k = 0;
		ISwitch[] tmp = new ISwitch [this.switches.Length - 1];
		for (int i=0; i<this.switches.Length-1; i++) {
			if(swt == this.switches[i]) k = 1;
			tmp[i] = this.switches[i+k];
		}
		this.switches = tmp;
	}

	public ISwitch getSwitch(string id){
		ISwitch r = null;
		foreach (ISwitch isw in this.switches) {
			if(isw.id.Equals(id)){
				r = isw;
				break;
			}
		}
		return r;
	}

	public ISwitch[] getList(){
		return this.switches;
	}

	public bool consultSwitch(string id){
		return getSwitch (id).state;
	}
}
