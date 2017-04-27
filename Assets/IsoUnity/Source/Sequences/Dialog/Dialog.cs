using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using IsoUnity.Entities;

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

	    public void AddFragment(string name = "", string msg = "", bool isEntityFragment = false, Entity entity = null)
	    {
	        Fragments.Add(new Fragment(name, msg, isEntityFragment, entity));
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
        private Texture2D face;
        [SerializeField]
        private string name;
        [SerializeField]
        private string msg;
        [SerializeField]
        private Entity entity;
        [SerializeField]
        private bool isEntityFragment;

        public Texture2D Face
        {
            get
            {
                if (this.face == null && this.Entity != null)
                {
                    return Entity.face;
                }
                return face;
            }
            set
            {
                if (this.Entity == null)
                    this.face = value;
                else if (this.Entity != null && this.Entity.face != value)
                    this.face = value;
                else
                    this.face = null;
            }
        }

        public string Name
        {
            get
            {
                if (this.name == "" && this.Entity != null)
                {
                    return Entity.name;
                }
                return name;
            }
            set
            {
                if (this.Entity == null)
                    this.name = value;
                else if (this.Entity != null && this.Entity.name != value)
                    this.name = value;
                else
                    this.name = "";
            }
        }

        public Entity Entity
        {
            get { return (isEntityFragment) ? entity : null; }
            set { this.entity = value; }
        }

        public bool IsEntityFragment
        {
            get { return isEntityFragment; }
            set { isEntityFragment = value; }
        }

        public string Msg
        {
            get { return msg; }
            set { msg = value; }
        }

        public Fragment(string name = "", string msg = "", bool isEntityFragment = false, Entity entity = null)
	    {
            this.isEntityFragment = false;
            this.name = name;
            this.msg = msg;
            this.entity = entity;
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