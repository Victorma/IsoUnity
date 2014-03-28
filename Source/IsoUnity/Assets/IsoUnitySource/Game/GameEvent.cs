using System.Collections.Generic;

public class GameEvent {
	
	private string name;
	public string Name {get;set;}

	private Dictionary<string, object> args = new Dictionary<string, object>();
	public object getParameter(string param){
		if(args.ContainsKey(param))
			return args[param];
		else
			return null;
	}

	public void setParameter(string param, object content){
		if(args.ContainsKey(param))
			args[param] = content;
		else
			args.Add(param, content);
	}

	public void removeParameter(string param){
		if(args.ContainsKey(param))
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
}

