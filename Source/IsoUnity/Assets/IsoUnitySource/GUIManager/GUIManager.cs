using UnityEngine;
using System.Collections;

public class GUIManager {

	private static bool drawingOptions = false;
	public static bool IsDrawingOptions{
		get{
			return drawingOptions;
		}
	}
	private static Vector2 position = new Vector2();
	private static float minRadius = 100;
	private static float minDist = 80;
	private static int number;

	public static void tick(){

		/*foreach(IsoUnityGUI gui in guis){
			gui.draw();
		}*/

		if(drawingOptions){
			Vector2 pos = GUIUtility.ScreenToGUIPoint(position);

			float angle = (2*Mathf.PI)/number;
			float currentDist = (Mathf.Tan (angle/2f)* minRadius)*2;
			float currentRadius = minRadius;
			if(currentDist < minDist){
				currentRadius = minDist / (Mathf.Tan (angle/2f)*2);
			}

			Rect rect = new Rect(pos.x-currentRadius, pos.y-currentRadius, currentRadius*2, currentRadius*2);
			GUI.skin = Resources.Load<GUISkin>("Skin");
			GUI.Box(rect,"");

			for(int i = 0; i<number; i++){
				Rect buttonRect= new Rect(pos.x + (currentRadius-minDist/2f)*Mathf.Sin(angle*i) -minDist/2f,pos.y - (currentRadius-minDist/2f)*Mathf.Cos(angle*i)-minDist/2f,minDist,minDist);
				if(Event.current.isMouse&& Event.current.type == EventType.MouseUp){
					if(buttonRect.Contains(GUIUtility.ScreenToGUIPoint(Event.current.mousePosition)))//TODO Return option selected
						stopDrawingOptions();
				}
				GUI.Button(buttonRect, i+"");
			}
			//GUI.Button(rect,"");
			//GUILayout.EndArea();
		}
	}

	public static void drawOptions(Vector2 position, object[] options){

		GUIManager.drawingOptions = true;
		GUIManager.position = position;
		number = options.Length;
	}

	public static void stopDrawingOptions(){
		GUIManager.drawingOptions = false;
	}
}
