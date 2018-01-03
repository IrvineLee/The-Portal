using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(HexScript))]
public class HexScriptEditor : Editor
{
	public bool isSetDirty;
	
	HexScript mSelf;
	bool foldoutPrefab = false;
	bool foldoutTexture = false;
	bool foldoutNeighboutID = false;
	
	void OnEnable () 
	{
		mSelf = (HexScript)target;
	}
	
	public override void OnInspectorGUI() 
	{
		EditorGUILayout.HelpBox("TILE INFO", MessageType.None);
		
		EditorGUILayout.LabelField("ID", mSelf.TileID.ToString ());
		
		foldoutNeighboutID = EditorGUILayout.Foldout(foldoutNeighboutID, "Neighbout ID");
		if (foldoutNeighboutID)
		{
			int size = mSelf.NeighbourIDList.Count;
			
			EditorGUILayout.LabelField("Size", size.ToString ());
			for(int i = 0; i < size; i++)
			{ EditorGUILayout.LabelField(i.ToString (), mSelf.NeighbourIDList[i].ToString ()); }
		}
		
		mSelf.isOpen = EditorGUILayout.Toggle("isOpen", mSelf.isOpen);
		
		mSelf.mTileType = (HexScript.TileType) EditorGUILayout.EnumPopup("Tile Type", mSelf.mTileType);
		if(mSelf.mTileType == HexScript.TileType.STORY_LOCK) mSelf.StoryTileNeeded = EditorGUILayout.IntField ("Tile Needed", mSelf.StoryTileNeeded);
		
		mSelf.mAreaType = (HexScript.AreaType) EditorGUILayout.EnumPopup("Area Type", mSelf.mAreaType);
		mSelf.mEncounterType = (HexScript.EncounterType) EditorGUILayout.EnumPopup("Encounter Type", mSelf.mEncounterType);
		mSelf.mTreasureChest = (HexScript.Treasure) EditorGUILayout.EnumPopup("Treasure", mSelf.mTreasureChest);
		
		mSelf.isUIText = EditorGUILayout.Toggle ("is UI Text", mSelf.isUIText);
		if(mSelf.isUIText) 
		{
			mSelf.mCamera = GameObject.FindGameObjectWithTag ("MainCamera").camera;
			mSelf.text = EditorGUILayout.TextField("Text", mSelf.text);
			
			mSelf.mUIAnchorPoint = (HexScript.UIAnchorPoint) EditorGUILayout.EnumPopup("Anchor Point", mSelf.mUIAnchorPoint);
			if((mSelf.mUIAnchorPoint == HexScript.UIAnchorPoint.LEFT && mSelf.widthAnchor != mSelf.width)
				|| (mSelf.mUIAnchorPoint == HexScript.UIAnchorPoint.CENTER && mSelf.widthAnchor != mSelf.width / 2)
				|| (mSelf.mUIAnchorPoint == HexScript.UIAnchorPoint.RIGHT && mSelf.widthAnchor != 0.0f))
			{
				mSelf.SetAnchorPoint();
				EditorUtility.SetDirty(mSelf);
			}
			
			mSelf.x_Offset = EditorGUILayout.FloatField("X Offset", mSelf.x_Offset);
			mSelf.y_Offset = EditorGUILayout.FloatField("Y Offset", mSelf.y_Offset);
			mSelf.width = EditorGUILayout.FloatField("Width", mSelf.width);
			mSelf.height = EditorGUILayout.FloatField("Height", mSelf.height);
			mSelf.GizmoColor = EditorGUILayout.ColorField("Gizmo Color", mSelf.GizmoColor);
		}
		else if(!mSelf.isUIText) mSelf.ResetUIField ();
		
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("OTHERS", MessageType.None);
		
		mSelf.isPreGenerateArea = EditorGUILayout.Toggle("GenerateArea", mSelf.isPreGenerateArea);
		mSelf.isDialog = EditorGUILayout.Toggle("Dialog", mSelf.isDialog);
		mSelf.TH_Y_Offset = EditorGUILayout.FloatField("TH Y Offset", mSelf.TH_Y_Offset);
		mSelf.RedOverlay_Y_Offset = EditorGUILayout.FloatField("Red Y Offset", mSelf.RedOverlay_Y_Offset);
		
		foldoutPrefab = EditorGUILayout.Foldout(foldoutPrefab, "Prefabs");
		if (foldoutPrefab)
		{
			mSelf.pTreasure = EditorGUILayout.ObjectField ("P.Treasure", mSelf.pTreasure, typeof(GameObject), false) as GameObject;
			mSelf.pHiddenTreasure = EditorGUILayout.ObjectField ("P.Hidden Treasure", mSelf.pHiddenTreasure, typeof(GameObject), false) as GameObject;
			mSelf.pSavePoint = EditorGUILayout.ObjectField ("P.Save Point", mSelf.pSavePoint, typeof(GameObject), false) as GameObject;
			mSelf.pPixie = EditorGUILayout.ObjectField ("P.Pixie", mSelf.pPixie, typeof(GameObject), false) as GameObject;
			mSelf.pBattleArea = EditorGUILayout.ObjectField ("P.Battle Area", mSelf.pBattleArea, typeof(GameObject), false) as GameObject;
			mSelf.pRedOverlay = EditorGUILayout.ObjectField ("P.Tough Area", mSelf.pRedOverlay, typeof(GameObject), false) as GameObject;
			mSelf.pNextFloor = EditorGUILayout.ObjectField ("P.Next Floor", mSelf.pNextFloor, typeof(GameObject), false) as GameObject;
			mSelf.pTH = EditorGUILayout.ObjectField ("P.TH", mSelf.pTH, typeof(GameObject), false) as GameObject;
		}

		foldoutTexture = EditorGUILayout.Foldout(foldoutTexture, "Textures");
		if (foldoutTexture)
		{
			mSelf.OpenTexture = EditorGUILayout.ObjectField ("T.Open", mSelf.OpenTexture, typeof(Texture2D), false) as Texture2D;
			mSelf.StoryOpenTexture = EditorGUILayout.ObjectField ("T.StoryOpen", mSelf.StoryOpenTexture, typeof(Texture2D), false) as Texture2D;
			mSelf.StoryLockTexture = EditorGUILayout.ObjectField ("T.StoryLock", mSelf.StoryLockTexture, typeof(Texture2D), false) as Texture2D;
			mSelf.LockTexture = EditorGUILayout.ObjectField ("T.Lock", mSelf.LockTexture, typeof(Texture2D), false) as Texture2D;
			mSelf.HoverOpenTexture = EditorGUILayout.ObjectField ("T.HoverOpen", mSelf.HoverOpenTexture, typeof(Texture2D), false) as Texture2D;
			mSelf.HoverLockTexture = EditorGUILayout.ObjectField ("T.HoverLock", mSelf.HoverLockTexture, typeof(Texture2D), false) as Texture2D;
			mSelf.ExtSearchTexture = EditorGUILayout.ObjectField ("T.ExtSearch", mSelf.ExtSearchTexture, typeof(Texture2D), false) as Texture2D;
		}
		
		if (GUI.changed) EditorUtility.SetDirty(mSelf);
	}
}
