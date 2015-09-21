using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SecuenceWindow: EditorWindow{
	
	private Secuence secuence;

	public Secuence Secuence {
		get { return secuence; }
		set { this.secuence = value; }
	}

    private Dictionary<int, SecuenceNode> nodes = new Dictionary<int, SecuenceNode>();
	private Dictionary<SecuenceNode, NodeEditor> editors = new Dictionary<SecuenceNode, NodeEditor>();
	
	void nodeWindow(int id)
	{
		SecuenceNode myNode = nodes[id];

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

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
		editor.draw ();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        int i = 1;
        foreach (var c in myNode.Childs)
        {
            var n = i +""; 
            if (c != null) n = c.Name;
            GUILayout.Button(n);
            i++;
        }
        GUILayout.FlexibleSpace();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
		
		nodes[id] = editor.Result;	
		
		
		if (Event.current.type != EventType.layout) {
			Rect lastRect = GUILayoutUtility.GetLastRect ();
			Rect myRect = secuence.getRectFor(myNode);
			myRect.height = lastRect.y + lastRect.height;
            secuence.setRectFor(myNode, myRect);
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

    /**
     *  Rectangle backup code calculation
     **
     
        if(!rects.ContainsKey(node.Childs[i]))
			rects.Add(node.Childs[i], new Rect(rects[node].x + 315, rects[node].y + i*altura, 150, 0));
		curveFromTo(rects[node], rects[node.Childs[i]], new Color(0.3f,0.7f,0.4f), s);
     
     */
	
	void createWindows(Secuence secuence){
		float altura = 100;
		foreach(var node in secuence.Nodes){
            nodes.Add(node.GetInstanceID(), node);
            secuence.setRectFor(node, GUILayout.Window(node.GetInstanceID(), secuence.getRectFor(node), nodeWindow, node.Name, GUILayout.MinWidth(300)));
		}
	}
	
	Color s = new Color(0.4f, 0.4f, 0.5f);
	void OnGUI()
	{
		SecuenceNode nodoInicial = secuence.Root;
        GUILayout.BeginVertical(GUILayout.Height(20));
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("New Node")){
            secuence.createChild();
        }

        if(GUILayout.Button("Set Root")){

        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

		BeginWindows();
        nodes.Clear();
		createWindows(secuence);
		EndWindows();
	}
}