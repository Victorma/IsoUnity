
using UnityEngine;

public class DialogGUI : IsoGUI {
	private Dialog.Fragment frg;
	private Dialog.DialogOption[] opt;
	private Texture2D picture;
	private string name;
	private string msg;
	private Object launcher;

	private int mode; // 0 = fragment mode; 1 = option mode;

	bool initialized = false;
	float textmultiplier = 1f;
	GUIStyle sbox, stitle, smessage;

	public void init(Object launcher, Dialog.Fragment frg){
		this.picture = frg.Face;
		this.name = frg.Name;
		this.msg = frg.Msg;
		this.launcher = launcher;
		this.mode = 0;
	}

	public void init(Object launcher, Dialog.DialogOption[] opt){
		this.opt = opt;
		this.launcher = launcher;
		this.mode = 1;
	}

	public void showMessage(){
	}


	public override void draw(){

		switch (this.mode){
		case 0: {
				
				float relation = 0.30f;

				if(Screen.width<Screen.height)
					relation = 0.20f;
				
				if(!initialized){
					GUI.skin = Resources.Load<GUISkin>("Skin");
					
					sbox = new GUIStyle(GUI.skin.FindStyle("DialogGUIBox"));
					stitle = new GUIStyle(GUI.skin.FindStyle ("DialogGUITitle"));
				    smessage = new GUIStyle(GUI.skin.FindStyle("DialogGUIMessage"));

					if(Screen.dpi!=0){
						textmultiplier = (Screen.dpi/160f);
					}

					stitle.fontSize = Mathf.RoundToInt(stitle.fontSize*textmultiplier);
					smessage.fontSize = Mathf.RoundToInt(smessage.fontSize*textmultiplier);
					initialized = true;
				}
				

				GUI.Box (new Rect (0, 0, Screen.width, Screen.height*relation),"", sbox);
				
				GUIStyle stl = new GUIStyle ();
				stl.normal.background = picture;
				GUI.Box (new Rect (Screen.height*relation*0.1f, Screen.height*relation*0.1f, Screen.height*relation*0.8f, Screen.height*relation*0.8f), "", stl);
				
				stl = new GUIStyle ();
				stl.fontStyle = FontStyle.Bold;
				stl.fontSize = 20; 
				stl.normal.textColor = Color.white;

				GUI.Label (new Rect (Screen.height*relation, Screen.height*relation*0.1f, Screen.width - Screen.height*relation*0.1f, stitle.fontSize), this.name, stitle);
				GUI.Label (new Rect (Screen.height*relation, Screen.height*relation*0.1f+stitle.fontSize+Screen.height*relation*0.05f, Screen.width - Screen.height*relation*0.1f, Screen.height*relation-(stitle.fontSize+Screen.height*relation*0.05f)), this.msg, smessage);
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
			ScriptableObject.DestroyImmediate(this);
		}
		//throw new System.NotImplementedException ();
	}
};


