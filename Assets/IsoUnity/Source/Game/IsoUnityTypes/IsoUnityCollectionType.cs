using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IsoUnityCollectionType : IsoUnityType
{
    [SerializeField]
    private IsoUnityList l;
    [SerializeField]
    private string whatIs = "";

    public override IsoUnityType clone()
    {
        return IsoUnityCollectionType.CreateInstance<IsoUnityCollectionType>();
    }

    public override bool canHandle(object o)
    {
        return o is IList<object>;
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
            if (value is IList<object>) { 
                var lo = value as IList<object>;
                if (lo is IsoUnityList)
                {
                    l = lo as IsoUnityList;
                }
                else
                {
                    var myList = IsoUnityList.CreateInstance<IsoUnityList>();
                    foreach (var o in lo)
                        myList.Add(o);
                    l = myList;
                }
                whatIs = l.GetType().ToString(); 
            }
        }
    }

    public override void fromJSONObject(JSONObject json)
    {
        List<JSONObject> ljo = json.list;
        if (this.l == null)
            this.l = ScriptableObject.CreateInstance<IsoUnityList>();
        else
            this.l.Clear();
        foreach (var jo in ljo)
        {
            JSONAble unserialized = JSONSerializer.UnSerialize(jo);
            this.l.Add(unserialized);
        }
    }

    public override JSONObject toJSONObject()
    {
        JSONObject[] array = new JSONObject[l.Count];
        int i = 0;
        foreach (var o in this.l.getSerializable())
        {
            array[i] = JSONSerializer.Serialize(o);
            i++;
        }
        return new JSONObject(array);
    }
  
}
