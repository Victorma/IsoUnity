using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using System;
using System.Linq;

namespace IsoUnity.Sequences {
	public class SequenceWindow : EditorWindow
	{
	    [OnOpenAsset(1)]
	    public static bool Open(int instanceID, int line)
	    {
	        var o = EditorUtility.InstanceIDToObject(instanceID);
			if (o is SequenceAsset)
	        {
	            var newWindow = ScriptableObject.CreateInstance<SequenceWindow>();
	            newWindow.sequence = o as SequenceAsset;
	            newWindow.Show();
	            return true;
	        }
	        return false;
	    }

	    private Sequence sequence;

	    public Sequence Sequence
	    {
	        get { return sequence; }
	        set { this.sequence = value; }
	    }

		/*******************
		 *  ATTRIBUTES
		 * *****************/

		// Main vars
	    private Dictionary<int, SequenceNode> nodes = new Dictionary<int, SequenceNode>();
	    private Dictionary<SequenceNode, NodeEditor> editors = new Dictionary<SequenceNode, NodeEditor>();
	    private GUIStyle closeStyle, collapseStyle, selectedStyle;

		// Graph control
		private int hovering = -1;
		private SequenceNode hoveringNode = null;
	    private int focusing = -1;

		// Graph management
	    private int lookingChildSlot;
		private SequenceNode lookingChildNode;
		private Dictionary<SequenceNode, bool> loopCheck = new Dictionary<SequenceNode, bool>();

		// Graph scroll
		private Rect scrollRect = new Rect(0, 0, 1000, 1000);
		private Vector2 scroll;

		// Selection
		private bool toSelect = false;
		private List<SequenceNode> selection = new List<SequenceNode>();
		private Vector2 startPoint;

		/******************
		 * Window behaviours
		 * ******************/

		void OnEnable(){
			InitStyles ();
		}

		void OnGUI()
		{
			if (sequence == null)
				this.Close();

			Sequence.current = sequence;
			this.wantsMouseMove = true;

			// Print the toolbar
			var lastRect = DoToolbar ();

			var rect = new Rect(0, lastRect.y + lastRect.height, position.width, position.height - lastRect.height);

			// Do the sequence graph
			DoGraph (rect);

			Sequence.current = null;
		}

		/*******************************
		 * Initialization methods
		 ******************************/

		void InitStyles(){

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

			if (selectedStyle == null) {
				selectedStyle = Resources.Load<GUISkin> ("resplandor").box;
			}
		}

		/**************************
		 * TOOLBAR
		 **************************/
		Rect DoToolbar(){

			GUILayout.BeginVertical(GUILayout.Height(17));
			GUILayout.BeginHorizontal("toolbar");

			using (new EditorGUI.DisabledScope())
			{
				if (GUILayout.Button("Globals", "toolbarButton", GUILayout.Width(100)))
				{
					var o = SwitchesMenu.ShowAtPosition(GUILayoutUtility.GetLastRect().Move(new Vector2(5,16)));
					if (o) GUIUtility.ExitGUI();
				}
				if (GUILayout.Button("Locals", "toolbarButton", GUILayout.Width(100)))
				{
					var o = SwitchesMenu.ShowAtPosition(GUILayoutUtility.GetLastRect().Move(new Vector2(105, 16)), sequence.LocalVariables);
					if (o) GUIUtility.ExitGUI();
				}
			}

			GUILayout.Space(5);

			if (GUILayout.Button("New Node", "toolbarButton"))
			{
				var node = sequence.CreateNode();
				node.Position = new Rect(scroll + position.size / 2 - node.Position.size / 2, node.Position.size);
				node.Position = new Rect(new Vector2((int)node.Position.x, (int)node.Position.y), node.Position.size);
			}
			if (GUILayout.Button("Set Root", "toolbarButton"))
			{
				if (nodes.ContainsKey(focusing))
				{
					sequence.Root = nodes[focusing];
				}
			}

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			return GUILayoutUtility.GetLastRect();
		}

