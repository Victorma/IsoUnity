using UnityEngine;
using System.Collections;

public abstract class IsoUnityType : ScriptableObject, JSONAble
{
    public abstract bool canHandle(object o);
    public abstract IsoUnityType clone();
    public abstract object Value
    {
        get;
        set;
    }
    public abstract JSONObject toJSONObject();

    public abstract void fromJSONObject(JSONObject json);
}
