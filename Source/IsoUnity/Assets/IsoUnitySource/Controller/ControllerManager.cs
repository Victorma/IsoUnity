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
	public Option option;
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

		if(enabled && !GUIManager.CaptureEvent()){

			ControllerEventArgs args = new ControllerEventArgs();
			if(Input.simulateMouseWithTouches == false)
				Input.simulateMouseWithTouches = true;
			bool send = false;
			if(Input.GetMouseButtonDown(0) == true){
				RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
				if(hits.Length>0){Heap<float> pqHits = new Heap<float>(hits.Length);
					for(int i = 0; i<hits.Length; i++)
						pqHits.push(i+1, hits[i].distance);

					bool encontrado = false;
					while(!encontrado){
						RaycastHit hit = hits[pqHits.top().elem-1];
						pqHits.pop();
						Cell c = hit.collider.GetComponent<Cell>();
						if(c!=null){
							args.cellTouched = c;
							args.isEntityEvent = false;
							send = true;
							encontrado = true;
						}else{
							Entity e = hit.collider.GetComponent<Entity>();
							Options[] options = e.getOptions();
							if(options.Length > 1)
								GUIManager.drawOptions(Input.mousePosition, options);
							else if (options.Length > 0)
								args.option = options[0];

							encontrado=true;
						}
					}
					if(!encontrado || args.cellTouched != null){
						if(GUIManager.IsDrawingOptions){
							GUIManager.stopDrawingOptions();
						}
					}
				}

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
