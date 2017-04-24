using UnityEngine;
using System.Collections;
using System;

namespace IsoUnity.Sequences {
	[NodeContent("Fork/Group/Any", 2)]
	public class AnyFork : ForkGroup {

	    public override bool check()
	    {
	        return forks.Count == 0 || forks.Exists(f => f.check());
	    }

	    public override string ToString()
	    {
	        return string.Join(" || ", forks.ConvertAll(f => "( " + f.ToString() + " )").ToArray());
	    }
	}
}