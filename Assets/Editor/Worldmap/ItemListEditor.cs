/*using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ItemList))]
public class ItemListEditor : Editor 
{
	ItemList mSelf;
	
	string name;
	
	void OnEnable () 
	{
		mSelf = (ItemList)target;
	}
	
	public override void OnInspectorGUI() 
	{
		for(int i = 0; i < mSelf.AccList.Count; i++)
		{ 
			mSelf.AccList[i].name = EditorGUILayout.TextField("Name", mSelf.AccList[i].name); 
			mSelf.AccList[i].desciption = EditorGUILayout.TextField("Desc.", mSelf.AccList[i].desciption); 
		
			mSelf.AccList[i].effects 
				= (ItemList.AccInfo.Effects) EditorGUILayout.EnumPopup("Effect", mSelf.AccList[i].effects);
					
		}
		
		if (GUILayout.Button("Add Item")) mSelf.AddItem();
		
	}
}*/
