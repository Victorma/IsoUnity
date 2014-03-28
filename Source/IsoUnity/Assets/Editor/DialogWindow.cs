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

public class DialogWindow: EditorWindow{
	
	
	[MenuItem("Window/DialogWindow")]
	static void init()
	{
		DialogWindow editor = EditorWindow.GetWindow<DialogWindow>();
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