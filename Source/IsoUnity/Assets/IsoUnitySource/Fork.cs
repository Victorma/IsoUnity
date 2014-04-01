using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class Fork : ScriptableObject
{

	public enum ForkTypes {Switch, Item, GameTime};

	private ForkTypes forktype;
	public string switchID;
	public bool switchstate;
	public Item item;
	public int gametime;

	public Fork(){
		this.forktype = ForkTypes.Switch;
	}

	public ForkTypes getForkType(){
		return this.forktype;
	}

	public void setForkType(ForkTypes newType){
		this.forktype = newType;
	}

	public string[] getTypes(){
		return ForkTypes.GetNames(typeof(ForkTypes));
	}

	public bool check(){
		bool chk = false;
		switch (this.forktype) {
		case ForkTypes.Switch :
			chk = (IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch(this.switchID) == this.switchstate);
		break;

		case ForkTypes.Item :
		break;

		case ForkTypes.GameTime :
		break;
		}
		return chk;
	}
}