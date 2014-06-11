using UnityEngine;
using System.Collections.Generic;

public class InventoryGUI : IsoGUI {

	private Inventory inventory;
	public InventoryGUI(Inventory inventory){
		this.inventory = inventory;
		GameEvent g1 = new GameEvent(),
				g2 = new GameEvent(),
				g3 = new GameEvent(),
				g4 = new GameEvent(),
				g5 = new GameEvent();
		g1.Name = "add item";
		g1.setParameter("Item", new Hierbajo());
		g1.setParameter("Inventory", inventory);

		g2.Name = "add item";
		g2.setParameter("Item", new Hierbajo());
		g2.setParameter("Inventory", inventory);

		g3.Name = "add item";
		g3.setParameter("Item", new Hierbajo());
		g3.setParameter("Inventory", inventory);

		g4.Name = "add item";
		g4.setParameter("Item", new Hierbajo());
		g4.setParameter("Inventory", inventory);

		g5.Name = "add item";
		g5.setParameter("Item", new Hierbajo());
		g5.setParameter("Inventory", inventory);

		Game.main.enqueueEvent(g1);
		Game.main.enqueueEvent(g2);
		Game.main.enqueueEvent(g3);
		Game.main.enqueueEvent(g4);
		Game.main.enqueueEvent(g5);
	}

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
			if (GUI.Button (new Rect(0, altoGUI-100, anchoGUI, 100), "Close"))
				GUIManager.removeGUI (this);
		GUI.EndGroup();
	}

	public override void fillControllerEvent (ControllerEventArgs args)
	{

	}

	private class Hierbajo : Item {
		public override string Description {
			get {
				return "Un hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonicoUn hierbajo mu bonico";
			}
			set {}
		}

		public override string Name {
			get {
				return "Hierbajo";
			}
			set {}
		}

		public override Texture2D Image {
			get {
				return Resources.Load<Texture2D>("grassImage");
			}
			set {}
		}
		public override IsoDecoration Representation {
			get {
				return Resources.Load<IsoDecoration>("Grass");
			}
			set {}
		}
		public override bool isEqualThan (Item other)
		{
			return other is Hierbajo;
		}
		public override void use (){}
	}

}
