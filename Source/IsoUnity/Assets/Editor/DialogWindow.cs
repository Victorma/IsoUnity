using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SecuenceWindow: EditorWindow{
	
	private Secuence secuence;

	public Secuence Secuence {
		get { return secuence; }
		set { this.secuence = value; }
	}
	private Dictionary<int, SecuenceNode> nodos = new Dictionary<int, SecuenceNode>();
	private Dictionary<SecuenceNode, Rect> rects = new Dictionary<SecuenceNode, Rect>();
	private Dictionary<SecuenceNode, Vector2> scrolls = new Dictionary<SecuenceNode, Vector2>();

	string newParameter = "";
	void nodeWindow(int id)
	{
		SecuenceNode myNode = nodos[id];

		int selected = 0;
		if (myNode.Content is GameEvent) {
			selected = 1;

			GameEvent ge = (GameEvent)myNode.Content;
			string[] editors = EventEditorFactory.Intance.CurrentEventEditors;
			int editorSelected = 0;
			if(ge.Name == null)
				ge.Name = "";
			for (int i = 1; i< editors.Length; i++)
				if (editors [i].ToLower () == ge.Name.ToLower ())
					editorSelected = i;
			
			editorSelected = EditorGUILayout.Popup (editorSelected, EventEditorFactory.Intance.CurrentEventEditors);
			EventEditor editor = EventEditorFactory.Intance.createEventEditorFor (editors[editorSelected]);
			editor.useEvent (ge);		
			
			editor.draw ();
			
			myNode.Content = editor.Result;

			if (Event.current.type != EventType.layout)
				if (myNode.Childs.Length != 1) {
					myNode.clearChilds ();
					myNode.addNewChild ();
					this.Repaint ();
				}
		} else if (myNode.Content is Dialog) {
			selected = 2;
			Event e = Event.current;
			
			Dialog dialog = myNode.Content as Dialog;
			
			GUIStyle style = new GUIStyle();
			style.padding = new RectOffset(5,5,5,5);

			dialog.id = UnityEditor.EditorGUILayout.TextField("Name", dialog.id);
			Dialog.Fragment[] fragments = dialog.getFragments();
			Dialog.DialogOption[] options = dialog.getOptions();
			
			EditorGUILayout.HelpBox("You have to add at least one", MessageType.None);
			bool infoShown = false;
			if(fragments != null){
				bool isScrolling = false;
				if(fragments.Length > 3){
					scrolls[myNode] = EditorGUILayout.BeginScrollView(scrolls[myNode], GUILayout.ExpandWidth(true), GUILayout.Height(250));
					isScrolling = true;
				}
				foreach(Dialog.Fragment frg in fragments){
					EditorGUILayout.BeginHorizontal();
					frg.IsEntityFragment = EditorGUILayout.Toggle("Is entity: ", frg.IsEntityFragment);
					bool showInfo = false;
					if(frg.IsEntityFragment){
						frg.Entity = (Entity)EditorGUILayout.ObjectField(frg.Entity, typeof(Entity), true);
						showInfo = true;
					}
					EditorGUILayout.EndHorizontal();
					if(showInfo){
						if(!infoShown)
							EditorGUILayout.HelpBox("Empty face or name will show entity's default face or name.", MessageType.Info);
						infoShown = true;
					}
					EditorGUILayout.BeginHorizontal();
					frg.Face = EditorGUILayout.ObjectField(frg.Face, typeof(Texture2D), true, GUILayout.Width(50),GUILayout.Height(50)) as Texture2D;
					EditorGUILayout.BeginVertical();
					frg.Name = EditorGUILayout.TextField(frg.Name);
					frg.Msg = EditorGUILayout.TextArea(frg.Msg,GUILayout.Height(40));
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					
					/*EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Reset: ", GUILayout.Width(40));
					frg.reset = EditorGUILayout.Toggle(frg.reset);
					EditorGUILayout.EndHorizontal();*/
					
					
					GUIContent btt = new GUIContent("Remove");
					Rect btr = GUILayoutUtility.GetRect(btt, style);		
					if(GUI.Button(btr,btt)){
						dialog.removeFragment(frg);
					};
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();

				}
				if(isScrolling)
					EditorGUILayout.EndScrollView();
			}
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EndHorizontal();
			
			GUIContent bttext = new GUIContent("Add Fragment");
			Rect btrect = GUILayoutUtility.GetRect(bttext, style);		
			if(GUI.Button(btrect,bttext)){
				dialog.addFragment();
			};
			
			EditorGUILayout.HelpBox("Options are the lines between you have to choose at the end of the dialog. Leave empty to do nothing, put one to execute this as the dialog ends, or put more than one to let the player choose between them.", MessageType.None);
			if(options != null){
				int i = 0;
				foreach(Dialog.DialogOption opt in options){
					EditorGUILayout.BeginHorizontal();
					//EditorGUILayout.LabelField("Tag: ", GUILayout.Width(27));
					//opt.tag = EditorGUILayout.TextField(opt.tag);
					EditorGUILayout.LabelField("Text: ", GUILayout.Width(35));
					opt.text = EditorGUILayout.TextField(opt.text);
					GUIContent btt = new GUIContent("Remove");
					Rect btr = GUILayoutUtility.GetRect(btt, style);		
					if(GUI.Button(btr,btt)){
						dialog.removeOption(opt);
						if(myNode.Childs.Length > 1)
							myNode.removeChild(i);
					};
					myNode.Childs[i].Name = "Option "+(i+1);
					EditorGUILayout.EndHorizontal();
					i++;
				}
			}
			
			bttext = new GUIContent("Add Option");
			btrect = GUILayoutUtility.GetRect(bttext, style);		
			if(GUI.Button(btrect,bttext)){
				dialog.addOption();
				if(myNode.Childs.Length < dialog.getOptions().Length)
					myNode.addNewChild();
			};

			if (Event.current.type != EventType.layout)
				if (myNode.Childs.Length < 1) {
					myNode.addNewChild ();
					this.Repaint ();
				}

		} else if (myNode.Content is Fork) {
			selected = 3;
			
			Fork fork = myNode.Content as Fork;

			string[] forktypes = fork.getTypes();
			
			fork.setForkType((Fork.ForkTypes) EditorGUILayout.Popup ((int)fork.getForkType(), forktypes));

			EditorGUILayout.BeginHorizontal();
			switch(fork.getForkType()){
				case Fork.ForkTypes.Switch:
				EditorGUILayout.LabelField("SwitchID: ", GUILayout.Width(35));
				fork.switchID = EditorGUILayout.TextField(fork.switchID);
				fork.switchstate = EditorGUILayout.Toggle("SwitchState: ", fork.switchstate);
				break;
				case Fork.ForkTypes.Item:
				break;
				case Fork.ForkTypes.GameTime:
				break;
			}
			EditorGUILayout.EndHorizontal();
			
			if (myNode.Childs.Length != 2) {
				myNode.clearChilds ();
				myNode.addNewChild ();
				myNode.addNewChild ();
				myNode.Childs[0].Name = "Case fork True";
				myNode.Childs[1].Name = "Case fork False";
				this.Repaint ();
			}
		}

		if(myNode.Content == null){
			selected = 0;
			if(Event.current.type != EventType.layout)
			if(myNode.Childs.Length != 0 && Application.isEditor){
				myNode.clearChilds();
				this.Repaint();
			}
		}
		
		GUIContent[] optionsPopup = new GUIContent[]{
			new GUIContent("Empty node"),
			new GUIContent("Game event"),
			new GUIContent("Dialog"),
			new GUIContent("Fork")
		};
		
		int lastSelected = selected;
		selected = EditorGUILayout.Popup(selected, optionsPopup);
		if(lastSelected != selected){
			myNode.Content = null;
		}
		if(myNode.Content == null)
		switch(selected){
			case 1: myNode.Content = ScriptableObject.CreateInstance<GameEvent>();	break;
			case 2:	myNode.Content = new Dialog(); break;
			case 3:	myNode.Content = new Fork(); break;
			default: break;
		}		
		
		
		if (Event.current.type != EventType.layout) {
			Rect lastRect = GUILayoutUtility.GetLastRect ();
			Rect myRect = rects [myNode];
			myRect.height = lastRect.y + lastRect.height;
			rects [myNode] = myRect;
			this.Repaint();
		}
		GUI.DragWindow();
	}
	void curveFromTo(Rect wr, Rect wr2, Color color, Color shadow)
	{
		Drawing.bezierLine(
			new Vector2(wr.x + wr.width, wr.y + 3 + wr.height / 2),
			new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + 3 + wr.height / 2),
			new Vector2(wr2.x, wr2.y + 3 + wr2.height / 2),
			new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + 3 + wr2.height / 2), shadow, 5, true,20);
		Drawing.bezierLine(
			new Vector2(wr.x + wr.width, wr.y + wr.height / 2),
			new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + wr.height / 2),
			new Vector2(wr2.x, wr2.y + wr2.height / 2),
			new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + wr2.height / 2), color, 2, true,20);
	}
	
	private int windowId;
	void createWindows(SecuenceNode node){
		rects[node] = GUILayout.Window(windowId, rects[node], nodeWindow, node.Name, GUILayout.MinWidth(300));
		nodos.Add (windowId, node);
		windowId++;
		
		float altura = 100;
		for(int i = 0; i< node.Childs.Length; i++){
			if(!rects.ContainsKey(node.Childs[i]))
				rects.Add(node.Childs[i], new Rect(rects[node].x + /*rects[node].width*/ + 315, rects[node].y + i*altura, 150, 0));
			if(!scrolls.ContainsKey(node.Childs[i]))
				scrolls.Add (node.Childs[i], new Vector2(0,0));
			curveFromTo(rects[node], rects[node.Childs[i]], new Color(0.3f,0.7f,0.4f), s);
			createWindows(node.Childs[i]);
		}
	}
	
	Color s = new Color(0.4f, 0.4f, 0.5f);
	void OnGUI()
	{
		windowId = 0;
		SecuenceNode nodoInicial = secuence.Root;
		if(!rects.ContainsKey(nodoInicial))
			rects.Add(nodoInicial, new Rect(10, 10, 300, 0));
		if(!scrolls.ContainsKey(nodoInicial))
			scrolls.Add (nodoInicial, new Vector2(0,0));
		BeginWindows();
		nodos.Clear();
		createWindows(nodoInicial);
		EndWindows();
	}
}