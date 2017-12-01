using UnityEngine;
using System.Collections;
using System;

namespace IsoUnity {
	public class IsoUnityEnumType : IsoUnityType
	{
        [SerializeField]
	    private int enumerated = 0;
        [SerializeField]
        private string type = "";

        private void OnEnable()
        {
        }

        public override bool canHandle(object o)
	    {
	        return o is System.Enum;
	    }

	    public override IsoUnityType clone()
	    {
	        return IsoUnityEnumType.CreateInstance<IsoUnityEnumType>();
	    }

	    public override object Value
	    {
	        get
	        {
                var t = Type.GetType(type);
                return t != null ? Enum.ToObject(t, enumerated): enumerated;
	        }
	        set
	        {
	            enumerated = (int)value;
                type = value.GetType().FullName;
	        }
	    }

	    public override JSONObject toJSONObject()
	    {
	        throw new System.NotImplementedException();
	    }

	    public override void fromJSONObject(JSONObject json)
	    {
	        throw new System.NotImplementedException();
	    }
	}
}