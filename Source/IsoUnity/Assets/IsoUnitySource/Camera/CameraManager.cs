using UnityEngine;
using System.Collections;

public class CameraManager  {
	
	private static float distance = 30;
	private static Vector3 separation;

	public static void initialize(){

		if(Camera.main == null){
			GameObject camera = new GameObject();
			camera.AddComponent<Camera>();
			camera.name = "MainCamera";
			camera.tag = "MainCamera";
		}
		RenderSettings.ambientLight = Color.white;

		Camera.main.isOrthoGraphic = true;
		Texture2D baseTile = IsoSettingsManager.getInstance().getIsoSettings().defautTextureScale;

		float angle = 30;
		if(baseTile != null){
			float angulo = Mathf.Rad2Deg * Mathf.Acos(baseTile.height / (baseTile.width*1f));
			angle = 90f - angulo;
		}
		Camera.main.transform.rotation = Quaternion.Euler(angle, 45, 0);

		separation = new Vector3(0,0,distance);
		separation = Quaternion.Euler(angle, 45, 0) * separation;

	}

	public static void lookTo(GameObject go){

		Camera.main.transform.position = go.transform.position - separation;

	}

	public static void follow(GameObject go){

	}
	
}
