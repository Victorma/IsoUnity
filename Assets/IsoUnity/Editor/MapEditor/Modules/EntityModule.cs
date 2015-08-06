using UnityEngine;
using UnityEditor;
using System.Collections;

public class EntityModule : MapEditorModule {
	
	public string Name {get{return "Entities";}}
	public int Order {get{return 4;}}

	private Map map;
	private Tool selected;

	// InspectorGUI vars
	private Entity entity;
	
	public void useMap(Map map){
		this.map = map;
	}
	
	public void OnEnable(){
		selected = Tools.current;
		Tools.current = Tool.None;
	}
	
	public void OnDisable(){
		Tools.current = selected;
	}


	public void OnInspectorGUI(){
		entity = EditorGUILayout.ObjectField(entity, typeof(Entity),true) as Entity;
	}

	public void OnSceneGUI(SceneView scene){
		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		RaycastHit info = new RaycastHit();

		GameObject selected = null;
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		if(Physics.Raycast(ray, out info/*, LayerMask.NameToLayer("Cells Layer")*/)) //TODO Arreglar esto porque al parecer la distancia no funciona correctamente
			if(info.collider.transform.IsChildOf(this.map.transform))
				selected = info.collider.gameObject;



		bool moveEntity = false;
		if(Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
			moveEntity = true;

		
		if(selected != null){
			Cell cs = selected.GetComponent<Cell>();
			if(cs!=null){
                FaceNoSC f = cs.getFaceByPoint(info.point);
				if(f!=null){
					if(moveEntity)
						entity.Position = cs;
					
					Vector3[] vertex = f.SharedVertex;
					int[] indexes = f.VertexIndex;
					
					Vector3[] puntos = new Vector3[4];
					for(int i = 0; i< indexes.Length; i++)
						puntos[i] = cs.transform.TransformPoint(vertex[indexes[i]]);

					if(indexes.Length == 3)
						puntos[3] = cs.transform.TransformPoint(vertex[indexes[2]]);

					Handles.DrawSolidRectangleWithOutline(puntos, Color.yellow, Color.white);
				}
			}
		}
	}
	public void OnDestroy(){}

	private bool repaint;
	public bool Repaint {
		get{
			return this.repaint;
		}
		set{
			this.repaint = value;
		}
	}
	
}
