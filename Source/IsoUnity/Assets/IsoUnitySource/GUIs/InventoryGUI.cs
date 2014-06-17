using UnityEngine;
using System.Collections.Generic;

public class InventoryGUI : IsoGUI {

	private Inventory inventory;
	public void init(Inventory inventory){
		this.inventory = inventory;
	}

	private Option[] OptionsToCreate;
	private OptionsGUI optionsGUI = null;
	private Vector2 scroll = new Vector2(0,0);
	public override void draw ()
	{
		float anchoGUI = Screen.width,
			altoGUI = Screen.height;
			GUI.BeginGroup(new Rect (0, 0, anchoGUI, altoGUI), "Inventory");
			GUI.Box(new Rect(0,0, anchoGUI, altoGUI),"");
			GUIStyle style = new GUIStyle ();
			style.padding = new RectOffset(15,15,15,15);

			GUI.BeginGroup (new Rect (0, 0, anchoGUI, 150));
				GUI.Label (new Rect(0,0,anchoGUI, 150), "Inventory");
			GUI.EndGroup ();

			// Filtramos los item
			Dictionary<Item, int> cantidades = new Dictionary<Item, int> ();
			foreach(Item i in this.inventory.Items){
				bool encontrado = false;
				foreach(Item clave in cantidades.Keys){
					if(clave.isEqualThan(i)){
						cantidades[clave]++;
						encontrado = true;
						break;
					}
				}
				if(!encontrado)
					cantidades.Add(i, 1);
			}
			


			float itemYPos = 0;
			float itemHeight = 50;
			float anchoScroll = anchoGUI-40;
			scroll = GUI.BeginScrollView(new Rect(10,50,anchoGUI-20, altoGUI- 150),scroll,
			                             new Rect(0, 0, anchoScroll, itemHeight*cantidades.Count));
			foreach(Item i in cantidades.Keys){
			//Cada item
			if(GUI.Button(new Rect(0,itemYPos, anchoScroll, itemHeight), "", GUIStyle.none) && optionsGUI == null){
				GameEvent use = ScriptableObject.CreateInstance<GameEvent>();
				use.Name = "use item";
				use.setParameter("inventory", this.inventory);
				use.setParameter("item", i);
				GameEvent remove = ScriptableObject.CreateInstance<GameEvent>();
				remove.Name = "remove item";
				remove.setParameter("inventory", this.inventory);
				remove.setParameter("item", i);
				OptionsToCreate = new Option[]{new Option("Use", use,false,0), new Option("Remove", remove,false,0)};
			}

			GUI.BeginGroup(new Rect(0,itemYPos, anchoScroll, itemHeight));
				GUI.Box(new Rect(0,0, anchoScroll, itemHeight),"");
				GUI.Box(new Rect(0,0, itemHeight, itemHeight),"");
//				GUIStyle bgImage = new GUIStyle ();
				style.normal.background = i.Image;
				GUI.Box(new Rect(1,1, itemHeight-2, itemHeight-2),"", style);

				float quantityLabel = 0; 
				//Dependiendo de la cantidad pintaremos la cantidad o no
				if(cantidades[i]>1){
					quantityLabel = 50;
					GUI.BeginGroup(new Rect(anchoScroll-quantityLabel, 0, quantityLabel, itemHeight));
						GUI.Box(new Rect(0,0,quantityLabel, itemHeight),"");
						GUI.Label(new Rect(quantityLabel/2f, itemHeight/2f, quantityLabel, itemHeight), ""+cantidades[i]);
				    GUI.EndGroup();
				}
				GUI.Label(new Rect(itemHeight, 0, anchoScroll- itemHeight-quantityLabel, itemHeight), i.Name+": "+ i.Description);
				itemYPos+=itemHeight;
			GUI.EndGroup();
			}

			GUI.EndScrollView();
			if (GUI.Button (new Rect(0, altoGUI-100, anchoGUI, 100), "Close")){
				GUIManager.removeGUI (this);
				ScriptableObject.Destroy(this);
			}
		GUI.EndGroup();
	}

	public override void fillControllerEvent (ControllerEventArgs args)
	{
		if(OptionsToCreate!=null){
			optionsGUI = ScriptableObject.CreateInstance<OptionsGUI>();
			optionsGUI.init(args, args.mousePos, OptionsToCreate);
			OptionsToCreate = null;
			GUIManager.addGUI(optionsGUI,100);
		}
		if(args.options != null)
			Debug.Log("he recibido opciones");
	}

}
