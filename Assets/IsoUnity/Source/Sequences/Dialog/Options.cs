using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IsoUnity.Sequences {
	[NodeContent("Options")]
	public class Options : ScriptableObject, NodeContent, System.ICloneable
	{
	    
	    public string[] ChildNames { get { return options.ConvertAll(o => o.Text).ToArray(); } }
	    public int ChildSlots { get { return options.Count; } }
	    
	    public static Options Create(params Option[] options)
	    {
	        return Create(new List<Option>(options));
	    }

	    public static Options Create(List<Option> options)
	    {
	        var op = CreateInstance<Options>();
	        op.options = options;
	        return op;
	    }

	    [SerializeField]
	    private List<Option> options = new List<Option>();
	    
	    public string Question { get; set; }

	    public List<Option> Values
	    {
	        get
	        {
	            if (options == null)
	                options = new List<Option>();

	            return this.options;
	        }
	        set { this.options = value; }
	    }

	    public void AddOption(string option = "", string parameter = "")
	    {
	        var conditions = ScriptableObject.CreateInstance<AnyFork>();
#if UNITY_EDITOR
	        if (Application.isEditor && !Application.isPlaying 
                && (UnityEditor.AssetDatabase.IsMainAsset(this) || UnityEditor.AssetDatabase.IsSubAsset(this)))
	        {
	            UnityEditor.AssetDatabase.AddObjectToAsset(conditions, this);
	        }
	#endif
	        this.Values.Add(new Option(option, parameter, conditions));
	    }

	    public void removeOption(Option option)
	    {
	        this.Values.Remove(option);

	#if UNITY_EDITOR
	        if (Application.isEditor && !Application.isPlaying
                && (UnityEditor.AssetDatabase.IsMainAsset(this) || UnityEditor.AssetDatabase.IsSubAsset(this)))
            {
	            ScriptableObject.DestroyImmediate(option.Fork, true);
	        }
	        else
	        {
	            ScriptableObject.DestroyImmediate(option.Fork);
	        }
	#else
			ScriptableObject.DestroyImmediate(option.Fork);
	#endif
	    }
	    
	    public object Clone()
	    {
	        var r = this.MemberwiseClone() as Options;
	        
	        r.Question = this.Question;
	        r.options = options.ConvertAll(o => o.Clone() as Option);

	        return r;
	    }
	}


	[System.Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Option : System.ICloneable
	{
	    [SerializeField]
	    private Checkable fork;
	    [SerializeField]
	    private string text = string.Empty;
	    [SerializeField]
	    private string parameter = string.Empty;

	    public Option(string text = "", string parameter = "", Checkable fork = null)
	    {
	        this.text = text;
	        this.parameter = parameter;
	        this.fork = fork;
	    }

	    public string Text { get { return text; } set { this.text = value; } }
	    public string Parameter { get { return parameter; } set { this.parameter = value; } }
	    public Checkable Fork { get { return fork; } }


	    public object Clone()
	    {
	        return this.MemberwiseClone() as Option;
	    }
	}
}