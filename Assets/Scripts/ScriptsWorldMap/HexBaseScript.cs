using UnityEngine;
using System.Collections.Generic;

public class HexBaseScript : MonoBehaviour 
{
	public GameObject pTreasure, pHiddenTreasure, pSavePoint, pPixie, pBattleArea, pRedOverlay, pNextFloor, pTH;
	public Texture2D OpenTexture, LockTexture, StoryOpenTexture, StoryLockTexture, HoverOpenTexture, HoverLockTexture, ExtSearchTexture;
	public float TH_Y_Offset = 0.003f;
	public float RedOverlay_Y_Offset = 0.006f;
	public static List<HexScript> IsOverheadUIList = new List<HexScript>();
	
	protected bool mIsEditMode = true;
	
	protected PlayerController mPlayerController;
	protected CameraScript mCameraScript;
	protected FOWScript mFOWScript;
	protected SaveManager mSaveManager;
	
	void Awake()
	{
		mPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		mCameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
		mFOWScript = GameObject.FindGameObjectWithTag("FOW").GetComponent<FOWScript>();
		mSaveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
		mIsEditMode = false;
	}
}
