using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace IsoUnity.Sequences {
	[NodeContent("Dialog", new string[] { "next" })]
	public class Dialog : ScriptableObject
	{

	    public static Dialog Create(params Fragment[] fragments)
	    {
	        return Create(new List<Fragment>(fragments));
	    }

	    public static Dialog Create(List<Fragment> fragments)
	    {
	        var d = ScriptableObject.CreateInstance<Dialog>();
	        d.fragments = fragments;
	        return d;
	    }

	    [SerializeField]
	    private List<Fragment> fragments = new List<Fragment>();

	    public List<Fragment> Fragments {
	        get
	        {
	            if (fragments == null)
	                fragments = new List<Fragment>();
	            return this.fragments;
	        }
	        set { this.fragments = value; }
	    }

	    public void AddFragment(string name = "", string msg = "", string character = "", string parameter = "")
	    {
	        Fragments.Add(new Fragment(name, msg, character, parameter));
		}

		public void RemoveFragment(Fragment frg){
	        Fragments.Remove(frg);
		}
	}

	[System.Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Fragment : ICloneable
	{
	    [SerializeField]
	    private string name = string.Empty;
	    [SerializeField]
	    private string msg = string.Empty;
	    [SerializeField]
	    private string character = string.Empty;
	    [SerializeField]
	    private string parameter = string.Empty;

	    public string Name
	    {
	        get { return name; }
	        set { this.name = value; }
	    }

	    public string Msg
	    {
	        get { return msg; }
	        set { msg = value; }
	    }

	    public string Character
	    {
	        get { return character; }
	        set { character = value; }
	    }

	    public string Parameter
	    {
	        get { return parameter; }
	        set { parameter = value; }
	    }

	    public Fragment(string name = "", string msg = "", string character = "", string parameter = "")
	    {
	        this.name = name;
	        this.msg = msg;
	        this.character = character;
	        this.parameter = parameter;
	    }

	    object ICloneable.Clone()
	    {
	        return this.MemberwiseClone();
	    }

	    public Fragment Clone()
	    {
	        return this.MemberwiseClone() as Fragment;
	    }
	}
}