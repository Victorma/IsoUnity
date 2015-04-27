using UnityEngine;
using System.Collections;

public abstract class IsoUnityType : ScriptableObject
{
    public abstract bool canHandle(object o);
    public abstract IsoUnityType clone();
    public abstract object Value
    {
        get;
        set;
    }
}
