using System;

namespace IsoUnity.Sequences {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class NodeContentAttribute : Attribute {

	    // constructors
	    public NodeContentAttribute() : this(string.Empty, 1) { }
	    public NodeContentAttribute(string name) : this(name, 1) { }
	    public NodeContentAttribute(int slots) : this(string.Empty, slots) { }
	    public NodeContentAttribute(string[] childNames) : this(string.Empty, childNames) { }
	    public NodeContentAttribute(string name, int slots)
	    {
	        var names = new string[slots];
	        for (var i = 0; i < slots; i++)
	            names[i] = i.ToString();

	        ChildNames = names;
	        Name = name;
	        Slots = slots;
	    }

	    public NodeContentAttribute(string name, string[] childNames)
	    {
	        ChildNames = childNames;
	        Name = name;
	        Slots = childNames.Length;
	    }

	    // Public attributes
	    public string[] ChildNames { get; protected set; }
	    public string Name { get; protected set; }
	    public int Slots { get; protected set; }
	}
}