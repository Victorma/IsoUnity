using UnityEngine;
using UnityEditor;
using System.Collections;

public class DialogNodeEditor : NodeEditor {

	private SecuenceNode myNode;
	private Vector2 scroll = new Vector2(0,0);

	public void draw(){

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
				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandWidth(true), GUILayout.Height(250));
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
					if(myNode.Childs.Length > 1){
						myNode.removeChild(i);
						i--;
					}
				}else{
					myNode.Childs[i].Name = "Option "+(i+1);
				}
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
				//this.Repaint ();
			}
	}
	
	public SecuenceNode Result { get{ return myNode; } }
	public string NodeName{ get { return "Dialog"; } }
	public NodeEditor clone(){ return new DialogNodeEditor(); }
	
	public bool manages(SecuenceNode c) { return c.Content != null && c.Content is Dialog; }
	public void useNode(SecuenceNode c) {
		if(c.Content == null || !(c.Content is Dialog))
			c.Content = ScriptableObject.CreateInstance<Dialog>();

		myNode = c;
	}
}