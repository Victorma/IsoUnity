using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IsoUnityCollectionType : IsoUnityType
{
    [SerializeField]
    private IList l;
    [SerializeField]
    private string whatIs = "";

    public override IsoUnityType clone()
    {
        return IsoUnityCollectionType.CreateInstance<IsoUnityCollectionType>();
    }

    public override bool canHandle(object o)
    {
        return o is IList;
    }

    public override object Value
    {
        get
        {
            if (whatIs == l.GetType().ToString()) return l;

            return null;
        }
        set
        {
            if (value is IList) { l = (IList)value; whatIs = l.GetType().ToString(); }
        }
    }
  
}
