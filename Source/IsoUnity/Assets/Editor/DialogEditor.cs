<<<<<<< HEAD
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//TODO esto a su sitio
public class DialogNode {

	private List<DialogNode> childs = new List<DialogNode>();
	private object content = null;
	private string name = "";


	public DialogNode(){

	}

	public DialogNode[] Childs {
		get{ return childs.ToArray() as DialogNode[]; }
	}

	public string Name{
		get{ return name;} 
		set{ name = value;}
	}

	public object Content{
		get{ return content;}
		set{ content = value;}
	}

	public void clearChilds(){
		childs.Clear();
	}

	public DialogNode addNewChild(){
		DialogNode node = new DialogNode();
		this.childs.Add(node);
		return node;
	}

	public void removeChild(int i){
		this.childs.RemoveAt(i);
	}

	public void removeChild(DialogNode child){
		this.childs.Remove(child);
	}


}

public class DialogEditor: EditorWindow{


    [MenuItem("Window/DialogEditor")]
    static void init()
    {
		DialogEditor editor = EditorWindow.GetWindow<DialogEditor>();
		editor.nodoInicial = new DialogNode();
    }

	private DialogNode nodoInicial;
	private Dictionary<int, DialogNode> nodos = new Dictionary<int, DialogNode>();
	private Dictionary<DialogNode, Rect> rects = new Dictionary<DialogNode, Rect>();

	int selected = 0;
	string newParameter = "";
	void nodeWindow(int id)
    {
		DialogNode myNode = nodos[id];

		if(myNode.Content is GameEvent){
			selected = 1;
			GameEvent cont = myNode.Content as GameEvent;
			cont.Name = EditorGUILayout.TextField("Name", cont.Name);
			
			foreach(string param in cont.Params){
				EditorGUILayout.BeginHorizontal();
				cont.setParameter(param, EditorGUILayout.ObjectField(param, (Object)cont.getParameter(param), typeof(Object), true));
				if(GUILayout.Button("X"))
					cont.removeParameter(param);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.BeginHorizontal();
			newParameter = EditorGUILayout.TextField("New Parameter", newParameter);
			if(GUILayout.Button("Add"))
				cont.setParameter(newParameter, null);
			EditorGUILayout.EndHorizontal();
			
			if(Event.current.type != EventType.layout)
				if(myNode.Childs.Length != 1){
					myNode.clearChilds();
					myNode.addNewChild();
					this.Repaint();
				}
		}

		if(myNode.Content == null){
			selected = 0;
			if(Event.current.type != EventType.layout)
				if(myNode.Childs.Length != 0){
					myNode.clearChilds();
					this.Repaint();
				}
		}

		GUIContent[] options = new GUIContent[]{
			new GUIContent("Empty node"),
			new GUIContent("Game event"),
			new GUIContent("Dialog")
		};

		int lastSelected = selected;
		selected = EditorGUILayout.Popup(selected, options);
		if(lastSelected != selected){
			myNode.Content = null;
		}
		if(myNode.Content == null)
			switch(selected){
			case 1: myNode.Content = new GameEvent();	break;
			case 2:	/*myNode.Content = new Dialog();*/		break;
			default: break;
		}
		/*else if(myNode.Content is Dialog){

		*/



		/*Rect lastRect = GUILayoutUtility.GetLastRect();
		Rect myRect = rects[myNode];
		myRect.height = lastRect.y + lastRect.height;
		rects[myNode] = myRect;*/
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
	void createWindows(DialogNode node){
		rects[node] = GUILayout.Window(windowId, rects[node], nodeWindow, node.Name);
		nodos.Add (windowId, node);
		windowId++;

		float altura = 100;
		for(int i = 0; i< node.Childs.Length; i++){
			if(!rects.ContainsKey(node.Childs[i])){
				rects.Add(node.Childs[i], new Rect(rects[node].x + rects[node].width + 15, rects[node].y + i*altura, 150, 0));
			}
			curveFromTo(rects[node], rects[node.Childs[i]], new Color(0.3f,0.7f,0.4f), s);
			createWindows(node.Childs[i]);
		}
	}

	Color s = new Color(0.4f, 0.4f, 0.5f);
    void OnGUI()
    {
		windowId = 0;

		if(!rects.ContainsKey(nodoInicial)){
			rects.Add(nodoInicial, new Rect(10, 10, 150, 0));
		}
		BeginWindows();
		nodos.Clear();
		createWindows(nodoInicial);
        EndWindows();
    }
}
=======
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Dialog))]
public class DialogEditor : Editor {
		public DialogEditor ()
		{
		}

