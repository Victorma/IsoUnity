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
	private string name = "";
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
		if (args.ContainsKey (param)) 
			return (args[param] is IsoUnityBasicType)? ((IsoUnityBasicType)args [param]).Value: args[param];
		else 
			return null;
	}

	public void setParameter(string param, object content){
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
}

