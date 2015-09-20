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
	private Dictionary<SecuenceNode, NodeEditor> editors = new Dictionary<SecuenceNode, NodeEditor>();
	
	void nodeWindow(int id)
	{
		SecuenceNode myNode = nodos[id];

		string[] editorNames = NodeEditorFactory.Intance.CurrentNodeEditors;

		int preEditorSelected = NodeEditorFactory.Intance.NodeEditorIndex(myNode);
		int editorSelected = EditorGUILayout.Popup (preEditorSelected, editorNames);
		
		NodeEditor editor = null;
        editors.TryGetValue(myNode, out editor);

		if(editor == null || preEditorSelected!=editorSelected){
			editor = NodeEditorFactory.Intance.createNodeEditorFor (editorNames[editorSelected]);
			editor.useNode (myNode);

			if(!editors.ContainsKey(myNode))	   editors.Add (myNode, editor);
			else    editors[myNode] = editor;
		}
			
		
		editor.draw ();
		
		nodos[id] = editor.Result;	
		
		
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
		BeginWindows();
		nodos.Clear();
		createWindows(nodoInicial);
		EndWindows();
	}
}