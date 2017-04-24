using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace IsoUnity {
	[CustomEditor(typeof(IsoSwitches))]
	public class IsoSwitchesEditor : Editor{

		private Vector2 scrollposition = new Vector2(0,0);

	    private ReorderableList switchList;

		IsoSwitches isoSwitches;
		public void OnEnable(){

	        if (target == null)
	            return;

	        isoSwitches = target as IsoSwitches;

	        switchList = new ReorderableList(isoSwitches.switches, typeof(ISwitch), true, false, true, true);

	        switchList.elementHeight = 35;
	        
	        switchList.drawElementCallback += (rect, index, isActive, isFocused) =>
	        {
	            var isw = switchList.list[index] as ISwitch;

	            EditorGUI.BeginChangeCheck();
	            isw.id = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, rect.height / 2f - 2f), "ID ", isw.id);
	            isw.State = ParamEditor.editorFor(new Rect(rect.x, rect.y + rect.height / 2f , rect.width, rect.height /2f - 2f), isw.State);
	            if (EditorGUI.EndChangeCheck())
	            {
	                EditorUtility.SetDirty(isw);
	            }

	        };

	        switchList.onRemoveCallback += (list) =>
	        {
	            isoSwitches.removeSwitch(isoSwitches.switches[list.index]);
	        };

	        switchList.onAddCallback += (list) =>
	        {
	            var isw = isoSwitches.addSwitch();
	            isw.id = search;
	        };
		}

	    string search = "";
		public override void OnInspectorGUI(){

			isoSwitches = target as IsoSwitches;
			
			GUIStyle style = new GUIStyle();
			style.padding = new RectOffset(5,5,5,5);

			isoSwitches = target as IsoSwitches;
	        search = EditorGUILayout.TextField("Search", search);
			EditorGUILayout.HelpBox("List of switches that represent the state of the game.", MessageType.None);

	        if (string.IsNullOrEmpty(search))
	        {
	            switchList.list = isoSwitches.switches;
	            switchList.draggable = true;
	        }
	        else
	        {
	            switchList.list = isoSwitches.switches.FindAll(i => i.id.Contains(search));
	            switchList.draggable = false;
	        }

			scrollposition = EditorGUILayout.BeginScrollView (scrollposition);

	        switchList.DoLayoutList();

			EditorGUILayout.EndScrollView ();

	        

			/*ISwitch[] switches = isoSwitches.getList ();
			if(switches != null){
				int i = 0;
				scrollposition = EditorGUILayout.BeginScrollView(scrollposition, GUILayout.ExpandHeight(true));
				foreach(ISwitch isw in switches){
					
					i++;
				}
				EditorGUILayout.EndScrollView();
			}

			EditorGUILayout.BeginHorizontal();
			GUIContent bttext = new GUIContent("Add Switch");
			Rect btrect = GUILayoutUtility.GetRect(bttext, style);		
			if(GUI.Button(btrect,bttext)){
			};
			EditorGUILayout.EndHorizontal();*/
		}
	}
}