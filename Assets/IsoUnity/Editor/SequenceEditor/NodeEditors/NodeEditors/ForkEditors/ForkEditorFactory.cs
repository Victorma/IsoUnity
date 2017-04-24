using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using IsoUnity.Sequences;
using System.Linq;

public abstract class ForkEditorFactory {

	private static ForkEditorFactory instance;
	public static ForkEditorFactory Intance {
		get{ 
			if(instance == null)
				instance = new ForkEditorFactoryImp();
			return instance; 
		}
	}

	public abstract string[] CurrentForkEditors { get; }
    public abstract Checkable createForkOf(string forkName);
    public abstract Editor createForkEditorFor(Checkable forkName);
    public abstract int ForkEditorIndex(Checkable fork);

}

public class ForkEditorFactoryImp : ForkEditorFactory {

	private List<Editor> forkEditors;
	private Editor defaultForkEditor;

	public ForkEditorFactoryImp(){
		this.forkEditors = new List<Editor> ();
	}

	public override string[] CurrentForkEditors {
		get {
			return GetPossibleCreations().Keys.ToArray();
		}
	}
    
    public override Checkable createForkOf(string forkName)
    {
        if(GetPossibleCreations().ContainsKey(forkName))
            return ScriptableObject.CreateInstance(GetPossibleCreations()[forkName]) as Checkable;

        return null;
    }

    public override Editor createForkEditorFor (Checkable forkName)
	{
        return Editor.CreateEditor(forkName);
    }

	public override int ForkEditorIndex(Checkable fork){
        if (fork == null)
            return 0;

        return GetPossibleCreations().Values.ToList().FindIndex(t => t == fork.GetType());
	}

    private Dictionary<string, Type> possibleCreationsCache;
    private Dictionary<string, Type> GetPossibleCreations()
    {
        if (possibleCreationsCache == null)
        {
            possibleCreationsCache = new Dictionary<string, Type>();
            // Make sure is a DOMWriter
            var contents = AttributesUtil.GetTypesWith<NodeContentAttribute>(true).Where(t => (typeof(Checkable)).IsAssignableFrom(t));
            foreach (var content in contents)
            {
                foreach (var attr in content.GetCustomAttributes(typeof(NodeContentAttribute), true))
                {
                    var nodeContent = attr as NodeContentAttribute;
                    var name = nodeContent.Name == string.Empty ? content.ToString() : nodeContent.Name;
                    possibleCreationsCache.Add(name, content);
                }
            }
        }
        return possibleCreationsCache;
    }
}