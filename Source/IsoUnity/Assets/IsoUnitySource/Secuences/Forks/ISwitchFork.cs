using UnityEngine;
using System.Collections;

public class ISwitchFork : Checkable {

	void OnEnable(){
		if(value == null)
			value = ScriptableObject.CreateInstance<IsoUnityBasicType>();
	}

	public enum ComparationType {Equal, Greather, Less, Distinct, GreatherEqual, LessEqual};
	[SerializeField]
	public ComparationType comparationType = ComparationType.Equal;
	[SerializeField]
	public string id = "";
	[SerializeField]
	private IsoUnityBasicType value;
	public object Value {
		get { return value.Value; }
		set { this.value.Value = value; } 
	}

	public override bool check(){

		System.IComparable chk = (System.IComparable) IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch(id);
		if(chk == null)
			return false;

		bool c = false;
		switch(comparationType){
			case ComparationType.Equal:			c = chk.CompareTo(value.Value) == 0; break;
			case ComparationType.Greather: 		c = chk.CompareTo(value.Value)  > 0; break;
			case ComparationType.Less: 			c = chk.CompareTo(value.Value)  < 0; break;
			case ComparationType.Distinct: 		c = chk.CompareTo(value.Value) != 0; break;
			case ComparationType.GreatherEqual: c = chk.CompareTo(value.Value) >= 0; break;
			case ComparationType.LessEqual: 	c = chk.CompareTo(value.Value) <= 0; break;
		}

		return c;
	}
}
