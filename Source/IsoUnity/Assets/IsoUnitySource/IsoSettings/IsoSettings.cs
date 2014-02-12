using System;
using UnityEngine;
using UnityEditor;

public class IsoSettings : ScriptableObject
{
	[SerializeField]
	public GameObject defaultMapPrefab;
	[SerializeField]
	public GameObject defaultCellPrefab;
	[SerializeField]
	public GameObject defaultDecorationPrefab;
	[SerializeField]
	public Texture2D defautTextureScale;



	public IsoSettings (){
	}
}
