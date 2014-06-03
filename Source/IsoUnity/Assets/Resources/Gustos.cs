using UnityEngine;
using System.Collections.Generic;

public class Gustos : ScriptableObject {

	[System.Serializable]
	public class MapaTipos {

		public string Id;
		public int hombre;
		public int mujer;
		public int joven;
		public int adulto;
		public int anciano;
		public int prole;
		public int normal;
		public int burgues;
	}

	public List<MapaTipos> Tipos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
