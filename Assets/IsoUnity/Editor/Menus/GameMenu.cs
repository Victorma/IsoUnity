using UnityEngine;
using UnityEditor;

public class GameMenu {

	[MenuItem("GameObject/IsoUnity/IsoUnity Game", false, 10)]
	public static void createGame(MenuCommand menuCommand){

		GameObject go = new GameObject ("Game");

		go.AddComponent<Game> ();
		go.AddComponent<AnimationManager> ();
		go.AddComponent<SecuenceManager> ();
		go.AddComponent<IsoSwitchesEventManager> ();

		Selection.activeObject = go;    
	}

}
