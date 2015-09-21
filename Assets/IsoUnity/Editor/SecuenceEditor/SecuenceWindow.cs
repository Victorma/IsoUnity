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
    private GUIStyle closeStyle, collapseStyle;

    private int hovering = -1;
    private int focusing = -1;

    private int lookingChildSlot;
    private SecuenceNode lookingChildNode;
	
	void nodeWindow(int id)
	{
		SecuenceNode myNode = nodes[id];

        if (myNode.Collapsed)
        {
            if (GUILayout.Button("Open"))
                myNode.Collapsed = false;
        }
        else
        {

            string[] editorNames = NodeEditorFactory.Intance.CurrentNodeEditors;

            int preEditorSelected = NodeEditorFactory.Intance.NodeEditorIndex(myNode);
            GUILayout.BeginHorizontal();
            int editorSelected = EditorGUILayout.Popup(preEditorSelected, editorNames);

            if (GUILayout.Button("-", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
                myNode.Collapsed = true;
            if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                secuence.removeChild(myNode);
            GUILayout.EndHorizontal();

            NodeEditor editor = null;
            editors.TryGetValue(myNode, out editor);

            if (editor == null || preEditorSelected != editorSelected)
            {
                editor = NodeEditorFactory.Intance.createNodeEditorFor(editorNames[editorSelected]);
                editor.useNode(myNode);

                if (!editors.ContainsKey(myNode)) editors.Add(myNode, editor);
                else editors[myNode] = editor;
            }

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            editor.draw();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            int i = 0;
            foreach (var c in myNode.Childs)
            {
                var n = (i+1) + "";
                if (c != null) n = c.Name;
                if (GUILayout.Button(n))
                {
                    // Detach
                    myNode.Childs[i] = null;

                    // Start search
                    lookingChildNode = myNode;
                    lookingChildSlot = i;
                }
                i++;
            }
            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            nodes[id] = editor.Result;	
        }

		
		
		if (Event.current.type != EventType.layout) {
			Rect lastRect = GUILayoutUtility.GetLastRect ();
            Rect myRect = myNode.Position;
			myRect.height = lastRect.y + lastRect.height;
            myNode.Position = myRect;
			this.Repaint();
        }

        if (Event.current.type == EventType.mouseMove)
        {
            if (new Rect(0, 0, myNode.Position.width, myNode.Position.height).Contains(Event.current.mousePosition))
            {
                hovering = id;
            }
        }

        if (Event.current.type == EventType.mouseDown)
        {
            if (hovering == id) focusing = hovering;
            if (lookingChildNode != null)
            {
                // link creation between nodes
                lookingChildNode.Childs[lookingChildSlot] = myNode;
                // finishing search
                lookingChildNode = null;
            }
        }

		GUI.DragWindow();
	}
	void curveFromTo(Rect wr, Rect wr2, Color color, Color shadow)
	{
        Vector2 start = new Vector2(wr.x + wr.width, wr.y + 3 + wr.height / 2),
            startTangent = new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + 3 + wr.height / 2),
            end = new Vector2(wr2.x, wr2.y + 3 + wr2.height / 2),
            endTangent = new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + 3 + wr2.height / 2);

        Handles.BeginGUI();
        Handles.color = color;
        Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 3);
        Handles.EndGUI();

		/*Drawing.bezierLine(
			,
			,
			new Vector2(wr2.x, wr2.y + 3 + wr2.height / 2),
			, shadow, 5, true,20);
		Drawing.bezierLine(
			new Vector2(wr.x + wr.width, wr.y + wr.height / 2),
			new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + wr.height / 2),
			new Vector2(wr2.x, wr2.y + wr2.height / 2),
			new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + wr2.height / 2), color, 2, true,20);*/
	}

    private Rect sumRect(Rect r1, Rect r2)
    {
        return new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
    }

    private Dictionary<SecuenceNode, bool> loopCheck = new Dictionary<SecuenceNode, bool>();

    void drawLines(Rect from, SecuenceNode to)
    {
        if (to == null)
            return;

        // Visible loop line
        curveFromTo(from, to.Position, l, s);



        if (!loopCheck.ContainsKey(to))
        {
            loopCheck.Add(to, true);
            float h = to.Position.height / (to.Childs.Length * 1.0f);
            for (int i = 0; i < to.Childs.Length; i++)
            {        
                Rect fromRect = sumRect(to.Position, new Rect(0, h * i, 0, h-to.Position.height));
                // Looking child line
                if (lookingChildNode == to && i == lookingChildSlot)
                {
                    if (hovering != -1) curveFromTo(fromRect, nodes[hovering].Position, l, s);
                    else curveFromTo(fromRect, new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1, 1), l, s);
                }
                else drawLines(fromRect, to.Childs[i]);
            }
        }

    }

    void drawLines(Secuence secuence)
    {
        loopCheck.Clear();
        drawLines(new Rect(0, 0, 0, position.height), secuence.Root);

        // Draw the rest of the lines in red
        foreach (var n in secuence.Nodes)
        {
            if (!loopCheck.ContainsKey(n))
            {
                float h = n.Position.height / (n.Childs.Length * 1.0f);
                for (int i = 0; i < n.Childs.Length; i++)
                    if(n.Childs[i]!=null)
                    { 
                        Rect fromRect = sumRect(n.Position, new Rect(0, h * i, 0, h - n.Position.height));
                        curveFromTo(fromRect, n.Childs[i].Position, r, s);
                    }
            }
        }
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
            node.Position = GUILayout.Window(node.GetInstanceID(), node.Position, nodeWindow, node.Name);
		}
	}
	
	Color s = new Color(0.4f, 0.4f, 0.5f),
        l = new Color(0.3f, 0.7f, 0.4f),
        r = new Color(0.8f, 0.2f, 0.2f);
	void OnGUI()
	{
        this.wantsMouseMove = true;

        if (closeStyle == null)
        {
            closeStyle = new GUIStyle(GUI.skin.button);
            closeStyle.padding = new RectOffset(0, 0, 0, 0);
            closeStyle.margin = new RectOffset(0, 5, 2, 0);
            closeStyle.normal.textColor = Color.red;
            closeStyle.focused.textColor = Color.red;
            closeStyle.active.textColor = Color.red;
            closeStyle.hover.textColor = Color.red;
        }

        if (collapseStyle == null)
        {
            collapseStyle = new GUIStyle(GUI.skin.button);
            collapseStyle.padding = new RectOffset(0, 0, 0, 0);
            collapseStyle.margin = new RectOffset(0, 5, 2, 0);
            collapseStyle.normal.textColor = Color.blue;
            collapseStyle.focused.textColor = Color.blue;
            collapseStyle.active.textColor = Color.blue;
            collapseStyle.hover.textColor = Color.blue;
        }

		SecuenceNode nodoInicial = secuence.Root;
        GUILayout.BeginVertical(GUILayout.Height(20));
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("New Node")){
            secuence.createChild();
        }
        if(GUILayout.Button("Set Root")){
            if (nodes.ContainsKey(focusing))
            {
                secuence.Root = nodes[focusing];
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        // Clear mouse hover
        if (Event.current.type == EventType.mouseMove) hovering = -1;

		BeginWindows();
        nodes.Clear();
		createWindows(secuence);

        if (Event.current.type == EventType.repaint)
            drawLines(secuence);

		EndWindows();
	}
}