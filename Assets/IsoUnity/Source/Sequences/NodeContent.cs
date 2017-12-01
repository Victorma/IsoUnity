using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsoUnity.Sequences {
	public interface NodeContent {
	    
	    string[] ChildNames { get; }
	    int ChildSlots { get; }
	}
}