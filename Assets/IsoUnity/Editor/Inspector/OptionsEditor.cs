using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IsoUnity.Sequences {
	[CustomEditor(typeof(Options))]
	public class OptionsEditor : NodeContentEditor {

	    private static Texture2D conditionsTex, noConditionsTex;

	    private ReorderableList optionsReorderableList;
	    private Editor forkEditor;

	    protected override void OnEnable()
	    {
	        base.OnEnable();

	        if (!conditionsTex) conditionsTex = (Texture2D)Resources.Load("isometra/img/icons/conditions-16x16", typeof(Texture2D));
	        if (!noConditionsTex) noConditionsTex = (Texture2D)Resources.Load("isometra/img/icons/no-conditions-16x16", typeof(Texture2D));

	        optionsReorderableList = new ReorderableList(new ArrayList(), typeof(Option), true, true, true, true);
	        //optionsReorderableList.elementHeight = 70;
	        optionsReorderableList.drawHeaderCallback += DrawOptionsHeader;
	        optionsReorderableList.drawElementCallback += DrawOption;
	        optionsReorderableList.onAddCallback += AddOption;
	        optionsReorderableList.onRemoveCallback += RemoveOption;
	        optionsReorderableList.onReorderCallback += ReorderOptions;
	        optionsReorderableList.onSelectCallback += (list) =>
	        {
	            forkEditor = list.index != -1 ? Editor.CreateEditor(options.Values[list.index].Fork) : null;
	        };
	    }


	    private Options options;
	    protected override void NodeContentInspectorGUI()
	    {
	        options = target as Options;

	        //EditorGUILayout.HelpBox("Options are the lines between you have to choose at the end of the dialog. Leave empty to do nothing, put one to execute this as the dialog ends, or put more than one to let the player choose between them.", MessageType.None);

	        optionsReorderableList.list = options.Values;
	        optionsReorderableList.DoLayoutList();

	        if (forkEditor != null && forkEditor.target != null)
	            forkEditor.OnInspectorGUI();
	    }



	    /**************************
	     * OPTIONS LIST OPERATIONS
	     ***************************/
	     
	    private void DrawOptionsHeader(Rect rect)
	    {
	        GUI.Label(rect, "Dialog options");
	    }

	    private void DrawOption(Rect rect, int index, bool active, bool focused)
	    {
	        EditorGUI.BeginChangeCheck();

	        Option opt = (Option)optionsReorderableList.list[index];
	        
	        opt.Text = EditorGUI.TextField(new Rect(rect.position + new Vector2(0,1), new Vector2(rect.width-20, rect.height-4)), opt.Text);
	        GUI.DrawTexture(new Rect(rect.position + new Vector2(rect.width - 17,0), new Vector2(16, 16)), (opt.Fork as ForkGroup).List.Count > 0 ? conditionsTex : noConditionsTex);

	        if (EditorGUI.EndChangeCheck())
	        {
	            EditorUtility.SetDirty(options);
	        }
	    }

	    private void AddOption(ReorderableList list)
	    {
	        options.AddOption();
	        EditorUtility.SetDirty(options);
	    }

	    private void RemoveOption(ReorderableList list)
	    {
	        options.removeOption(options.Values[list.index]);
	        EditorUtility.SetDirty(options);
	    }

	    private void ReorderOptions(ReorderableList list)
	    {
	        List<Option> l = (List<Option>)optionsReorderableList.list;
	        options.Values = l;
	        EditorUtility.SetDirty(options);
	    }

	    /**
	     * moveRect
	     * */
	     
	    private Rect moveRect(Rect target, Rect move)
	    {
	        Rect r = new Rect(move.x + target.x, move.y + target.y, target.width, target.height);

	        if (r.x + r.width > move.x + move.width)
	        {
	            r.width = (move.width + 25) - r.x;
	        }

	        return r;
	    }
	}
}