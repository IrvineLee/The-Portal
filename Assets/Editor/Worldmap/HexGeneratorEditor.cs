using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(HexGenerator))]
public class HexGeneratorEditor : Editor
{
	HexGenerator mSelf;
	
	void OnEnable()
	{
		mSelf = (HexGenerator)target;
	}
	
	public override void OnInspectorGUI()
	{
		mSelf.prefabHex = EditorGUILayout.ObjectField ("P.Hex", mSelf.prefabHex, typeof(GameObject), false) as GameObject;
		mSelf.prefabBoundary = EditorGUILayout.ObjectField ("P.Boundary", mSelf.prefabBoundary, typeof(GameObject), false) as GameObject;
		mSelf.prefabOuterBoundary = EditorGUILayout.ObjectField ("P.OuterBoundary", mSelf.prefabOuterBoundary, typeof(GameObject), false) as GameObject;
		mSelf.prefabWall = EditorGUILayout.ObjectField ("P.Wall", mSelf.prefabWall, typeof(GameObject), false) as GameObject;
		mSelf.mRow = EditorGUILayout.IntField("Row", mSelf.mRow);
		mSelf.mCol = EditorGUILayout.IntField("Col", mSelf.mCol);
		
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox("BASIC", MessageType.None);
		if (GUILayout.Button("Generate!")) mSelf.GenerateTiles();
		if (GUILayout.Button("Outer Boundaries")) mSelf.GenerateOuterBoundaries();
		if (GUILayout.Button("Walls")) mSelf.GenerateWalls();
		
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox("DELETE", MessageType.None);
		if (GUILayout.Button("Destroy All")) mSelf.DestroyChildren();
		if (GUILayout.Button("Destroy Outer Boundaries")) mSelf.DestroyOuterBoundaries();
		if (GUILayout.Button("Destroy Walls")) mSelf.DestroyWalls();
		
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox("AFTER DELETE OPTIONS", MessageType.None);
		mSelf.mRelinkType = (HexGenerator.RelinkType) EditorGUILayout.EnumPopup("AD : Relink Type", mSelf.mRelinkType);
		if (GUILayout.Button("AD : Relinked")) mSelf.Refresh(); 
		if (GUILayout.Button("AD : Del Unused Boundaries")) mSelf.Delete();
		//if (GUILayout.Button("Reload")) mSelf.Reload();
		//if (GUILayout.Button("DrawLink")) mSelf.DrawLink();
		
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox("ADD", MessageType.None);
		mSelf.mAddID = EditorGUILayout.IntField("ID", mSelf.mAddID);
		mSelf.mAddType = (HexGenerator.AddType) EditorGUILayout.EnumPopup("Add Type", mSelf.mAddType);
		if (GUILayout.Button("Add Tile")) mSelf.AddTileByID(mSelf.mAddID);
		
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox("OTHERS", MessageType.None);
		if (GUILayout.Button("Reset")) mSelf.Reset();
		if (GUILayout.Button("Check")) mSelf.Check();
		if (GUI.changed) { EditorUtility.SetDirty(target); }
	}
}
