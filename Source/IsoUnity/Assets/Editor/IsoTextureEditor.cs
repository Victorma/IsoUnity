using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IsoTexture))]
public class IsoTextureEditor : Editor{


	IsoTexture isoTexture;
	public void OnEnable(){
		isoTexture = target as IsoTexture;

	}


	public override void OnInspectorGUI(){

		isoTexture = target as IsoTexture;

		if(GUILayout.Button("Open texture assistant")){
			TextureAssistant.OpenWindowEditor(isoTexture);
		}

	}



}



