using UnityEngine;
using System.Collections;

[System.Serializable]
public class IsoUnityBasicType : IsoUnityType {

	public int i;
	public float f;
	public string s;
	public Vector2 v2;
	public Vector3 v3;
	public Vector4 v4;
	public Quaternion q;
	public bool b;
	public char c;

	public string whatIs;

    public override IsoUnityType clone()
    {
        return IsoUnityBasicType.CreateInstance<IsoUnityBasicType>();
    }

    public override bool canHandle(object o)
    {
        if (o == null)
            return false;

        string[] types = new string[]{
            typeof(string).ToString(),
            typeof(Vector2).ToString(),
            typeof(Vector3).ToString(),
            typeof(Vector4).ToString(),
            typeof(Quaternion).ToString(),
            typeof(int).ToString(),
            typeof(float).ToString(),
            typeof(bool).ToString(),
            typeof(char).ToString()
        };

        var myType = o.GetType().ToString();

        foreach (var type in types)
            if (type == myType)
                return true;

        return false;
    }

	public override object Value {
		get{
			object vt = null;

			if 	   (whatIs == typeof(int).ToString())		{vt = i;}
			else if(whatIs == typeof(float).ToString())		{vt = f;}
			else if(whatIs == typeof(string).ToString())	{vt = s;}
			else if(whatIs == typeof(Vector2).ToString())	{vt = v2;}
			else if(whatIs == typeof(Vector3).ToString())	{vt = v3;}
			else if(whatIs == typeof(Vector4).ToString())	{vt = v4;}
			else if(whatIs == typeof(Quaternion).ToString()){vt = q;}
			else if(whatIs == typeof(bool).ToString())		{vt = b;}
			else if(whatIs == typeof(char).ToString())		{vt = c;}

			return vt;
		}
		set{
			object vt = value;
			if(vt!=null){
				whatIs = vt.GetType().ToString();

				if 	   (vt.GetType() == typeof(int))		{i = (int)vt;}
				else if(vt.GetType() == typeof(float))		{f = (float)vt;}
				else if(vt.GetType() == typeof(string))		{s = (string)vt;}
				else if(vt.GetType() == typeof(Vector2))	{v2 = (Vector2)vt;}
				else if(vt.GetType() == typeof(Vector3))	{v3 = (Vector3)vt;}
				else if(vt.GetType() == typeof(Vector4))	{v4 = (Vector4)vt;}
				else if(vt.GetType() == typeof(Quaternion)) {q = (Quaternion)vt;}
				else if(vt.GetType() == typeof(bool))		{b = (bool)vt;}
				else if(vt.GetType() == typeof(char))		{c = (char)vt;}
				else whatIs = null;
			}else whatIs = null;

		}
	}

    public override JSONObject toJSONObject()
    {

        JSONObject o = null;
        if (whatIs == typeof(int).ToString()) { o = new JSONObject(i); }
        else if (whatIs == typeof(float).ToString()) { o = new JSONObject(f); }
        else if (whatIs == typeof(string).ToString()) { o = JSONObject.CreateStringObject(s); }
        else if (whatIs == typeof(Vector2).ToString()) { o = JSONObject.CreateStringObject(v2.ToString()); }
        else if (whatIs == typeof(Vector3).ToString()) { o = JSONObject.CreateStringObject(v3.ToString()); }
        else if (whatIs == typeof(Vector4).ToString()) { o = JSONObject.CreateStringObject(v4.ToString()); }
        else if (whatIs == typeof(Quaternion).ToString()) { o = JSONObject.CreateStringObject(q.ToString()); }
        else if (whatIs == typeof(bool).ToString()) { o = new JSONObject(b); }
        else if (whatIs == typeof(char).ToString()) { o = new JSONObject(c); }
        return o;
    }

    public override void fromJSONObject(JSONObject json)
    {
        if (json.IsString)
        {
            object vq = null;// VectorUtil.getVQ(param.str);
            if (vq == null)
                Value = json.str;
            else Value = vq;
        }
        else if (json.IsBool)
        {
            Value = json.b;
        }
        else if (json.IsNumber)
        {
            int i = Mathf.RoundToInt(json.n);
            if (json.n == i) Value = i;
            else Value = json.n;
        }
    }

}
