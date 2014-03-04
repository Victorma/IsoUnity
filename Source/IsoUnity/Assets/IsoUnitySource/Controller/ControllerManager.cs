using UnityEngine;
using System.Collections;

public class ControllerEventArgs {

	public bool isKeyboardEvent;
	public bool UP;
	public bool DOWN;
	public bool LEFT;
	public bool RIGHT;

	public bool isEntityEvent;
	public Cell cellTouched;
	public Entity entityTouched;
	public string entityActionSelected;
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

	public delegate void ControllerDelegate(ControllerEventArgs args);
	public static ControllerDelegate onControllerEvent;

	public static void tick(){

		if(enabled){
			ControllerEventArgs args = new ControllerEventArgs();
			bool send = false;
			if(Input.GetMouseButtonDown(0) == true){


			}else{
				args.isKeyboardEvent = true;
				float vertical = Input.GetAxisRaw("Vertical");
				float horizontal = Input.GetAxisRaw("Horizontal");
				send = true;
				if(vertical != 0){
					if(vertical>0) args.UP = true;
					else args.DOWN = true;
				}else if(horizontal != 0){
					if(horizontal>0) args.RIGHT = true;
					else args.LEFT = true;
				}else
					send = false;
			}
			if(send)
				onControllerEvent(args);

		}

	}

}
