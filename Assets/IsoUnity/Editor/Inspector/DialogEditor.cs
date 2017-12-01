using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using IsoUnity.Entities;

namespace IsoUnity.Sequences {
	[CustomEditor(typeof(Dialog))]
	public class DialogEditor : NodeContentEditor {

	    protected override void OnEnable()
	    {

	        base.OnEnable();
	        // Add listeners to draw events

	        fragmentsReorderableList = new ReorderableList(new ArrayList(), typeof(Fragment), true, true, true, true);
	        fragmentsReorderableList.drawHeaderCallback += DrawFragmentsHeader;
	        fragmentsReorderableList.drawElementCallback += DrawFragment;
	        fragmentsReorderableList.onAddCallback += AddFragment;
	        fragmentsReorderableList.onRemoveCallback += RemoveFragment;
	        fragmentsReorderableList.onReorderCallback += ReorderFragments;
	    }

	    private ReorderableList fragmentsReorderableList;
	    private Dialog dialog;
	    private Vector2 scroll = Vector2.zero;

	    protected override void NodeContentInspectorGUI()
	    {
	        dialog = target as Dialog;

	        GUIStyle style = new GUIStyle();
	        style.padding = new RectOffset(5, 5, 5, 5);
	        dialog.name = UnityEditor.EditorGUILayout.TextField("Name", dialog.name);

	        fragmentsReorderableList.list = dialog.Fragments;

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

	            if (isScrolling)
	                EditorGUILayout.EndScrollView();
	        }
	    }



	    private Rect moveRect(Rect target, Rect move)
	    {
	        Rect r = new Rect(move.x + target.x, move.y + target.y, target.width, target.height);

	        if (r.x + r.width > move.x + move.width)
	        {
	            r.width = (move.width + 25) - r.x;
	        }

	        return r;
	    }

	    /*****************************
	     * FRAGMENTS LIST OPERATIONS
	     *****************************/

	    private void DrawFragmentsHeader(Rect rect)
	    {
	        GUI.Label(rect, "Dialog fragments");
	    }

	    private void DrawFragment(Rect rect, int index, bool active, bool focused)
	    {
	        EditorGUI.BeginChangeCheck();

	        Fragment frg = (Fragment)fragmentsReorderableList.list[index];


            Rect entityRect = new Rect(0, 2, 40, 15);
            Rect objectRect = new Rect(45, 2, 1850, 15);
            Rect faceRect = new Rect(-5, 20, 43, 43);
            Rect nameRect = new Rect(45, 20, 1900, 15);
            Rect textRect = new Rect(45, 35, 1900, 30);

            /*Rect characterRect = new Rect(0, 2, rect.width * .5f, 15);
			Rect parameterRect = new Rect(rect.width * .5f, 2, rect.width * .5f, 15);
			Rect nameRect = new Rect(0, 20, rect.width, 15);
			Rect textRect = new Rect(0, 35, rect.width, 30);*/  

            EditorGUI.LabelField(moveRect(entityRect, rect), "Target: ");
            frg.IsEntityFragment = true;
            frg.Entity = (Entity)EditorGUI.ObjectField(moveRect(objectRect, rect), frg.Entity, typeof(Entity), true);
            frg.IsEntityFragment = frg.Entity != null;
            frg.Face = EditorGUI.ObjectField(moveRect(faceRect, rect), frg.Face, typeof(Texture2D), true) as Texture2D;
            frg.Name = EditorGUI.TextField(moveRect(nameRect, rect), frg.Name);
            frg.Msg = EditorGUI.TextArea(moveRect(textRect, rect), frg.Msg);

            // If you are using a custom PropertyDrawer, this is probably better
            // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
            // Although it is probably smart to cach the list as a private variable ;)


            if (EditorGUI.EndChangeCheck())
	        {
	            EditorUtility.SetDirty(dialog);
	        }
	    }

	    private void AddFragment(ReorderableList list)
	    {
	        dialog.AddFragment();
	        EditorUtility.SetDirty(dialog);
	    }

	    private void RemoveFragment(ReorderableList list)
	    {
	        dialog.RemoveFragment(dialog.Fragments[list.index]);
	        EditorUtility.SetDirty(dialog);
	    }

	    private void ReorderFragments(ReorderableList list)
	    {
	        List<Fragment> l = (List<Fragment>)fragmentsReorderableList.list;
	        dialog.Fragments = l;

	        EditorUtility.SetDirty(dialog);
	    }

	}
}