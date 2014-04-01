using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject{

	void Awake(){
		if (args == null || args.Count != keys.Count) {
			args = new Dictionary<string, object>();
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
	private List<object> values = new List<object>();

	private Dictionary<string, object> args = new Dictionary<string, object>();
	public object getParameter(string param){
		if (args.ContainsKey (param)) 
			return args [param];
		else 
			return null;
	}

	public void setParameter(string param, object content){
		if(args.ContainsKey(param))
			args[param] =  content;
		else
			args.Add(param, content);

		this.keys = new List<string> (args.Keys);
		this.values = new List<object> (args.Values);
	}

	public void removeParameter(string param){
		if(args.ContainsKey(param))
			args.Remove(param);

		this.keys = new List<string> (args.Keys);
		this.values = new List<object> (args.Values);
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

