using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
	PlayerController mSelf;
	
	void OnEnable()
	{
		mSelf = (PlayerController)target;
	}
	
	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox("PLAYER", MessageType.None);
		mSelf.moveSpeed = EditorGUILayout.FloatField("Move Speed", mSelf.moveSpeed);
		mSelf.normalTile = EditorGUILayout.IntField("Normal Tile", mSelf.normalTile);
		mSelf.storyTile = EditorGUILayout.IntField("Story Tile", mSelf.storyTile);
		
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("ENCOUNTER", MessageType.None);
		mSelf.checkInterval = EditorGUILayout.Slider("Check Interval(s)", mSelf.checkInterval, 0.1f, 3.0f);
		mSelf.encounterRate = EditorGUILayout.IntSlider("Encounter Rate(%)", mSelf.encounterRate, 0, 100);
		mSelf.noOfCheckInterval = EditorGUILayout.IntSlider("+ Rate Every(CI)", mSelf.noOfCheckInterval, 1, 3);
		mSelf.incDecRate = EditorGUILayout.IntSlider("+ Rate(%)", mSelf.incDecRate, 0, 10);
		mSelf.invulnerableTime = EditorGUILayout.Slider("Inv. Time(s)", mSelf.invulnerableTime, 0, 3);
		
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("OTHERS", MessageType.None);
		mSelf.hoverBlinkSpeed = EditorGUILayout.FloatField("Hover Blink Spd", mSelf.hoverBlinkSpeed);
		mSelf.hoverCursorY_Offset = EditorGUILayout.FloatField("Hover Y Offset", mSelf.hoverCursorY_Offset);
		mSelf.unlockMode = (PlayerController.UnlockMode) EditorGUILayout.EnumPopup("Unlock Mode", mSelf.unlockMode);
		mSelf.isDrawGizmo = EditorGUILayout.Toggle("isDrawGizmo", mSelf.isDrawGizmo);
		mSelf.isTreasureHunter = EditorGUILayout.Toggle("isTreasureHunter", mSelf.isTreasureHunter);
	}
}
