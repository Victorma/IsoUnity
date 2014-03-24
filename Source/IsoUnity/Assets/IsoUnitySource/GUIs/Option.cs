using UnityEngine;

public class Option {

	public Option(string name, GameEvent action){ init(name, null, action, true); }
	public Option(Texture2D image, GameEvent action){ init(null, image, action, true); }
	public Option(string name, GameEvent action, bool hasToMove){ init(name, null, action, hasToMove); }
	public Option(Texture2D image, GameEvent action, bool hasToMove){ init(null, image, action, hasToMove); }
	public Option(string name, Texture2D image, GameEvent action, bool hasToMove) { init(name, image, action, hasToMove); }


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

	private bool hasToMove;
	public bool HasToMove{
		get{return hasToMove;}
	}

	private void init(string name, Texture2D image, GameEvent action, bool hasToMove){
		this.name = name;
		this.image = image;
		this.action = action;
		this.hasToMove = hasToMove;
	}
}
