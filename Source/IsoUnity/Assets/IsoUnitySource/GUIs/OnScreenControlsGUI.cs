using UnityEngine;
using System.Collections;

public class OnScreenControlsGUI : IsoGUI {
	

	bool upPressed = false;
	bool downPressed = false;
	bool rightPressed = false;
	bool leftPressed = false;
	
	public OnScreenControlsGUI(){

	}


	public override void fillControllerEvent (ControllerEventArgs args)
	{
		args.UP |= upPressed;
		args.DOWN |= downPressed;
		args.RIGHT |= rightPressed;
		args.LEFT |= leftPressed;
		args.send = true;
	}
	
	public override void draw(){

		float targetWidth = 1080f;
		float scale = Screen.width / targetWidth;

		//Rect leftButtonsArea = new Rect(20*scale, Screen.height-300*scale, 300*scale, 300*scale);

		GUIStyle style = Resources.Load<GUISkin>("Skin").FindStyle("OnScreenButton");
			Vector2 point = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);

		Rect buttonUp = new Rect(100*scale, Screen.height-300*scale, 100*scale, 100*scale);
		upPressed = buttonUp.Contains(point);

		Rect buttonDown = new Rect(100*scale,Screen.height-300*scale + 200*scale, 100*scale, 100*scale);
		downPressed = buttonDown.Contains(point);

		Rect buttonLeft = new Rect(0,Screen.height-300*scale + 100*scale, 100*scale, 100*scale);
		leftPressed = buttonLeft.Contains(point);
			

		Rect buttonRight = new Rect(200*scale, Screen.height-300*scale +100*scale, 100*scale, 100*scale);

		rightPressed = buttonRight.Contains(point);
		if(Event.current.type == EventType.repaint){

			style.Draw(buttonUp, "U", true, upPressed | backupUp,  true, true);
			style.Draw(buttonDown, "D",true, downPressed | backupDown, true, true);
			style.Draw(buttonLeft, "L",true, leftPressed | backupLeft, true, true);
			style.Draw(buttonRight, "R", true,rightPressed | backupRight, true, true);
		}

		if((upPressed || leftPressed || downPressed || rightPressed) && Event.current.type!= EventType.repaint )
			Event.current.Use();
	}

	bool backupUp, backupLeft, backupDown, backupRight;

	private bool survive = true;
	public override bool captureEvent(ControllerEventArgs args){
		bool was = upPressed || leftPressed || downPressed || rightPressed;

		backupUp = args.UP;
		backupLeft = args.LEFT;
		backupDown = args.DOWN;
		backupRight = args.RIGHT;
		
		return was; 
	}
}
