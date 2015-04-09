using UnityEngine;

public class Option {

	public Option(string name, GameEvent action, int distance){ init(name, null, action, true, distance); }
	public Option(Texture2D image, GameEvent action, int distance){ init(null, image, action, true, distance); }
	public Option(string name, GameEvent action, bool hasToMove, int distance){ init(name, null, action, hasToMove, distance); }
	public Option(Texture2D image, GameEvent action, bool hasToMove, int distance){ init(null, image, action, hasToMove, distance); }
	public Option(string name, Texture2D image, GameEvent action, bool hasToMove, int distance) { init(name, image, action, hasToMove, distance); }


	private string name;
	public string Name{
		get{return name;}
	}

	private Texture2D image;
	public Texture2D Image{
		get{return image;}
	}

	private GameEvent action;
	public GameEvent Action{
		get{return action;}
	}

	private int distance;
	public int Distance{
		get{return distance;}
	}

	private bool hasToMove;
	public bool HasToMove{
		get{return hasToMove;}
	}

	private void init(string name, Texture2D image, GameEvent action, bool hasToMove, int distance){
		this.name = name;
		this.image = image;
		this.action = action;
		this.hasToMove = hasToMove;
		this.distance = distance;
	}

	private void init(string name, Texture2D image, GameEvent action, bool hasToMove){
		this.init(name,image,action,hasToMove,0);
	}
}
