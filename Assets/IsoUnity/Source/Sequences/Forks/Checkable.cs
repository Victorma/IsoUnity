using UnityEngine;
using System.Collections.Generic;

namespace IsoUnity.Sequences {
	[System.Serializable]
	public abstract class Checkable : ScriptableObject, IFork
	{
		public abstract bool check();

	}
}