using UnityEngine;
using System.Collections.Generic;

public abstract class ForkEditorFactory {

	private static ForkEditorFactory instance;
	public static ForkEditorFactory Intance {
		get{ 
			if(instance == null)
				instance = new ForkEditorFactoryImp();
			return instance; 
		}
	}

	public abstract string[] CurrentForkEditors { get; }
	public abstract ForkEditor createForkEditorFor (string forkName);
	public abstract int ForkEditorIndex(Checkable fork);

}

public class ForkEditorFactoryImp : ForkEditorFactory {

	private List<ForkEditor> forkEditors;
	private ForkEditor defaultForkEditor;

	public ForkEditorFactoryImp(){
		this.forkEditors = new List<ForkEditor> ();
		this.forkEditors.Add (new ISwitchForkEditor ());
		this.forkEditors.Add (new ItemForkEditor ());

		this.defaultForkEditor = new ISwitchForkEditor ();
	}

	public override string[] CurrentForkEditors {
		get {
			string[] descriptors = new string[forkEditors.Count+1];
			descriptors[0] = "Default";
			for(int i = 0; i< forkEditors.Count; i++)
				descriptors[i+1] = forkEditors[i].ForkName;
			return descriptors;
		}
	}


	public override ForkEditor createForkEditorFor (string forkName)
	{
		/*if (forkName.ToLower () == "default")
			return defaultForkEditor;*/

		foreach (ForkEditor forkEditor in forkEditors) {
			if(forkEditor.ForkName.ToLower() == forkName.ToLower()){
				return forkEditor.clone();
			}
		}
		return defaultForkEditor;
	}

	public override int ForkEditorIndex(Checkable fork){
		/*if (defaultForkEditor.manages(fork))
			return 0;*/

		int i = 1;
		foreach (ForkEditor forkEditor in forkEditors) 
			if(forkEditor.manages(fork))	return i;
			else 							i++;

		return 0;
	}
}