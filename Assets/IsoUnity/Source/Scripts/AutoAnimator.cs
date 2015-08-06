using UnityEngine;
using System.Collections;

public class AutoAnimator : MonoBehaviour {

	
	public int[] FrameSecuence;
	public float FrameRate;

	public int Repeat = 0;
	public bool AutoDestroy = false;

	private int currentpos = 0;
	private int currentloop = 0;
	private float currenttime = 0f;

	private Decoration dec;

	private GameEvent ev;

	// Use this for initialization
	void Start () {
		dec = this.GetComponent<Decoration> ();
	}
	
	// Update is called once per frame
	void Update () {
		currenttime+=Time.deltaTime;


		if (currenttime > this.FrameRate) {
			currenttime-=this.FrameRate;

			if(this.FrameSecuence.Length==0)
				if((dec.Tile+1)<(dec.IsoDec.nCols*dec.IsoDec.nRows))
					dec.Tile++;
				else{
					dec.Tile=0;
					if(this.AutoDestroy)
						this.currentloop++;
				}
			else{
				if((currentpos+1)<this.FrameSecuence.Length)
					currentpos++; 
				else{
					currentpos=0;
					if(this.AutoDestroy)
						this.currentloop++;
				}

				dec.Tile =this.FrameSecuence[currentpos];
			}

			if(this.AutoDestroy)
				if(this.currentloop>=this.Repeat){
					if(ev!=null)
						Game.main.eventFinished(ev);
					GameObject.Destroy(this.gameObject);
				}
		}
	}

	public void registerEvent(GameEvent ev){
		this.ev = ev;
	}
}
