using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;

public class TextureAssistant : EditorWindow {

	[MenuItem("Window/TextureAssistant")]
	static void ShowWindow () {
		EditorWindow.GetWindow(typeof(TextureAssistant));
	}

	public static void OpenWindowEditor(IsoTexture selected){
		TextureAssistant assistant = EditorWindow.GetWindow(typeof(TextureAssistant)) as TextureAssistant;
		assistant.currentText = selected;

		IsoTexture[] textures = TextureManager.getInstance().textureList();
		for(int i = 0; i< textures.Length; i++)
			if(selected == textures[i])
				assistant.selected  = i;

	}

	
	public TextureAssistant ()
	{
		extra = new AnimBool(true);
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	AnimBool extra;
	private float lastValue;
	IsoTexture currentText = null;
	int selected;
	//Vector2 scrollPos = new Vector2(0,0);
	Rect textureRect;
	void OnGUI(){

		IsoTexture[] textures = TextureManager.getInstance().textureList();

		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.padding = new RectOffset(5,5,5,5);

		GUIStyle s = new GUIStyle(GUIStyle.none);
		s.fontStyle = FontStyle.Bold;

		/* NO me gusta como se ve esto la verdad...
		 * EditorGUILayout.BeginHorizontal(); 

			if(GUILayout.Button("New", style))
				TextureManager.getInstance().newTexture();
			
			
			if(GUILayout.Button("Delete", style))
				if(selected < textures.Length){
					TextureManager.getInstance().deleteTexture(TextureManager.getInstance().textureList()[selected]);
					selected = textures.Length;
				}

		EditorGUILayout.EndHorizontal();


		GUIContent[] texts = new GUIContent[textures.Length];
		for(int i = 0; i< textures.Length; i++){
			texts[i] = new GUIContent(textures[i].name);
		}

		EditorGUILayout.PrefixLabel("IsoTextures List", s);

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.Width(0), GUILayout.Height(60));
		selected = GUILayout.SelectionGrid(selected,texts,2,style,GUILayout.MaxWidth(auxWidth-25)); 
		EditorGUILayout.EndScrollView();
*/

		if(selected < textures.Length && textures[selected] != null)
			currentText = textures[selected];
		else 
			currentText = null;

		EditorGUILayout.PrefixLabel("IsoTexture", s);
		currentText = EditorGUILayout.ObjectField(currentText, typeof(IsoTexture), false) as IsoTexture;
		for(int i = 0; i< textures.Length; i++)
			if(currentText == textures[i])
				selected = i;

		// Textura
		if(currentText != null){

			currentText.name = UnityEditor.EditorGUILayout.TextField("Name", currentText.name);
			currentText.setTexture(UnityEditor.EditorGUILayout.ObjectField("Base tile", currentText.getTexture(), typeof(Texture2D), true) as Texture2D);

			Texture2D texture = currentText.getTexture();

			if(texture != null){

				EditorGUILayout.PrefixLabel("Presets", s);

				if(GUILayout.Button("Top")){
					currentText.Rotation = 0;
					currentText.setXCorner(Mathf.RoundToInt(currentText.getTexture().width/2f));
					currentText.setYCorner(Mathf.RoundToInt(currentText.getTexture().height/2f));
				}
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Left")){
					currentText.Rotation = 0;
					currentText.setXCorner(0);
					currentText.setYCorner(Mathf.RoundToInt((2f*currentText.getTexture().height)/3f));
				}
				if(GUILayout.Button("Right")){
					currentText.Rotation = 3;
					currentText.setXCorner(currentText.getTexture().width);
					currentText.setYCorner(Mathf.RoundToInt(currentText.getTexture().height/3f));
				}

				EditorGUILayout.EndHorizontal();

				extra.target = EditorGUILayout.ToggleLeft("Advanced", extra.target);
				if (EditorGUILayout.BeginFadeGroup(extra.faded))
					
				{
					EditorGUI.indentLevel++;
					
					if(GUILayout.Button("Rotation: " + currentText.Rotation*90))
						currentText.Rotation++; 
					
					EditorGUILayout.BeginHorizontal();
					currentText.setXCorner(EditorGUILayout.IntField(currentText.getXCorner(),GUILayout.Width(30)));
					currentText.setXCorner(Mathf.FloorToInt(GUILayout.HorizontalSlider(currentText.getXCorner(),0,texture.width,null)));
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					currentText.setYCorner(EditorGUILayout.IntField(currentText.getYCorner(),GUILayout.Width(30)));
					currentText.setYCorner(Mathf.FloorToInt(GUILayout.HorizontalSlider(currentText.getYCorner(),0,texture.height,null)));
					EditorGUILayout.EndHorizontal();
					
					EditorGUI.indentLevel--;
					
				}

				EditorGUILayout.EndFadeGroup();

				if(extra.faded != lastValue)
					this.Repaint();
				lastValue = extra.faded;

				EditorGUILayout.Space();
				Rect rect = GUILayoutUtility.GetLastRect();
				if(Event.current.type == EventType.repaint){
					float w = position.width;
					float h = position.height - rect.y;

					float rh = (w / texture.width*1.0f ) * (texture.height*1.0f) - 10;
					float rw = w - 10;

					if(rh > h){
						rw = (h / texture.height*1.0f ) * texture.width - 10;
						rh = h - 10;
					}

					textureRect = new Rect(w/2f - rw/2f, rect.y + 5, rw, rh);
				}

				float converter = textureRect.width / texture.width;

				/*Rect auxRect = GUILayoutUtility.GetRect(auxWidth, auxHeight);
				auxRect.width = auxWidth;*/
				GUI.DrawTexture(textureRect,texture);

				Vector2 rectCorner = new Vector2(textureRect.x,textureRect.y);
				Vector2 xCorner = rectCorner + new Vector2( converter *currentText.getXCorner(), 0);
				Vector2 yCorner = rectCorner + new Vector2(0, converter * currentText.getYCorner());
				Vector2 xOtherCorner = rectCorner + new Vector2(converter * currentText.getOppositeXCorner(), textureRect.height);
				Vector2 yOtherCorner = rectCorner + new Vector2(textureRect.width, converter * currentText.getOppositeYCorner());

                Handles.BeginGUI( );
                Handles.color = Color.yellow;
                Handles.DrawLine(xCorner, yCorner);
                Handles.DrawLine(yCorner, xOtherCorner);
                Handles.DrawLine(xOtherCorner, yOtherCorner);
                Handles.DrawLine(yOtherCorner, xCorner);

                Handles.EndGUI();
			}
			else
				EditorGUILayout.LabelField("Please asign a texture!");
		}else{
			EditorGUILayout.LabelField("Please select a texture o create a new one!", style);
		}

		//GUI
	}
	
	void OnSceneGUI (SceneView sceneView){
	
	}

}