		/********************
		 * GRAPH
		 * ******************/
		void DoGraph(Rect rect){


			float maxX = rect.width, maxY = rect.height;
			foreach (var node in sequence.Nodes)
			{
				var px = node.Position.x + node.Position.width + 50;
				var py = node.Position.y + node.Position.height + 50;
				maxX = Mathf.Max(maxX, px);
				maxY = Mathf.Max(maxY, py);
			}

			scrollRect = new Rect(0, 0, maxX, maxY);
			scroll = GUI.BeginScrollView(rect, scroll, scrollRect);

			// Clear mouse hover
			if (Event.current.type == EventType.MouseMove) { 
				if (hovering != -1) 
					this.Repaint ();
				
				hovering = -1; 
				hoveringNode = null; 
			}
			GUI.Box(scrollRect, "", "preBackground");
			drawBackground (scrollRect);

			BeginWindows();
			{
				nodes.Clear();
				createWindows(sequence);

				if(Event.current.type == EventType.Repaint)
					foreach (var n in selection)
						GUI.Box (new Rect(
							n.Position.position - new Vector2(0,0), 
							n.Position.size + new Vector2(0	,0)), 
							"", selectedStyle);

				drawSlots(sequence);

				if (Event.current.type == EventType.Repaint)
				{
					drawLines(sequence);
				}
			}
			EndWindows();	


			switch (Event.current.type) {		
			case EventType.MouseMove:
				if (lookingChildNode != null) {
					this.Repaint ();
					Event.current.Use ();
				}
			break;
			case EventType.MouseDrag: 
				{
					if (EditorGUIUtility.hotControl == 0) {
						scroll -= Event.current.delta;
						Repaint ();
					}
				}
				break;
			case EventType.MouseDown: 
				{
					if (Event.current.button == 0) {
						// Selecting
						if (GUIUtility.hotControl == 0) {
							// Start selecting
							GUIUtility.hotControl = this.GetHashCode();
							startPoint = Event.current.mousePosition;
							selection.Clear ();
							Event.current.Use ();
						}
					} 
				}
				break;
			case EventType.MouseUp:
				{
					if (Event.current.button == 0) {
						if (GUIUtility.hotControl == this.GetHashCode()) {
							GUIUtility.hotControl = 0;

							UpdateSelection ();
							Event.current.Use ();
						}

					} else if (Event.current.button == 1) {
						// Right click

						var menu = new GenericMenu();
						var mousePos = Event.current.mousePosition;
						int i = 0;
						foreach (var a in GetPossibleCreations())
						{

							menu.AddItem(new GUIContent("Create/" + a.Key), false, (t) => {
								var kv = (KeyValuePair < string, Type>)t;
								var newObject = CreateInstance(kv.Value);
								var child = sequence.CreateNode(newObject);
								child.Position = new Rect(mousePos, child.Position.size);
							}, a);
							i++;
						}

						menu.ShowAsContext();
					}
				}
				break;
			case EventType.Repaint: 
				// Draw selection rect 
				if (GUIUtility.hotControl == GetHashCode ()) {
					UpdateSelection ();
					Handles.BeginGUI();
					Handles.color = Color.white;
					Handles.DrawSolidRectangleWithOutline (
						Rect.MinMaxRect (startPoint.x, startPoint.y, Event.current.mousePosition.x, Event.current.mousePosition.y), 
						new Color (.3f, .3f, .3f, .3f),
						Color.gray);
					Handles.EndGUI ();
				}
				break;
			}

			GUI.EndScrollView();
		}


		// AUX GRAPH FUNCTIONS

