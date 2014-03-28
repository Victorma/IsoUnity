using UnityEngine;
using System.Collections;

public class TestDeltaTime : MonoBehaviour {
	
	void OnGUI()
	{
		Test();
	}

	private static float oldTime = 0;
	public static void Test()
	{
		float delta = Time.time - oldTime;
		Debug.Log("unity " + Time.deltaTime);
		Debug.Log("mine " + delta);
		oldTime = Time.time;
		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("UPDATE");
	}

}
