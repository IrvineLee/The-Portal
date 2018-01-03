using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StartMenuScript))]
public class StartMenuEditor : Editor 
{
	StartMenuScript mSelf;
	bool foldoutButton = false;
	bool foldoutRect = false;
	
	void OnEnable()
	{
		mSelf = (StartMenuScript)target;
	}
	
	public override void OnInspectorGUI()
	{
		mSelf.FadeOutTime = EditorGUILayout.FloatField("FadeOutTime", mSelf.FadeOutTime);
		mSelf.FadeInTime = EditorGUILayout.FloatField("FadeInTime", mSelf.FadeInTime);
		mSelf.mInputMode = (StartMenuScript.InputMode) EditorGUILayout.EnumPopup("Input Mode", mSelf.mInputMode);
		
		foldoutButton = EditorGUILayout.Foldout(foldoutButton, "Button Text");
		if (foldoutButton)
		{
			int size = mSelf.buttonStringArray.Length;
			
			for(int i = 0; i < size; i++)
			{ EditorGUILayout.TextField("Element " + i.ToString (), mSelf.buttonStringArray[i].ToString ()); }
		}
		
		foldoutRect = EditorGUILayout.Foldout(foldoutRect, "Rect");
		if (foldoutRect)
		{
			mSelf.isXCenter = EditorGUILayout.Toggle("IsXCenter", mSelf.isXCenter); 
			mSelf.posXRatio = EditorGUILayout.Slider("X", mSelf.posXRatio, 0.0f, 1.0f); 
			mSelf.isYCenter = EditorGUILayout.Toggle("IsYCenter", mSelf.isYCenter); 
			mSelf.posYRatio = EditorGUILayout.Slider("Y", mSelf.posYRatio, 0.0f, 1.0f); 
			mSelf.widthRatio = EditorGUILayout.Slider("Width", mSelf.widthRatio, 0.0f, 1.0f); 
			mSelf.heightRatio = EditorGUILayout.Slider("Height", mSelf.heightRatio, 0.0f, 1.0f); 
			mSelf.offsetRatio = EditorGUILayout.Slider("Offset Height", mSelf.offsetRatio, 0.0f, 1.0f); 
		}
		
		if (GUILayout.Button("Clear Save!")) mSelf.ClearSave();
		
		if (GUI.changed) EditorUtility.SetDirty(mSelf);
	}
}
