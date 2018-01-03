using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FOWRevealer))]
public class FogRevealerEditor : Editor 
{
	FOWRevealer mSelf;
	
	void OnEnable()
	{
		mSelf = (FOWRevealer)target;
	}
	
	public override void OnInspectorGUI()
	{
		mSelf.initRadius = EditorGUILayout.IntSlider("Init Radius", mSelf.initRadius, 1, 5);
		mSelf.initRevealSpeed = EditorGUILayout.FloatField("Reveal Speed", mSelf.initRevealSpeed);
		mSelf.initRevealTime = EditorGUILayout.FloatField("Max Time", mSelf.initRevealTime);
		
		EditorGUILayout.Space();
		mSelf.walkRadius = EditorGUILayout.Slider("Walk Radius", mSelf.walkRadius, 1.0f, 5.0f);
		mSelf.eagleEyeRadius = EditorGUILayout.Slider("E.E Radius", mSelf.eagleEyeRadius, 1.0f, 5.0f);
		mSelf.walkRevealSpeed = EditorGUILayout.FloatField("Reveal Speed", mSelf.walkRevealSpeed);
		
		EditorGUILayout.Space();
		mSelf.isDrawGizmo = EditorGUILayout.Toggle("isDrawGizmo", mSelf.isDrawGizmo);
		mSelf.isEagleEye = EditorGUILayout.Toggle("isEagleEye", mSelf.isEagleEye);
	}
}
