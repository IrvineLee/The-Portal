using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AStar))]
public class AStarEditor : Editor
{
	AStar mSelf;

	void OnEnable()
	{
		mSelf = (AStar)target;
	}
	
	public override void OnInspectorGUI()
	{
		mSelf.mLayer = EditorGUILayout.IntField("Layer", mSelf.mLayer);
		mSelf.mRadius = EditorGUILayout.FloatField("Radius", mSelf.mRadius);
		mSelf.mRow = EditorGUILayout.IntField("Row", mSelf.mRow);
		mSelf.mCol = EditorGUILayout.IntField("Col", mSelf.mCol);
		mSelf.mIsDrawGizmo = EditorGUILayout.Toggle("Is Draw Gizmo", mSelf.mIsDrawGizmo);
		
		if (GUILayout.Button("Generate Graph"))
		{
			mSelf.regenerateGraph();
			EditorUtility.SetDirty(target);
		}
	}
}
