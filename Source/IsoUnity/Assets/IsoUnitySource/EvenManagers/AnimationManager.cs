using UnityEngine;
using System.Collections;

public class AnimationManager : EventManager {

	public override void ReceiveEvent (GameEvent ev)
	{
		if(ev.Name == "ShowAnimation"){
			Decoration dec = (ev.getParameter("Objetive") as GameObject).GetComponent<Decoration>();
			GameObject animation = (GameObject)ev.getParameter("Animation");

			GameObject go = (GameObject) GameObject.Instantiate(animation);

			Decoration animation2 = go.GetComponent<Decoration>();

			animation2.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
			animation2.Father = dec;
			animation2.adaptate();

			AutoAnimator anim = go.GetComponent<AutoAnimator>();
			anim.registerEvent(ev);
		}
	}

	public override void Tick(){}
	
}
