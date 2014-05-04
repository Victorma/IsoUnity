using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class Checkable : ScriptableObject
{
	public abstract bool check();

}