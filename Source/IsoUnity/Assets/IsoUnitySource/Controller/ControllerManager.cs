using UnityEngine;
using System.Collections;

public class ControllerEventArgs {

	public Vector2 mousePos;
	public bool leftStatus;
	public bool isLeftUp = false;
	public bool isLeftDown = false;
	
	public bool UP = false;
	public bool DOWN = false;
	public bool LEFT = false;
	public bool RIGHT = false;
	
	public Cell cell;
	public Entity entity;
	public Option[] options;

	public bool send = false;
}

public class ControllerManager  {

	private static bool enabled = false;
	public static bool Enabled{
		get{
			return enabled;
		}
		set{
			enabled = value;
		}
	}

	private static bool left = false;

	private static void insertMouseConditions(ControllerEventArgs args){

		args.mousePos = Input.mousePosition;
		args.leftStatus = Input.GetMouseButton(0);
		args.isLeftDown = left == false && args.leftStatus == true;
		args.isLeftUp = left == true && args.leftStatus == false;

		left = args.leftStatus;
	}

	private static void insertKeyboardConditions(ControllerEventArgs args){

		float vertical = Input.GetAxisRaw("Vertical"),
			horizontal = Input.GetAxisRaw("Horizontal");

		if(vertical != 0){
			if(vertical>0) args.UP = true;
			else args.DOWN = true;
		}

		if(horizontal != 0){
			if(horizontal>0) args.RIGHT = true;
			else args.LEFT = true;
		}
	}


	public delegate void ControllerDelegate(ControllerEventArgs args);
	public static ControllerDelegate onControllerEvent;

	public static void tick(){

		/**
		 * -Evento de control
			-> Controllador:
				-> Iniciamos un nuevo ControllerEventArgs
				-> 
				-> Recopila: 
					-> Posicion y estado del raton
					-> Posicion y estado del teclado
				-> Pregunta GUI si quiere el evento
			 		-> Si la GUI no lo captura
						-> Le da el evento al mapa para que:
							-> Detecte la celda
							-> Detecte la entidad
							-> Detecte las opciones
					-> Si la GUI lo captura
						-> Le da el evento a la GUI para que lo termine.
				-> Si el evento se tiene que enviar
					-> Se manda el nuevo evento.
		*/

		if(enabled){

			//Tactil = raton
			if(Input.simulateMouseWithTouches == false)
				Input.simulateMouseWithTouches = true;

			ControllerEventArgs args = new ControllerEventArgs();

			// Recopilamos estado
			insertMouseConditions(args);
			insertKeyboardConditions(args);


			//Preguntamos a la GUI.
			IsoGUI gui = GUIManager.getGUICapturing(args);

			if(gui == null)	MapManager.getInstance().fillControllerEvent(args);
			else 			gui.fillControllerEvent(args);

			if(args.send)
				onControllerEvent(args);

		}
	}
}
