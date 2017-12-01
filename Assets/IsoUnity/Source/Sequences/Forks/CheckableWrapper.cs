using UnityEngine;
using System.Collections;
using System;

namespace IsoUnity.Sequences {
	public class CheckableWrapper : Checkable {

	    private IFork fork;
	    public CheckableWrapper(IFork fork)
	    {
	        this.fork = fork;
	    }

	    public override bool check()
	    {
	        return fork.check();
	    }
	}
}