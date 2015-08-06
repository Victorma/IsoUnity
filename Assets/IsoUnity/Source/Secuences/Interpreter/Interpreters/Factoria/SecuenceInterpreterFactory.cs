using UnityEngine;
using System.Collections.Generic;

public abstract class SecuenceInterpreterFactory {
	
	private static SecuenceInterpreterFactory instance;
	public static SecuenceInterpreterFactory Intance {
		get{ 
			if(instance == null)
				instance = new SecuenceInterpreterFactoryImp();
			return instance; 
		}
	}

	public abstract ISecuenceInterpreter createSecuenceInterpreterFor (SecuenceNode node);
	
}

public class SecuenceInterpreterFactoryImp : SecuenceInterpreterFactory {
	
	private List<ISecuenceInterpreter> secuenceInterpreters;
	
	public SecuenceInterpreterFactoryImp(){

		/* TODO implement assembly dynamic creation in run-time
		 * var type = typeof(ISecuenceInterpreter);
		var scriptable = typeof(ScriptableObject);
		Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();
		foreach(Assembly a in assembly)
			foreach(Type t in a.GetTypes())
				if(type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract){
					if(scriptable.IsAssignableFrom(t))
						secuenceInterpreters.Add(ScriptableObject.CreateInstance(t) as ISecuenceInterpreter);
					else
						secuenceInterpreters.Add(Activator.CreateInstance(t) as ISecuenceInterpreter);
				}
					*/
		secuenceInterpreters = new List<ISecuenceInterpreter>();
		secuenceInterpreters.Add(ScriptableObject.CreateInstance<DialogInterpreter>());
		secuenceInterpreters.Add(new GameEventInterpreter());
		secuenceInterpreters.Add(new CheckableInterpreter());
	}
	
	public override ISecuenceInterpreter createSecuenceInterpreterFor (SecuenceNode node)
	{
		foreach(ISecuenceInterpreter si in secuenceInterpreters){
			if(si.CanHandle(node))
				return si.Clone();
		}
		return null;
	}

}
