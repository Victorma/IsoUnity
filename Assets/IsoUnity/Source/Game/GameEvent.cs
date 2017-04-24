using UnityEngine;
using System.Collections.Generic;
using IsoUnity.Entities;
using IsoUnity.Sequences;

namespace IsoUnity {
	public class GameEvent : IGameEvent, NodeContent {

		public GameEvent(){

	    }

	    public GameEvent(string name)
	    {
	        this.name = name;
	    }

	    public GameEvent(string name, Dictionary<string, object> parameters)
	    {
	        this.name = name;
	        this.args = parameters;
	    }

	    public string name;
		public string Name {
			get{ return name; }
			set{ this.name = value; }
		}


		private Dictionary<string, object> args = new Dictionary<string, object>();
		public object getParameter(string param){
			param = param.ToLower();
			if (args.ContainsKey (param)) 
				return args[param];
			else 
				return null;
		}

		public void setParameter(string param, object content){
			param = param.ToLower();		
			if(args.ContainsKey(param))	args[param] = content;
			else						args.Add(param, content);
		}

		public void removeParameter(string param){
			param = param.ToLower();
	        if (args.ContainsKey(param))
				args.Remove(param);
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

	    public string[] ChildNames
	    {
	        get
	        {
	            return null;
	        }
	    }

	    public int ChildSlots
	    {
	        get
	        {
	            return 1;
	        }
	    }

	    public override bool Equals (object o)
		{
			if (o is IGameEvent)
				return GameEvent.CompareEvents (this, o as IGameEvent);
			else
				return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

	    /*
	     * Belong methods
	     */

	    private const string OWNER_PARAM = "entity";

        public bool belongsTo(Entity e) { return belongsTo(e, OWNER_PARAM); }
        public bool belongsTo(EntityScript es) { return belongsTo(es, OWNER_PARAM); }
        public bool belongsTo(GameObject g) { return belongsTo(g, OWNER_PARAM); }
	    public bool belongsTo(ScriptableObject so) { return belongsTo(so, OWNER_PARAM); }
	    public bool belongsTo(string tag) { return belongsTo(tag, OWNER_PARAM); }

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

        public bool belongsTo(GameObject g, string parameter)
	    {
	        object entityParam = getParameter(parameter);
	        if (entityParam == null || g == null)
	            return false;

			return entityParam == g || ( entityParam is string && ((string)entityParam) == g.tag);
	    }

	    public bool belongsTo(ScriptableObject so, string parameter)
	    {
	        object entityParam = getParameter(parameter);
	        if (entityParam == null || so == null)
	            return false;

	        // Compare normal and name
			return entityParam == so || ( entityParam is string && ((string) entityParam) == so.name);
	    }

	    public bool belongsTo(string tag, string parameter)
	    {
	        object entityParam = getParameter(parameter);
	        if (entityParam == null || tag == null)
	            return false;

			return ( entityParam is string && ((string)entityParam) == tag)
	            || (entityParam is GameObject) ? (entityParam as GameObject).CompareTag(tag) : false
	            || (entityParam is Component) ? (entityParam as Component).CompareTag(tag) : false;
	    }

	    /*
	     * Operators 
	     **/
			
		public static bool operator ==(GameEvent ge1, IGameEvent ge2){
			//Debug.Log ("Comparing with operator of GameEvent");
			return CompareEvents (ge1, ge2);
		}

		public static bool operator !=(GameEvent ge1, IGameEvent ge2){
			return !(ge1 == ge2);
		}

		public static bool CompareEvents(IGameEvent ge1, IGameEvent ge2)
		{
			// http://msdn.microsoft.com/en-us/library/ms173147(v=vs.80).aspx
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(ge1, ge2))
			{
				return true;
			}
			
			// If one is null, but not both, return false.
			if ((ge1 == null) || (ge2 == null))
			{
				return false;
			}


			bool result = ge1.Name.ToLower().Equals(ge2.Name.ToLower()) && ge1.Params.Length == ge2.Params.Length;

			if (result) {
				foreach (string arg in ge1.Params) { // From a to b
					var p1 = ge1.getParameter(arg);
					var p2 = ge2.getParameter(arg);

					result = (p1 == null && p2 == null) || p1.Equals(p2);
					if(!result)
						Debug.Log ("p1: " + p1 + " type( " + p1.GetType () + " ) == p2 : " + p2 + " type( " + p2.GetType () + " ) => " + (result));

					if (!result)
						break;
				}

				foreach(string arg in ge2.Params){ // From b to a
					var p1 = ge1.getParameter(arg);
					var p2 = ge2.getParameter(arg);

					result = (p1 == null && p2 == null) || p1.Equals(p2);
					if(!result)
						Debug.Log ("p1: " + p1 + " type( " + p1.GetType () + " ) == p2 : " + p2 + " type( " + p2.GetType () + " ) => " + (result));

					if (!result)
						break;
				}
			}

			
			return result;
		}

	    /// <summary>
	    /// JSon serialization things
	    /// </summary>

	    public JSONObject toJSONObject()
	    {
	        JSONObject json = new JSONObject();
	        json.AddField("name", name);
	        JSONObject parameters = new JSONObject();
	        foreach (KeyValuePair<string, object> entry in args)
	        {
				

				if (entry.Value is JSONAble) {
					var jsonAble = entry.Value as JSONAble;
					parameters.AddField (entry.Key, JSONSerializer.Serialize (jsonAble));
				} else if (entry.Value is Object) {
					var o = entry.Value as Object;
					parameters.AddField (entry.Key, o.GetInstanceID ());
				} else {
					var val = IsoUnityTypeFactory.Instance.getIsoUnityType (entry.Value);
					if (val != null && val is JSONAble)
						parameters.AddField (entry.Key, JSONSerializer.Serialize (val));
					else
						parameters.AddField (entry.Key, entry.Value.ToString ());
				}
	        }


	        json.AddField("parameters", parameters);
	        return json;
	    }

	    private static void destroyBasic(Dictionary<string, object> args)
	    {
	        if (args == null || args.Count == 0)
	            return;

			foreach (KeyValuePair<string, object> entry in args)
	            if (entry.Value is IsoUnityBasicType)
					IsoUnityBasicType.DestroyImmediate(entry.Value as IsoUnityBasicType);
	    }

	    public void fromJSONObject(JSONObject json)
	    {
	        this.name = json["name"].ToString();

	        //Clean basic types
	        destroyBasic(this.args);

	        this.args = new Dictionary<string, object>();

	        JSONObject parameters = json["parameters"];
	        foreach (string key in parameters.keys)
	        {
	            JSONObject param = parameters[key];
	            JSONAble unserialized = JSONSerializer.UnSerialize(param);
	            this.setParameter(key, unserialized);
	        }
	    }
	}

}