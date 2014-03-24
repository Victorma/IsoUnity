using UnityEngine;
using System.Collections;

public class OptionsGUI : IsoGUI {

	private Vector2 position = new Vector2();
	private float minRadius = 100;
	private float minDist = 80;

	private float radius;
	private float angle;
	private Option[] options;
	private Option[] returningOption;

	public OptionsGUI(Vector2 position, Option[] options){

		this.position = position;
		this.options = options;

		angle = (2f*Mathf.PI)/((float)options.Length);
		float currentDist = (Mathf.Sin (angle/2f)* (minRadius-(minDist/2f)))*2;
		this.radius = minRadius;
		if(angle < Mathf.PI && currentDist < minDist){
			this.radius = ((minDist/2f) / Mathf.Sin (angle/2f)) + minDist/2f;
		}
	}
	private bool sended = false;
	public override void fillControllerEvent (ControllerEventArgs args)
	{
		if(returningOption != null && !sended){
			args.options = returningOption;
			args.send = true;
			GUIManager.removeGUI(this);
			sended = true;
		}else
			args.send = false;
	}

	public override void draw(){
		if(!survive){
			GUIManager.removeGUI(this);
			return;
		}

		Vector2 pos = GUIUtility.ScreenToGUIPoint(position);
		
		Rect rect = new Rect(pos.x-radius, pos.y-radius, radius*2f, radius*2f);
		GUI.skin = Resources.Load<GUISkin>("Skin");
		GUI.Box(rect,"");
		
		for(int i = 0; i<options.Length; i++){
			Rect buttonRect = new Rect(pos.x + (radius-minDist/2f)*Mathf.Sin(angle*i) - minDist/2f, pos.y - (radius-minDist/2f)*Mathf.Cos(angle*i)-minDist/2f, minDist, minDist);
			if(Event.current.isMouse&& Event.current.type == EventType.MouseUp){
				if(buttonRect.Contains(GUIUtility.ScreenToGUIPoint(Event.current.mousePosition))){
					returningOption = new Option[]{options[i]};
				}
			}
			GUI.Button(buttonRect, options[i].Name);
		}
	}
	private bool survive = true;
	public override bool captureEvent(ControllerEventArgs args){
		bool enRango = (Vector2.Distance(args.mousePos, this.position) <= this.radius);
		if(!enRango && args.leftStatus == true)
			survive = false;

		return enRango; 
	}
}
