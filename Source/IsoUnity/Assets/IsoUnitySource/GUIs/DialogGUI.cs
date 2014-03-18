using System;
using UnityEngine;

public class DialogGUI : IsoGUI {
	private Texture2D picture;
	private String name;
	private String msg;

	public DialogGUI(Texture2D picture, String name, String msg){
		this.picture = picture;
		this.name = name;
		this.msg = msg;
	}

	public void showMessage(){
	}


	public override void draw(){

		GUI.Box(new Rect(0,0, Screen.width, 300));

		GUI.Box(new Rect(50,50,250,250), picture);

		GUI.Box(new Rect (100, 50, Screen.width - 50, 250), msg);
	}
};


