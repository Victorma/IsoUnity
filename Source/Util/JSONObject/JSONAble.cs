using UnityEngine;
using System.Collections;

public interface JSONAble 
{
    JSONObject toJSONObject();
    void fromJSONObject(JSONObject json);
}
