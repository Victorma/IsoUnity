using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor {

	private Map map;

	private int selected;
	private MapEditorModule[] modules;

	private GUIStyle toolBarStyle;

	private class ModuleComparision : IComparer<MapEditorModule> {
		public int Compare(MapEditorModule a, MapEditorModule b){
			return a.Order - b.Order;
		}
	}

	void OnEnable()
	{
		this.map = (Map)target;

		/*this.modules = new MapEditorModule[]{
			new NothingModule(),
			new EditModule(),
			new PaintModule(),
			new DecorateModule(),
			new EntityModule()
		};*/
		List<MapEditorModule> modules = new List<MapEditorModule>();

		var type = typeof(MapEditorModule);
		Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();
		foreach(Assembly a in assembly)
			foreach(Type t in a.GetTypes())
				if(type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
					modules.Add(Activator.CreateInstance(t) as MapEditorModule);

		modules.Sort(new ModuleComparision());

		this.modules = modules.ToArray() as MapEditorModule[];

		this.selected = 0;

		toolBarStyle = new GUIStyle();
		toolBarStyle.margin = new RectOffset(50,50,5,10);

        IsoSettings iso = IsoSettingsManager.getInstance().getIsoSettings();
        if (!iso.Configured)
        {
            IsoSettingsPopup.ShowIsoSettingsPopup(iso);
        }
	}
	
	void OnDestroy() 
	{
		map.removeGhost();

		foreach(MapEditorModule mem in modules)
			mem.OnDestroy();
	}

	private void checkRepaint(){
		if(modules[selected].Repaint){
			this.Repaint();
			modules[selected].Repaint = false;
		}
	}

    private bool SetPerspective = false;

	public override void OnInspectorGUI(){

		//GUI.Box (Rect (10,10,100,90), "Loader Menu");
		//map.CellPrefab = UnityEditor.EditorGUILayout.ObjectField("Base Cell", map.CellPrefab, typeof(GameObject), true) as GameObject;
		
		/*if(cellSize < 1)
			cellSize = 1;
		map.setCellSize(cellSize);
		cellSize = UnityEditor.EditorGUILayout.IntField("Cell size", cellSize);*/

		int lastMode = selected;
		if(Tools.current != Tool.None)	selected = 0;

		Rect tool = GUILayoutUtility.GetRect(0,25,toolBarStyle);

		string[] names = new string[modules.Length];
		for(int i = 0; i< modules.Length; i++)
			names[i] = modules[i].Name;

		selected = GUI.Toolbar(tool, selected, names);

		if(selected != lastMode){
			modules[lastMode].OnDisable();
			modules[selected].useMap(map);
			modules[selected].OnEnable();
		}

		modules[selected].OnInspectorGUI();

        // Align to de game view			
        if (GUILayout.Button("Set camera to game view"))
            SetPerspective = true;

		checkRepaint();

	}


	
	void OnSceneGUI (){
		SceneView sceneView = SceneView.currentDrawingSceneView;

        if (SetPerspective)
        {
            setPerspective(sceneView);
            SetPerspective = false;
        }

		modules[selected].OnSceneGUI(sceneView);

		sceneView.Repaint();

		checkRepaint();
	}

    private void setPerspective(SceneView scene)
    {
        /* Selection.transforms	*/

        float angle = 30;
        Texture baseTile = IsoSettingsManager.getInstance().getIsoSettings().defautTextureScale;
        if (baseTile != null)
        {
            float angulo = Mathf.Rad2Deg * Mathf.Acos(baseTile.height / (baseTile.width * 1f));
            angle = 90f - angulo;
        }

        scene.LookAtDirect(scene.pivot, Quaternion.Euler(angle, 45, 0));
        scene.orthographic = true;

        scene.Repaint();
    }
	
}
	