		void drawBackground(Rect rect){

			float increment = 15;

			float pos = 0;
			int i = 0;
			float max = Mathf.Max (rect.width, rect.height);

			Handles.BeginGUI ();

			Handles.DrawSolidRectangleWithOutline (rect, new Color(.2f,.2f,.2f,1f), new Color(.2f,.2f,.2f,1f));

			while (pos < max) {

				Handles.color = new Color(.1f,.1f,.1f,1f);

				// Horizontal
				Handles.DrawAAPolyLine (1f, new Vector3[] { new Vector2 (0,pos), new Vector2 (rect.width, pos)});
				// Vertical
				Handles.DrawAAPolyLine (1f, new Vector3[] { new Vector2 (pos,0), new Vector2 (pos, rect.height)});

				i++;	
				pos += increment;
			}

			i = 0;
			pos = 0;
			while (pos < max) {

				Handles.color = new Color(.05f,.05f,.05f,1f);

				// Horizontal
				Handles.DrawAAPolyLine (1f, new Vector3[] { new Vector2 (0,pos), new Vector2 (rect.width, pos)});
				// Vertical
				Handles.DrawAAPolyLine (1f, new Vector3[] { new Vector2 (pos,0), new Vector2 (pos, rect.height)});

				i+=10;
				pos += increment*10;
			}

			Handles.EndGUI ();
		}

		void drawSlots(Sequence sequence)
		{

			// Draw the rest of the lines in red
			foreach (var n in sequence.Nodes)
			{
				// InputSlot
				drawSlot(new Vector2(n.Position.x, n.Position.y + 3 + n.Position.height / 2));

				// OutputSlots
				float h = n.Position.height / (n.Childs.Length * 1.0f);
				for (int i = 0; i < n.Childs.Length; i++)
					if (drawSlot(new Vector2(n.Position.x + n.Position.width, n.Position.y + h * i + h / 2f)))
					{
						// Detach		
						n.Childs[i] = null;
						lookingChildNode = n;
						lookingChildSlot = i;
					}
			}
		}

		void drawLines(Sequence sequence)
		{
			loopCheck.Clear();

			// Draw the main nodes in green
			drawLines(new Rect(0, 0, 0, position.height), sequence.Root, 
				new Color(.4f, .8f, .4f, 1f), 
				new Color(.4f, .8f, .4f, .2f));

			// Draw the rest of the lines in red
			foreach (var n in sequence.Nodes)
			{
				drawLines(new Rect(-1,-1,-1,-1), n, 
					new Color(.8f, .2f, .2f, 1f), 
					new Color(.8f, .2f, .2f, .2f));
			}
		}


		void drawLines(Rect from, SequenceNode to, Color c, Color notHoveringColor, bool parentHovered = false)
		{
			if (to == null)
				return;

			var hoveringMe = hoveringNode != null && hoveringNode == to;
			var useColor = parentHovered || hoveringNode == null || hoveringMe ? c : notHoveringColor;

			// Visible loop line
			if(from.width != -1 && from.height != -1)
				curveFromTo(from, to.Position, useColor);

			if (!loopCheck.ContainsKey(to))
			{
				loopCheck.Add(to, true);
				float h = to.Position.height / (to.Childs.Length * 1.0f);
				for (int i = 0; i < to.Childs.Length; i++)
				{
					Rect fromRect = sumRect(to.Position, new Rect(0, h * i, 0, h - to.Position.height));
					// Looking child line
					if (lookingChildNode == to && i == lookingChildSlot)
					{
						if (hovering != -1) curveFromTo(fromRect, nodes[hovering].Position, useColor);
						else curveFromTo(fromRect, new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1, 1), useColor);
					}
					else drawLines(fromRect, to.Childs[i], c, notHoveringColor, hoveringMe);
				}
			}

		}

