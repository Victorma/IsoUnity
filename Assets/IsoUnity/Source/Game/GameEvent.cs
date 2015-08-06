using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GameEvent : ScriptableObject, JSONAble{

	void Awake(){
		if (args == null || args.Count != keys.Count) {
			args = new Dictionary<string, Object>();
			for(int i = 0; i< keys.Count; i++)
				args.Add (keys[i], values[i]);
		}
	}

	[SerializeField]
	public string Name {
		get{ return name; }
		set{ this.name = value; }
	}
	[SerializeField]
	private List<string> keys = new List<string> ();
	[SerializeField]
	private List<Object> values = new List<Object>();

	private Dictionary<string, Object> args = new Dictionary<string, Object>();
	public object getParameter(string param){
		param = param.ToLower();
		if (args.ContainsKey (param)) 
			return (args[param] is IsoUnityType)? ((IsoUnityType)args [param]).Value: args[param];
		else 
			return null;
	}

	public void setParameter(string param, object content){
		param = param.ToLower();
		object c = IsoUnityTypeFactory.Instance.getIsoUnityType(content);
        if (c == null)
            c = content;
		
		if(args.ContainsKey(param))	args[param] = (Object)c;
		else						args.Add(param, (Object)c);

		this.keys = new List<string> (args.Keys);
		this.values = new List<Object> (args.Values);
	}

	public void removeParameter(string param){
		param = param.ToLower();
        if (args.ContainsKey(param))
        {
            UnityEngine.Object v = args[param];
            if (v is IsoUnityBasicType)
                IsoUnityTypeFactory.Instance.Destroy(v as IsoUnityBasicType);

            args.Remove(param);
        }

		this.keys = new List<string> (args.Keys);
		this.values = new List<Object> (args.Values);
	}

	public string[] Params{
		get{
			string[] myParams = new string[args.Keys.Count];
			int i = 0;
			foreach(string key in args.Keys){
				myParams[i] = key; i++;
			}
			return myParams;
		}
	}

	public override bool Equals (object o)
	{
		return this == o;
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

    /*
     * Belong methods
     */

    private const string OWNER_PARAM = "entity";

    public bool belongsTo(GameObject g) { return belongsTo(g, OWNER_PARAM); }
    public bool belongsTo(Entity e) { return belongsTo(e, OWNER_PARAM); }
    public bool belongsTo(EntityScript es) { return belongsTo(es, OWNER_PARAM); }
    public bool belongsTo(ScriptableObject so) { return belongsTo(so, OWNER_PARAM); }
    public bool belongsTo(string tag) { return belongsTo(tag, OWNER_PARAM); }

    public bool belongsTo(GameObject g, string parameter)
    {
        object entityParam = getParameter(parameter);
        if (entityParam == null || g == null)
            return false;

        return entityParam == g || entityParam == g.tag;
    }

    public bool belongsTo(Entity e, string parameter)
    {
        object entityParam = getParameter(parameter);
        if (entityParam == null || e == null)
            return false;

        // Compare If is entity, if is gameobject or if is tag
        return entityParam == e || entityParam == e.gameObject || entityParam == e.tag;
    }

    public bool belongsTo(EntityScript es, string parameter)
    {
        object entityParam = getParameter(parameter);
        if (entityParam == null || es == null)
            return false;

        // Same as in entity but entity script comparition also.
        return entityParam == es || entityParam == es.Entity || entityParam == es.gameObject || entityParam == es.tag;
    }

    public bool belongsTo(ScriptableObject so, string parameter)
    {
        object entityParam = getParameter(parameter);
        if (entityParam == null || so == null)
            return false;

        // Compare normal and name
        return entityParam == so || entityParam == so.name;
    }

    public bool belongsTo(string tag, string parameter)
    {
        object entityParam = getParameter(parameter);
        if (entityParam == null || tag == null)
            return false;

        return entityParam == tag
            || (entityParam is GameObject) ? (entityParam as GameObject).CompareTag(tag) : false
            || (entityParam is Component) ? (entityParam as Component).CompareTag(tag) : false;
    }

    /*
     * Operators 
     **/

	public static bool operator ==(GameEvent ge1, GameEvent ge2)
	{
		// http://msdn.microsoft.com/en-us/library/ms173147(v=vs.80).aspx
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(ge1, ge2))
		{
			return true;
		}
		
		// If one is null, but not both, return false.
		if (((object)ge1 == null) || ((object)ge2 == null))
		{
			return false;
		}


		bool result = ge1.Name.ToLower().Equals(ge2.Name.ToLower()) && ge1.args.Count == ge2.args.Count;

		if(result)
			foreach(string arg in ge1.args.Keys){
				result = ge2.args.ContainsKey(arg) && (ge2.args[arg] == ge1.args[arg]);
				if(!result)break;
			}
		
		return result;
	}

    public static bool operator !=(GameEvent ge1, GameEvent ge2)
    {
        return !(ge1 == ge2);
    }

    /// <summary>
    /// JSon serialization things
    /// </summary>

    public JSONObject toJSONObject()
    {
        JSONObject json = new JSONObject();
        json.AddField("name", name);
        JSONObject parameters = new JSONObject();
        foreach (KeyValuePair<string, Object> entry in args)
        {
            if (entry.Value is JSONAble)
            {
                var jsonAble = entry.Value as JSONAble;
                parameters.AddField(entry.Key, JSONSerializer.Serialize(jsonAble));
            }
            else
            {
                parameters.AddField(entry.Key, entry.Value.GetInstanceID());
            }
        }


        json.AddField("parameters", parameters);
        return json;
    }

    private static void destroyBasic(Dictionary<string, Object> args)
    {
        if (args == null || args.Count == 0)
            return;

        foreach (KeyValuePair<string, Object> entry in args)
            if (entry.Value is IsoUnityBasicType)
                IsoUnityBasicType.DestroyImmediate(entry.Value);
    }

    public void fromJSONObject(JSONObject json)
    {
        this.name = json["name"].ToString();

        //Clean basic types
        destroyBasic(this.args);

        this.args = new Dictionary<string, Object>();

        JSONObject parameters = json["parameters"];
        foreach (string key in parameters.keys)
        {
            JSONObject param = parameters[key];
            JSONAble unserialized = JSONSerializer.UnSerialize(param);
            this.setParameter(key, unserialized);
        }
    }
}

