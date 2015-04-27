using UnityEngine;
using System;
using System.Collections;

public class JSONSerializer  {

    public static JSONObject Serialize(object jsonAble)
    {
        JSONObject json = new JSONObject();

        if (jsonAble is IsoUnityType)
        {
            json = ((JSONAble)jsonAble).toJSONObject();
        }
        else if(jsonAble is JSONAble)
        {
            json.AddField("_class", jsonAble.GetType().ToString());
            json.AddField("_data", ((JSONAble)jsonAble).toJSONObject());
        }
        else if (jsonAble is UnityEngine.Object)
        {
            json = new JSONObject(((UnityEngine.Object)jsonAble).GetInstanceID());
        }

        return json;
    }

    public static JSONAble UnSerialize(JSONObject jsonObject){
        JSONAble r = null;

        if (jsonObject.HasField("_class") && jsonObject.HasField("_data"))
        {
            string c = jsonObject.GetField("_class").str;
            Type t = Type.GetType(c);
            if (t.IsSubclassOf(typeof(JSONAble)))
            {
                if (t.IsSubclassOf(typeof(ScriptableObject)))
                {
                    r = ScriptableObject.CreateInstance(t) as JSONAble;
                    r.fromJSONObject(jsonObject.GetField("_data"));
                }
            }
        }
        else if (jsonObject.IsArray)
        {
            r = ScriptableObject.CreateInstance<IsoUnityCollectionType>();
            r.fromJSONObject(jsonObject);
        }
        else if (jsonObject.IsString || jsonObject.IsNumber || jsonObject.IsBool)
        {
            r = ScriptableObject.CreateInstance<IsoUnityBasicType>();
            r.fromJSONObject(jsonObject);
        }

        return r;
    }

}
