using UnityEngine;
using System.Collections;

namespace IsoUnity
{
    public interface JSONAble
    {
        JSONObject toJSONObject();
        void fromJSONObject(JSONObject json);
    }
}