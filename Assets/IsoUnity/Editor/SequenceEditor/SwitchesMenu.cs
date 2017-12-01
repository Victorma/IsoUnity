using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

namespace IsoUnity {
	public class SwitchesMenu : EditorWindow
	{
	    internal static bool ShowAtPosition(Rect buttonRect, IsoSwitches switches = null)
	    {
	        long num = DateTime.Now.Ticks / 10000L;
	        if (num >= SwitchesMenu.s_LastClosedTime + 50L)
	        {
	            if (Event.current != null)
	            {
	                Event.current.Use();
	            }
	            if (SwitchesMenu.s_DrawerParametersMenu == null)
	            {
	                SwitchesMenu.s_DrawerParametersMenu = ScriptableObject.CreateInstance<SwitchesMenu>();
	            }
	            SwitchesMenu.s_DrawerParametersMenu.Init(buttonRect, switches);
	            return true;
	        }
	        return false;
	    }

	    public static SwitchesMenu s_DrawerParametersMenu;
	    private static long s_LastClosedTime;

	    private Editor editor;
	    private void Init(Rect buttonRect, IsoSwitches switches)
	    {
	        buttonRect.position = GUIUtility.GUIToScreenPoint(buttonRect.position);
	        float y = 305f;
	        Vector2 windowSize = new Vector2(300f, y);
	        base.ShowAsDropDown(buttonRect, windowSize);
	        editor = Editor.CreateEditor(switches == null ? IsoSwitchesManager.getInstance().getIsoSwitches() : switches);
	    }

	    private void OnDisable()
	    {
	        SwitchesMenu.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
	        SwitchesMenu.s_DrawerParametersMenu = null;
	    }

	    void OnGUI()
	    {
	        editor.OnInspectorGUI();
	    }
	}

	public class SwitchesWindow : EditorWindow{

	    private Editor editor;
	    [MenuItem("Window/Switches")]
	    public static void OpenAsWindow()
	    {
	        var window = EditorWindow.GetWindow<SwitchesWindow>();
	        window.Show();
	        window.editor = Editor.CreateEditor(IsoSwitchesManager.getInstance().getIsoSwitches());
	    }

	    void OnGUI()
	    {
	        editor.OnInspectorGUI();
	    }
	}
}