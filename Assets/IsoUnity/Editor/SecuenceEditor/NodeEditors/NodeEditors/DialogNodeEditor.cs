using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

public class DialogNodeEditor : NodeEditor {

	private SecuenceNode myNode;
	private Vector2 scroll = new Vector2(0,0);

    private ReorderableList fragmentsReorderableList, optionsReorderableList;
    private Dialog dialog;

	public void draw(){

		dialog = myNode.Content as Dialog;
		
		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(5,5,5,5);
		dialog.id = UnityEditor.EditorGUILayout.TextField("Name", dialog.id);

        ArrayList lf = new ArrayList(dialog.getFragments());
        fragmentsReorderableList.list = lf;

        ArrayList lo = new ArrayList(dialog.getOptions());
        optionsReorderableList.list = lo;
		
		EditorGUILayout.HelpBox("You have to add at least one", MessageType.None);
        if (fragmentsReorderableList.list != null)
        {
			bool isScrolling = false;
            if (fragmentsReorderableList.list.Count > 3)
            {
				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandWidth(true), GUILayout.Height(250));
				isScrolling = true;
			}

            fragmentsReorderableList.elementHeight = fragmentsReorderableList.list.Count == 0 ? 20 : 70;
            fragmentsReorderableList.DoLayoutList();

			if(isScrolling)
				EditorGUILayout.EndScrollView();
		}
		
		EditorGUILayout.HelpBox("Options are the lines between you have to choose at the end of the dialog. Leave empty to do nothing, put one to execute this as the dialog ends, or put more than one to let the player choose between them.", MessageType.None);
        if (optionsReorderableList.list != null)
        {
            int i = optionsReorderableList.count;
		}

        optionsReorderableList.DoLayoutList();
		
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


        // This could be used aswell, but I only advise this your class inherrits from UnityEngine.Object or has a CustomPropertyDrawer
        // Since you'll find your item using: serializedObject.FindProperty("list").GetArrayElementAtIndex(index).objectReferenceValue
        // which is a UnityEngine.Object
        // reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);

        // Add listeners to draw events

        fragmentsReorderableList = new ReorderableList(new ArrayList(), typeof(Dialog.Fragment), true, true, true, true);
        fragmentsReorderableList.drawHeaderCallback  += DrawFragmentsHeader;
        fragmentsReorderableList.drawElementCallback += DrawFragment;
        fragmentsReorderableList.onAddCallback       += AddFragment;
        fragmentsReorderableList.onRemoveCallback    += RemoveFragment;
        fragmentsReorderableList.onReorderCallback   += ReorderFragments;


        optionsReorderableList = new ReorderableList(new ArrayList(), typeof(Dialog.DialogOption), true, true, true, true);
        //optionsReorderableList.elementHeight = 70;
        optionsReorderableList.drawHeaderCallback  += DrawOptionsHeader;
        optionsReorderableList.drawElementCallback += DrawOption;
        optionsReorderableList.onAddCallback       += AddOption;
        optionsReorderableList.onRemoveCallback    += RemoveOption;
        optionsReorderableList.onReorderCallback   += ReorderOptions;
	}


    private Rect moveRect(Rect target, Rect move)
    {
        Rect r = new Rect(move.x + target.x, move.y + target.y, target.width, target.height);

        if (r.x + r.width > move.x + move.width)
        {
            r.width = (move.width+25) - r.x;
        }

        return r;
    }

    /*****************************
     * FRAGMENTS LIST OPERATIONS
     *****************************/

    Rect entityRect = new Rect(0, 2, 40, 15);
    Rect objectRect = new Rect(45, 2, 185, 15);
    Rect faceRect = new Rect(-5, 20, 43, 43);
    Rect nameRect = new Rect(45, 20, 190, 15);
    Rect textRect = new Rect(45, 35, 190, 30);
    private void DrawFragmentsHeader(Rect rect)
    {
        GUI.Label(rect, "Dialog fragments");
    }

    private void DrawFragment(Rect rect, int index, bool active, bool focused)
    {
        Dialog.Fragment frg = (Dialog.Fragment)fragmentsReorderableList.list[index];

        EditorGUI.LabelField(moveRect(entityRect, rect), "Target: ");
        frg.IsEntityFragment = true;
        frg.Entity = (Entity)EditorGUI.ObjectField(moveRect(objectRect, rect), frg.Entity, typeof(Entity), true);
        frg.IsEntityFragment = frg.Entity != null;
        frg.Face = EditorGUI.ObjectField(moveRect(faceRect, rect), frg.Face, typeof(Texture2D), true) as Texture2D;
        frg.Name = EditorGUI.TextField(moveRect(nameRect,rect), frg.Name);
        frg.Msg = EditorGUI.TextArea(moveRect(textRect, rect), frg.Msg);

        // If you are using a custom PropertyDrawer, this is probably better
        // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
        // Although it is probably smart to cach the list as a private variable ;)
    }

    private void AddFragment(ReorderableList list)
    {
        dialog.addFragment();
    }

    private void RemoveFragment(ReorderableList list)
    {
        dialog.removeFragment(dialog.getFragments()[list.index]);

    }

    private void ReorderFragments(ReorderableList list)
    {
        ArrayList l = (ArrayList)fragmentsReorderableList.list;
        dialog.setFragments((Dialog.Fragment[])l.ToArray(typeof(Dialog.Fragment)));
    }


    /**************************
     * OPTIONS LIST OPERATIONS
     ***************************/

    Rect labelRect = new Rect(0, 2, 35, 15);
    Rect optionRect = new Rect(40, 2, 185, 15);
    private void DrawOptionsHeader(Rect rect)
    {
        GUI.Label(rect, "Dialog options");
    }

    private void DrawOption(Rect rect, int index, bool active, bool focused)
    {
        Dialog.DialogOption opt = (Dialog.DialogOption)optionsReorderableList.list[index];

        EditorGUI.LabelField(moveRect(labelRect, rect), "Text: ");
        opt.text = EditorGUI.TextField(moveRect(optionRect, rect), opt.text);
  
        if (myNode.Childs[index] != null)
            myNode.Childs[index].Name = "Option " + (index + 1);
    }

    private void AddOption(ReorderableList list)
    {
        dialog.addOption();
        if (myNode.Childs.Length < dialog.getOptions().Length)
            myNode.addNewChild();
    }

    private void RemoveOption(ReorderableList list)
    {
        dialog.removeOption(dialog.getOptions()[list.index]);
        if (myNode.Childs.Length > 1)
        {
            myNode.removeChild(list.index);
        }
    }

    private void ReorderOptions(ReorderableList list)
    {
        ArrayList l = (ArrayList)optionsReorderableList.list;
        dialog.setOptions((Dialog.DialogOption[])l.ToArray(typeof(Dialog.DialogOption)));
    }
}