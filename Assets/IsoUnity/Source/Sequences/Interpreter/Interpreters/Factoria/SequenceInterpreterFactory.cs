using UnityEngine;
using System.Collections.Generic;

namespace IsoUnity.Sequences {
	public abstract class SequenceInterpreterFactory {
		
		private static SequenceInterpreterFactory instance;
		public static SequenceInterpreterFactory Intance {
			get{ 
				if(instance == null)
					instance = new SequenceInterpreterFactoryImp();
				return instance; 
			}
		}

		public abstract ISequenceInterpreter createSequenceInterpreterFor (SequenceNode node);
		
	}

	public class SequenceInterpreterFactoryImp : SequenceInterpreterFactory
	{
		
		private List<ISequenceInterpreter> sequenceInterpreters;
		
		public SequenceInterpreterFactoryImp(){

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
			sequenceInterpreters = new List<ISequenceInterpreter>();
	        sequenceInterpreters.Add(new SimpleContentInterpreter());
	        sequenceInterpreters.Add(new CheckableInterpreter());
	        sequenceInterpreters.Add(ScriptableObject.CreateInstance<DialogInterpreter>());
	        sequenceInterpreters.Add(new GameEventInterpreter());
	    }
		
		public override ISequenceInterpreter createSequenceInterpreterFor (SequenceNode node)
		{
			foreach(ISequenceInterpreter si in sequenceInterpreters){
				if(si.CanHandle(node))
					return si.Clone();
			}
			return null;
		}

	}
}