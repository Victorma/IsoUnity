using UnityEngine;
using UnityEditor;
using System.Collections;

public class ItemForkEditor : ForkEditor {
	
	private ItemFork isf;
	
	public ItemForkEditor(){
		isf = ScriptableObject.CreateInstance<ItemFork>();
	}
	
	public Checkable Result { 
		get{ return isf;} 
	}
	
	public string ForkName{ 
		get{ return "Item Fork"; } 
	}
	
	public bool manages(Checkable c){
		return c!=null &&c is ItemFork;
	}
	
	public ForkEditor clone(){
		return new ItemForkEditor();
	}
	
	public void useFork(Checkable c){
		if(c is ItemFork)
			isf = c as ItemFork;
	}
	
	public void draw(){
		isf.contains = EditorGUILayout.Toggle("Contains", isf.contains);
		isf.item =  EditorGUILayout.ObjectField("Item", (Object)isf.item, typeof(Item), true) as Item;
		isf.inventory = EditorGUILayout.ObjectField("Inventory", (Object)isf.inventory, typeof(Inventory), true) as Inventory;
	}
}
