using UnityEngine;
using System.Collections;

public class OptionsGUI : IsoGUI {

	private Vector2 position = new Vector2();
	private float minRadius = 100;
	private float minDist = 80;

	private float radius;
	private Option[] options;

	public OptionsGUI(Vector2 position, Option[] options){

		this.position = position;
		this.options = options;

		float angle = (2*Mathf.PI)/options.Length;
		float currentDist = (Mathf.Tan (angle/2f)* minRadius)*2;
		this.radius = minRadius;
		if(currentDist < minDist){
			this.radius = minDist / (Mathf.Tan (angle/2f)*2);
		}
	}

	public override void draw(){
		Vector2 pos = GUIUtility.ScreenToGUIPoint(position);
		
		Rect rect = new Rect(pos.x-currentRadius, pos.y-currentRadius, currentRadius*2, currentRadius*2);
		GUI.skin = Resources.Load<GUISkin>("Skin");
		GUI.Box(rect,"");
		
		for(int i = 0; i<number; i++){
			Rect buttonRect= new Rect(pos.x + (radius-minDist/2f)*Mathf.Sin(angle*i) -minDist/2f,pos.y - (radius-minDist/2f)*Mathf.Cos(angle*i)-minDist/2f,minDist,minDist);
			if(Event.current.isMouse&& Event.current.type == EventType.MouseUp){
				if(buttonRect.Contains(GUIUtility.ScreenToGUIPoint(Event.current.mousePosition))){
					ControllerManager.optionChoosen(options[i]);
					GUIManager.removeGUI(this);
				}
			}
			GUI.Button(buttonRect, options[i].Name);
		}
	}

	public override bool captureEvent(ControllerEventArgs args){
		return (Vector2.Distance(Input.mousePosition, this.position) <= this.radius); 
	}
}
