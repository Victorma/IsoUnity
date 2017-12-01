using UnityEngine;
using System.Collections;

namespace IsoUnity.Sequences {
	public abstract class Condition : ScriptableObject {

		public abstract bool Eval();
	}
}