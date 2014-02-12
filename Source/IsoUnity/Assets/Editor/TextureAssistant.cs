using UnityEngine;
using UnityEditor;
using System.Collections;

public class TextureAssistant : EditorWindow {

	[MenuItem("Window/TextureAssistant")]
	static void ShowWindow () {
		EditorWindow.GetWindow(typeof(TextureAssistant));
	}

	
	public TextureAssistant ()
	{
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}


	IsoTexture currentText = null;
	int selected;
	Vector2 scrollPos = new Vector2(0,0);
	void OnGUI(){

		Event e = Event.current;

		IsoTexture[] textures = TextureManager.getInstance().textureList();

		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(5,5,5,5);
		EditorGUILayout.BeginHorizontal();
			GUIContent newButtonText = new GUIContent("New");
			Rect newButtonRect = GUILayoutUtility.GetRect(newButtonText, style);
			//newButtonRect.height += newButtonRect.height;
			
			if (e.isMouse && newButtonRect.Contains(e.mousePosition)) { 
				if(e.type == EventType.MouseUp){
					TextureManager.getInstance().newTexture();
				}
			} 
			
			GUI.Button(newButtonRect, newButtonText);

			GUIContent deleteButtonText = new GUIContent("Delete");
			Rect deleteButtonRect = GUILayoutUtility.GetRect(deleteButtonText, style);
			
			if (e.isMouse && deleteButtonRect.Contains(e.mousePosition)) 
			{ 
				if(e.type == EventType.MouseUp)
					if(selected < textures.Length){
						TextureManager.getInstance().deleteTexture(TextureManager.getInstance().textureList()[selected]);
						selected = textures.Length;
					}
			}
			
			GUI.Button(deleteButtonRect, deleteButtonText);

		EditorGUILayout.EndHorizontal();


		GUIContent[] texts = new GUIContent[textures.Length];
		for(int i = 0; i< textures.Length; i++){
			texts[i] = new GUIContent(textures[i].name);
		}

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.Width(0), GUILayout.Height(60));
		selected = GUILayout.SelectionGrid(selected,texts,2,style,null); 
		EditorGUILayout.EndScrollView();

		if(selected < textures.Length && textures[selected] != null)
			currentText = textures[selected];
		else 
			currentText = null;

		// Textura
		if(currentText != null){
			currentText.setTexture(UnityEditor.EditorGUILayout.ObjectField("Base tile", currentText.getTexture(), typeof(Texture2D), true) as Texture2D);
			currentText.name = UnityEditor.EditorGUILayout.TextField("Name", currentText.name);
			Texture2D texture = currentText.getTexture();

			if(texture != null){

				GUIContent rotationText = new GUIContent("Rotation: " + currentText.Rotation*90);
				Rect rotationRect = GUILayoutUtility.GetRect(rotationText, style);				
				if (e.isMouse && rotationRect.Contains(e.mousePosition)) { 
					
					if(e.type == EventType.MouseUp){
						currentText.Rotation++;
					}
				} 
				
				GUI.Button(rotationRect, rotationText);

				EditorGUILayout.BeginHorizontal();
					currentText.setXCorner(EditorGUILayout.IntField(currentText.getXCorner(),GUILayout.Width(30)));
					currentText.setXCorner(Mathf.FloorToInt(GUILayout.HorizontalSlider(currentText.getXCorner(),0,texture.width,null)));
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.BeginHorizontal();
					currentText.setYCorner(EditorGUILayout.IntField(currentText.getYCorner(),GUILayout.Width(30)));
					currentText.setYCorner(Mathf.FloorToInt(GUILayout.HorizontalSlider(currentText.getYCorner(),0,texture.height,null)));
				EditorGUILayout.EndHorizontal();
				float auxWidth = position.width;

				float converter = (auxWidth / texture.width*1.0f );

				float auxHeight = converter * (texture.height*1.0f);

				Rect auxRect = GUILayoutUtility.GetRect(auxWidth, auxHeight);

				auxRect.width = auxWidth;
				GUI.DrawTexture(auxRect,texture);

				Vector2 rectCorner = new Vector2(auxRect.x,auxRect.y);

				Vector2 xCorner = rectCorner + new Vector2( converter *currentText.getXCorner(), 0);
				Vector2 yCorner = rectCorner + new Vector2(0, converter * currentText.getYCorner());
				Vector2 xOtherCorner = rectCorner + new Vector2(converter * currentText.getOppositeXCorner(), auxHeight);
				Vector2 yOtherCorner = rectCorner + new Vector2(auxWidth, converter * currentText.getOppositeYCorner());

				Drawing.DrawLine(xCorner,yCorner, Color.yellow, 0.5f, true);
				Drawing.DrawLine(yCorner,xOtherCorner, Color.yellow, 0.5f, true);
				Drawing.DrawLine(xOtherCorner,yOtherCorner, Color.yellow, 0.5f, true);
				Drawing.DrawLine(yOtherCorner,xCorner, Color.yellow, 0.5f, true);

			}
			else
				EditorGUILayout.LabelField("Please asign a texture!", style);
		}else{
			EditorGUILayout.LabelField("Please select a texture o create a new one!", style);
		}

		//GUI
	}
	
	void OnSceneGUI (SceneView sceneView){
	
	}

}