		void createWindows(Sequence sequence)
		{
			foreach (var node in sequence.Nodes)
			{
				nodes.Add(node.GetInstanceID(), node);
				var finalPosition = GUILayout.Window(node.GetInstanceID(), node.Position, NodeWindow, node.Name);
				var diff = finalPosition.position - node.Position.position;

				// If the window has been moved, lets move the others too
				if (diff != Vector2.zero) {

					if (selection.Contains (node)) {
						selection.ForEach (n => {
							if(n != node) {
								n.Position = new Rect(n.Position.position + diff, n.Position.size);
								// Redo the window
								GUILayout.Window(n.GetInstanceID(), n.Position, NodeWindow, n.Name);
							}
						});
					}
				}
				node.Position = finalPosition;
			}
		}

	    void curveFromTo(Rect wr, Rect wr2, Color color)
		{

			Vector2 start = new Vector2 (wr.x + wr.width, wr.y + 3 + wr.height / 2),
				startTangent = new Vector2 (wr.x + wr.width + Mathf.Abs (wr2.x - (wr.x + wr.width)) / 2, wr.y + 3 + wr.height / 2),
				end = new Vector2 (wr2.x, wr2.y + 3 + wr2.height / 2),
				endTangent = new Vector2 (wr2.x - Mathf.Abs (wr2.x - (wr.x + wr.width)) / 2, wr2.y + 3 + wr2.height / 2);

			Handles.BeginGUI();
			Handles.color = color;
			if (start.x > end.x) {
				var sep = 30f;
				var upDown = start.y > end.y ? 1 : -1;
				/*

				var startMax = (startTangent - start).magnitude;
				var endMax = (endTangent - end).magnitude;
				var x = (start.y - end.y);


				var startVector = new Vector2 (0, upDown * ((x*(2*startMax - 2) + 3*startMax) / x)); 
				var endVector = new Vector2 (0, -upDown * ((x*(2*endMax - 2) + 3*endMax) / x)); 
	*/
				startTangent = start + new Vector2(1,0) * 50;
				endTangent =  end + new Vector2(-1,0) * 50;

				/*Vector2 staCornerClose = new Vector2 (wr.xMax + sep, wr.yMin - sep);
				Vector2 staCornerFar   = new Vector2 (wr.xMin - sep, wr.yMin - sep);
				Vector2 endCornerFar   = new Vector2 (wr2.xMax + sep, wr2.yMax + sep);

				Vector2 mid = (staCornerFar + endCornerFar) / 2f;

				Vector2 endCornerClose = new Vector2 (wr2.xMin - sep, wr2.yMax + sep);

				Vector2 FarFar = (wr2.center - wr.center);
				FarFar = FarFar* 50 / FarFar.magnitude ;
				Vector2 RFarFar = Vector2.Reflect (FarFar, new Vector2 (-1, 0));

				Vector2 staCornerCloseT1 = staCornerClose + new Vector2 (1, 1)*50;
				Vector2 staCornerCloseT2 = staCornerClose + new Vector2 (-1, -1)*50;
				Vector2 staCornerFarT1 = staCornerFar + new Vector2 (1, -1)*50;
				Vector2 staCornerFarT2 = staCornerFar + new Vector2 (-1, 1)*50;
				Vector2 endCornerFarT1 = endCornerFar + new Vector2 (1, -1)*50;
				Vector2 endCornerFarT2 = endCornerFar + new Vector2 (-1, 1)*50;
				Vector2 endCornerCloseT1 = endCornerClose + new Vector2 (1, 1)*50;
				Vector2 endCornerCloseT2 = endCornerClose + new Vector2 (-1, -1)*50;

				//staCornerCloseT1 = staCornerClose + RFarFar;
				//staCornerCloseT2 = staCornerClose - RFarFar;
				staCornerFarT1 = staCornerFar - FarFar;
				staCornerFarT2 = staCornerFar + FarFar;
				endCornerFarT1 = endCornerFar - FarFar;
				endCornerFarT2 = endCornerFar + FarFar;

				Vector2 midT1 = mid - FarFar;
				Vector2 midT2 = mid + FarFar;
				//endCornerCloseT1 = endCornerClose + RFarFar;
				//endCornerCloseT2 = endCornerClose - RFarFar;

				Handles.DrawBezier(start, staCornerClose, startTangent, staCornerCloseT1, Color.yellow, null, 3);
				Handles.DrawBezier(staCornerClose, mid, staCornerCloseT2, staCornerFarT1, Color.blue, null, 3);
				//Handles.DrawBezier(staCornerFar, endCornerFar, staCornerFarT2, endCornerFarT1, Color.red, null, 3);
				Handles.DrawBezier(mid, endCornerClose, endCornerFarT2, endCornerCloseT1, Color.green, null, 3);
				Handles.DrawBezier(endCornerClose, end, endCornerCloseT2, endTangent, Color.magenta, null, 3);*/


				Vector2 staCorner   = new Vector2 ( wr.center.x, upDown < 0 ?  wr.yMax :  wr.yMin);
				Vector2 endCorner   = new Vector2 (wr2.center.x, upDown < 0 ? wr2.yMin : wr2.yMax);
				sep = Mathf.Clamp((staCorner - endCorner).magnitude, 0, sep);
				staCorner += new Vector2 (0, upDown * -sep);
				endCorner += new Vector2 (0, upDown * sep);

				var superacionHorizontal = Mathf.Clamp (endCorner.x - staCorner.x, 0, 100) / 100;
					
				Vector2 midCorner   = (staCorner + endCorner) / 2f;
				midCorner = Vector2.Lerp (midCorner, (start + end) / 2f, superacionHorizontal);

				Vector2 staCornerT1 = staCorner + new Vector2( 1, 0) *  wr.width /1.5f;
				Vector2 staCornerT2 = staCorner + new Vector2(-1, 0) *  wr.width /1.5f;
				Vector2 endCornerT1 = endCorner + new Vector2( 1, 0) * wr2.width /1.5f;
				Vector2 endCornerT2 = endCorner + new Vector2(-1, 0) * wr2.width /1.5f;

				var aux = staCorner;
				var fus = Mathf.Clamp01 (Mathf.Max( upDown* (staCorner.y - endCorner.y) + 2*sep , 100 + 2*sep - (staCorner.x - endCorner.x))/100);


				staCorner = Vector2.Lerp (staCorner, midCorner, fus);
				endCorner = Vector2.Lerp (endCorner, midCorner, fus);

					
				Vector2 midCornerT1 = new Vector2(staCornerT1.x, staCorner.y);
				Vector2 midCornerT2 = new Vector2(endCornerT2.x, endCorner.y);

				var ds = Mathf.Lerp (Math.Min (wr.width / 1.5f, Math.Abs (staCorner.x - endCorner.x) / 2f), 0, fus);
				var de = Mathf.Lerp (Math.Min (wr2.width /1.5f, Math.Abs (staCorner.x - endCorner.x)/2f), 0, fus);

				staCornerT2 = staCorner + new Vector2(-1, 0) * Mathf.Lerp (ds, (ds+de) / 2f, fus);
				endCornerT1 = endCorner + new Vector2( 1, 0) * Mathf.Lerp (de, (ds+de) / 2f, fus);

				Handles.DrawBezier(start, staCorner, startTangent, midCornerT1, color /*Color.yellow*/, null, 3);
				if(fus < 1)
					Handles.DrawBezier(staCorner, endCorner, staCornerT2, endCornerT1, color /*Color.blue*/, null, 3);
				Handles.DrawBezier(endCorner, end, midCornerT2, endTangent, color /*Color.red*/, null, 3);

			} else {
				
				Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 3);

			}
			Handles.EndGUI();
	    }

	    private Rect sumRect(Rect r1, Rect r2)
	    {
	        return new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
	    }

	    bool drawSlot(Vector2 center)
	    {
	        return GUI.Button(new Rect(center.x - 10, center.y - 10, 20, 20), "");
	    }

		/**********************
		 * Node windows
		 *********************/

		void NodeWindow(int id)
		{
			SequenceNode myNode = nodes[id];

			// Editor selection

			DoContentEditor (id);

			switch (Event.current.type)
			{
			case EventType.MouseMove:

				if (new Rect (0, 0, myNode.Position.width, myNode.Position.height).Contains (Event.current.mousePosition)) {
					if (hovering != id) {
						this.Repaint ();
					}

					hovering = id;
					hoveringNode = myNode;
				}
				break;
			}

			DoChildSelector (id);
			DoNodeWindowEditorSelection (id);
			DoResizeEditorWindow (id);
			GUI.DragWindow();

		}

		void DoContentEditor(int id){

			var myNode = nodes[id];
			// Top toolbar
			GUILayout.BeginHorizontal();

			// Check if the editors are being changed
			EditorGUI.BeginChangeCheck();

			//
			if (!editors.ContainsKey(myNode) || EditorGUI.EndChangeCheck())
			{
				var editor = NodeEditorFactory.Intance.createNodeEditorFor(
					NodeEditorFactory.Intance.CurrentNodeEditors[
						NodeEditorFactory.Intance.NodeEditorIndex(myNode)
					]);
				
				//vinculating the node to the editor
				editor.useNode(myNode);

				// Cache the editor
				if (!editors.ContainsKey(myNode)) editors.Add(myNode, editor);
				else
				{
					ScriptableObject.DestroyImmediate(editors[myNode] as ScriptableObject);
					editors[myNode] = editor;
				}
			}
				
			// Collapsed nodes
			if (myNode.Collapsed)
			{
				if (GUILayout.Button(myNode.ShortDescription))
					myNode.Collapsed = false;
			}
			else
			{
				GUILayout.FlexibleSpace();
			}

			// Buttons for collapse and delete
			if (GUILayout.Button(myNode.Collapsed ? "+" : "-", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
				myNode.Collapsed = !myNode.Collapsed;
			if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
			{
				sequence.RemoveNode(myNode);
				return;
			}

			GUILayout.EndHorizontal();

			// Node drawing
			if (!myNode.Collapsed)
			{
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				editors[myNode].draw();
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				nodes[id] = editors[myNode].Result;
			}

			// Update the window size according to editor
			if (Event.current.type != EventType.layout)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				Rect myRect = myNode.Position;
				myRect.height = lastRect.y + lastRect.height;
				myNode.Position = myRect;
				//this.Repaint();
			}
		}

		void DoChildSelector(int id){

			var myNode = nodes[id];

			switch (Event.current.type) {

			case EventType.MouseDown:

				// Right Button
				if (Event.current.button == 1) {
					var menu = new GenericMenu ();
					var i = 0;
					string text = string.Empty;
					menu.AddItem (new GUIContent ("Set sequence root"), false, (node) => sequence.Root = node as SequenceNode, myNode);
					foreach (var a in editors[myNode].ChildNames) {
						text = (a == "") ? (i + "") : a;
						menu.AddItem (new GUIContent ("Set node for " + text), false, (t) => {
							// Detach		
							myNode.Childs [(int)t] = null;
							lookingChildNode = myNode;
							lookingChildSlot = (int)t;
						}, i);
						i++;
					}

					menu.ShowAsContext ();
				}

				break;
			}

		}

		void DoResizeEditorWindow(int id){

			var myNode = nodes[id];
			var resizeRect = new Rect(new Vector2(myNode.Position.width - 10, 0), new Vector2(10, myNode.Position.height));
			EditorGUIUtility.AddCursorRect(resizeRect,MouseCursor.ResizeHorizontal, myNode.GetHashCode());
			if (EditorGUIUtility.hotControl == 0 
				&& Event.current.type == EventType.MouseDown 
				&& Event.current.button == 0 
				&& resizeRect.Contains(Event.current.mousePosition))
			{
				EditorGUIUtility.hotControl = myNode.GetHashCode();
				Event.current.Use();
			}


			if(GUIUtility.hotControl == myNode.GetHashCode())
			{
				//Debug.Log("hotcontrol");
				myNode.Position = new Rect(myNode.Position.x, myNode.Position.y, Event.current.mousePosition.x + 5, myNode.Position.height);
				this.Repaint();
				//Event.current.Use();
				if (Event.current.type == EventType.MouseUp)
					EditorGUIUtility.hotControl = 0;
				//if(Event.current.type != EventType.layout)*/
			}

		}

		void DoNodeWindowEditorSelection(int id){

			var myNode = nodes[id];

			switch (Event.current.type)
			{
			case EventType.MouseDown:

				// Left button
				if (Event.current.button == 0)
				{
					if (hovering == id) {
						toSelect = false;
						focusing = hovering;
						if (Event.current.control) {
							if (selection.Contains (myNode))
								selection.Remove (myNode);
							else
								selection.Add (myNode);
						} else {
							toSelect = true;
							if (!selection.Contains (myNode)) {
								selection.Clear ();
								selection.Add (myNode);
							}
						}
					}
					if (lookingChildNode != null)
					{
						// link creation between nodes
						lookingChildNode.Childs[lookingChildSlot] = myNode;
						// finishing search
						lookingChildNode = null;
					}
					if(myNode.Content is UnityEngine.Object)
						Selection.activeObject = myNode.Content as UnityEngine.Object;
				}

				break;

			case EventType.MouseDrag:
				toSelect = false;
				break;
			case EventType.MouseUp:
				{
					if(toSelect) {
						selection.Clear ();
						selection.Add (myNode);
					}
				}
				break;
			}
		}

		/*************************
		 *  Selection
		 * **********************/

		void UpdateSelection(){

			var xmin = Math.Min (startPoint.x, Event.current.mousePosition.x);
			var ymin = Math.Min (startPoint.y, Event.current.mousePosition.y);
			var xmax = Math.Max (startPoint.x, Event.current.mousePosition.x);
			var ymax = Math.Max (startPoint.y, Event.current.mousePosition.y);
			selection = sequence.Nodes.ToList().FindAll (node => 
				RectContains(Rect.MinMaxRect (xmin, ymin, xmax, ymax), node.Position)
			);
			Repaint ();
		}

	    /**************************
	     * Possible node contents *
	     **************************/

	    private Dictionary<string, Type> possibleCreationsCache;
	    private Dictionary<string, Type> GetPossibleCreations()
	    {
	        if (possibleCreationsCache == null)
	        {
	            possibleCreationsCache = new Dictionary<string, Type>();
	            // Make sure is a DOMWriter
	            var contents = AttributesUtil.GetTypesWith<NodeContentAttribute>(true).Where(t => (typeof(UnityEngine.Object)).IsAssignableFrom(t));
	            foreach (var content in contents)
	            {
	                foreach (var attr in content.GetCustomAttributes(typeof(NodeContentAttribute), true))
	                {
	                    var nodeContent = attr as NodeContentAttribute;
	                    var name = nodeContent.Name == string.Empty ? content.ToString() : nodeContent.Name;
	                    possibleCreationsCache.Add(name, content);
	                }
	            }
	        }
	        return possibleCreationsCache;
	    }

		bool RectContains (Rect r1, Rect r2){
			var intersection = Rect.MinMaxRect (Mathf.Max (r1.xMin, r2.xMin), Mathf.Max (r1.yMin, r2.yMin), Mathf.Min (r1.xMax, r2.xMax), Mathf.Min (r1.yMax, r2.yMax));

			return Event.current.shift 
				? r1.xMin < r2.xMin && r1.xMax > r2.xMax && r1.yMin < r2.yMin && r1.yMax > r2.yMax // Completely inside
					:  intersection.width > 0 && intersection.height > 0;
		}

	}
}