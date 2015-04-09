using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject{

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
			return (args[param] is IsoUnityBasicType)? ((IsoUnityBasicType)args [param]).Value: args[param];
		else 
			return null;
	}

	public void setParameter(string param, object content){
		param = param.ToLower();
		object c = content;
		if(c is System.ValueType || c is string){
			c = ScriptableObject.CreateInstance<IsoUnityBasicType>();
			((IsoUnityBasicType)c).Value = content;
		}
		if(args.ContainsKey(param))	args[param] = (Object)c;
		else						args.Add(param, (Object)c);

		this.keys = new List<string> (args.Keys);
		this.values = new List<Object> (args.Values);
	}

	public void removeParameter(string param){
		param = param.ToLower();
		if(args.ContainsKey(param))
			args.Remove(param);

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
}

