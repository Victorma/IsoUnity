
using UnityEngine;

public class DialogGUI : IsoGUI {
	private Dialog.Fragment frg;
	private Dialog.DialogOption[] opt;
	private Texture2D picture;
	private string name;
	private string msg;
	private Object launcher;

	private int mode; // 0 = fragment mode; 1 = option mode;

	public DialogGUI(Object launcher, Dialog.Fragment frg){
		this.picture = frg.Face;
		this.name = frg.Name;
		this.msg = frg.Msg;
		this.launcher = launcher;
		this.mode = 0;
	}

	public DialogGUI(Object launcher, Dialog.DialogOption[] opt){
		this.opt = opt;
		this.launcher = launcher;
		this.mode = 1;
	}

	public void showMessage(){
	}


	public override void draw(){

		switch (this.mode){
		case 0: {
				GUIStyle stl = new GUIStyle ();
				//stl.normal.background = picture;
				GUI.Box (new Rect (0, 0, Screen.width, 200), "");

				stl = new GUIStyle ();
				stl.normal.background = picture;
				GUI.Box (new Rect (100, 50, 100, 100), "", stl);

				stl = new GUIStyle ();
				stl.fontStyle = FontStyle.Bold;
				stl.fontSize = 20; 
				stl.normal.textColor = Color.white;
				GUI.Label (new Rect (250, 50, Screen.width - 50, 25), this.name);
				GUI.Label (new Rect (250, 75, Screen.width - 50, 175), this.msg);
				break;
			}
		case 1: {
			float height = Screen.height/opt.Length;
			for(int i=0; i<opt.Length; i++){
				if (GUI.Button (new Rect (0, i*height, Screen.width, height), this.opt[i].text)){
					GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
					ge.Name = "chosen option"; 
					ge.setParameter ("launcher", launcher);
					ge.setParameter("option", i);
					Game.main.enqueueEvent (ge);
					GUIManager.removeGUI (this);
				}
			}
			break;
			}
		}

	}

	public override void fillControllerEvent (ControllerEventArgs args)
	{
		if(this.mode == 0)
		if (args.isLeftDown) {
			GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
			ge.Name = "ended fragment"; ge.setParameter ("Launcher", launcher);
			Game.main.enqueueEvent (ge);
			GUIManager.removeGUI (this);
		}
		//throw new System.NotImplementedException ();
	}
};