	GUIStyle normalStyle;
	GUIStyle toolBarStyle;
	GUIStyle pressedStyle;
	GUIStyle infoStyle;
	GUIStyle titleStyle;

	private Dialog dialog;
	
	void OnEnable()
	{
		//SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		this.dialog = (Dialog)target;
		
		normalStyle = new GUIStyle();
		normalStyle.padding = new RectOffset(5,5,5,5);
		
		pressedStyle = new GUIStyle();
		pressedStyle.padding = new RectOffset(5,5,5,5);
		GUIStyleState fo = new GUIStyleState();
		
		Texture2D lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
		lineTex.SetPixel(0, 1, Color.Lerp(Color.black, Color.gray, 0.5f));
		lineTex.Apply();
		
		fo.textColor = Color.white;
		
		fo.background = lineTex;
		
		pressedStyle.fontStyle = FontStyle.Bold;
		pressedStyle.alignment = TextAnchor.MiddleCenter;
		pressedStyle.normal = fo;
		
		toolBarStyle = new GUIStyle();
		toolBarStyle.margin = new RectOffset(50,50,5,10);
		
		infoStyle = new GUIStyle();
		infoStyle.margin = new RectOffset(10,10,0,10);
		
		titleStyle = new GUIStyle();
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.margin = new RectOffset(0,0,5,5);
	}

	public override void OnInspectorGUI(){
		
		Event e = Event.current;
		
		dialog = target as Dialog;
		
		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(5,5,5,5);

		
		// Textura
		if(dialog != null){
			dialog.id = UnityEditor.EditorGUILayout.TextField("Name", dialog.id);
			Dialog.Fragment[] fragments = dialog.getFragments();
			Dialog.DialogOption[] options = dialog.getOptions();

			EditorGUILayout.HelpBox("You have to add at least one", MessageType.None);
			if(fragments != null){
				foreach(Dialog.Fragment frg in fragments){
					EditorGUILayout.BeginHorizontal();
					frg.face = EditorGUILayout.ObjectField(frg.face, typeof(Texture2D), true, GUILayout.Width(60),GUILayout.Height(60)) as Texture2D;
					EditorGUILayout.BeginVertical();
					frg.name = EditorGUILayout.TextField(frg.name);
					frg.msg = EditorGUILayout.TextArea(frg.msg,GUILayout.Height(40));
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Reset: ", GUILayout.Width(40));
					frg.reset = EditorGUILayout.Toggle(frg.reset);
					EditorGUILayout.EndHorizontal();


					GUIContent btt = new GUIContent("Remove");
					Rect btr = GUILayoutUtility.GetRect(btt, style);		
					if(GUI.Button(btr,btt)){
						dialog.removeFragment(frg);
					};
					EditorGUILayout.EndVertical();

					EditorGUILayout.EndHorizontal();
				}
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
				foreach(Dialog.DialogOption opt in options){
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Tag: ", GUILayout.Width(27));
					opt.tag = EditorGUILayout.TextField(opt.tag);
					EditorGUILayout.LabelField("Text: ", GUILayout.Width(35));
					opt.text = EditorGUILayout.TextField(opt.text);
					GUIContent btt = new GUIContent("Remove");
					Rect btr = GUILayoutUtility.GetRect(btt, style);		
					if(GUI.Button(btr,btt)){
						dialog.removeOption(opt);
					};
					EditorGUILayout.EndHorizontal();
				}
			}

			bttext = new GUIContent("Add Option");
			btrect = GUILayoutUtility.GetRect(bttext, style);		
			if(GUI.Button(btrect,bttext)){
				dialog.addOption();
			};


		}else{
			EditorGUILayout.LabelField("Please select a texture o create a new one!", style);
		}
		
	}
}

>>>>>>> origin/Dialog-Branch
