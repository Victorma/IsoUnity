
using UnityEngine;

public class DialogGUI : IsoGUI {
	private Dialog.Fragment frg;
	private Texture2D picture;
	private string name;
	private string msg;
	private Talker tlk;

	public DialogGUI(Talker tlk, Dialog.Fragment frg){
		this.frg = frg;
		this.picture = frg.Face;
		this.name = frg.Name;
		this.msg = frg.Msg;
		this.tlk = tlk;
	}

	public void showMessage(){
	}


	public override void draw(){

		GUIStyle stl = new GUIStyle ();
		//stl.normal.background = picture;
		GUI.Box(new Rect(0,0, Screen.width, 200),"");

		stl = new GUIStyle ();
		stl.normal.background = picture;
		GUI.Box(new Rect(100,50,100,100), "", stl);

		stl = new GUIStyle ();
		stl.fontStyle = FontStyle.Bold;
		stl.fontSize = 20;
		stl.normal.textColor = Color.white;
		GUI.Label(new Rect (250, 50, Screen.width - 50, 25), this.name);
		GUI.Label(new Rect (250, 75, Screen.width - 50, 175), this.msg);

	}

	public override void fillControllerEvent (ControllerEventArgs args)
	{
		if (args.isLeftDown) {
			GameEvent ge = ScriptableObject.CreateInstance<GameEvent>();
			ge.Name = "ended fragment"; ge.setParameter ("Talker", tlk);
			Game.main.enqueueEvent (ge);
			GUIManager.removeGUI (this);
		}
		//throw new System.NotImplementedException ();
	}
};


