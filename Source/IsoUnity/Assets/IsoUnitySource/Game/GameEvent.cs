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
}

