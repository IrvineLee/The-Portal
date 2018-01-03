using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{
	public float moveSpeed = 5.0f;
	public int normalTile = 5;
	public int storyTile = 3;
	
	public int encounterRate = 15;
	public int incDecRate = 5;
	public int noOfCheckInterval = 1;
	public float checkInterval = 1.0f;
	public float invulnerableTime = 1.0f;
	
	public float hoverCursorY_Offset = 0.01f;
	public float hoverBlinkSpeed = 1.0f;
	
	public bool isDrawGizmo = false;
	public bool isTreasureHunter = false;
	
	public bool isUpdate = true;
	
	public enum Mode
	{
		EXPLORE = 0,
		UNLOCK,
		MENU,
		BATTLE,
		DIALOG,
		SHOP,
		NEXT_FLOOR
	};
	public Mode playerMode = Mode.EXPLORE;
	
	public enum UnlockMode
	{
		MOUSE = 0,
		KEYBOARD
	};
	public UnlockMode unlockMode = UnlockMode.MOUSE;
	
	public enum ShopMode
	{
		NONE = 0,
		TALK,
		COMBINE,
		BUFF,
		MINIGAME,
		EXIT
	};
	ShopMode mShopMode = ShopMode.NONE;
	
	// Handle movement.
	Vector3 mMoveDirection = Vector3.zero;
	
	// Determine where the player is standing.
	int mPlayerTileIDPrev = -1, mPlayerTileIDCurr = -1;
	float mRayCastDistance;
	bool mIsOnNewTile = false;
	bool mIsInteractionAgain = false;
	LayerMask mTileLayerMask = 1 << 8;
	HexScript mCurrTileScript, mPrevTileScript;
	
	// Determine where the player is pointing.
	int mHoverIDPrev = -1, mHoverIDCurr;
	GameObject mHoverTilePrev, mHoverTileCurr;
	
	// Handle Treasure Hunter skill.
	List<int> TreasureHunterList = new List<int>();
	bool mIsActivateTH= false;
	
	// Handle encounter rate.
	int DEFAULT_ENCOUNTER_RATE;
	int mSavedEncRate = -1;
	int mCurrCheckInterval = 0;
	float mIntervalTimer = 0.0f;
	float mInvulnerableTimer = 0.0f;
	Vector3 mPrevPos, mCurPos;
	bool isMove = false;
	bool mIsNoBattle = false;
	bool mIsMandatoryBattle = false;
	bool mIsWinBattle = false;
	
	/*GameObject mCursor;
	bool isSpawnCursor = false;*/
	
	public Transform mTHoverCursor;
	//Transform mTTreasureCursor;
	bool isCursorVisible = false;
	bool isDecreaseAlpha = false;
	
	// Resources cache.
	float mHexWidth = 0;
	Texture2D mTStoryTile, mTNormalTile;
	
	LayerMask mFOWLayerMask = 1 << 9;
	
	GameObject[] mFloorsGOArray;
	GameObject mCurrFloorGO;
	bool isNewFloor = false;
	
	// Handle dialog input.
	bool mIsGetResponse = false;
	bool mIsGetEffect = false;
	bool mIsReturnToShopMenu = false;
	
	CharacterController charController;
	HexGenerator mFloor;
	FOWScript mFOWScript;
	FOWRevealer mFOWRevealer;
	CameraScript mCameraScript;
	SceneManager mSceneManager;
	UIScript mUIScript;
	DialogScript mDialogScript;
	
	void Awake()
	{
		//Screen.showCursor = false;
		CacheAndDeactivateFloors();
	}
	
	void Start () 
	{
		mTHoverCursor = GameObject.FindGameObjectWithTag("HoverCursor").transform;
		//mTTreasureCursor = GameObject.FindGameObjectWithTag("HoverCursor").transform;
		/*mCursor = GameObject.FindGameObjectWithTag ("Cursor").gameObject;
		mCursor.GetComponent<MeshRenderer>().enabled = false;*/
		
		charController = GetComponent<CharacterController>();
		mFloor = GameObject.FindGameObjectWithTag ("Floor1").GetComponent<HexGenerator>();
		mCurrFloorGO = mFloor.gameObject;
		
		DEFAULT_ENCOUNTER_RATE = encounterRate;
		
		mFOWScript = GameObject.FindGameObjectWithTag("FOW").GetComponent<FOWScript>();
		mFOWRevealer = GameObject.FindGameObjectWithTag("Player").GetComponent<FOWRevealer>();
		mCameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
		mSceneManager = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>();
		mUIScript = GetComponent<UIScript>();
		
		GameObject hex = Resources.Load("Hex") as GameObject;
		mHexWidth = hex.renderer.bounds.size.x;
		
		mTStoryTile = Resources.Load ("StoryTileLock") as Texture2D;
		mTNormalTile = Resources.Load ("OpaqueHexTexture") as Texture2D;
	}
	
	void Update () 
	{
		if(!isUpdate) return;

		if(playerMode == Mode.EXPLORE)
		{
			if(mCameraScript.isMovingPlayer) return;
			if(!mCameraScript.RotationActive) mCameraScript.RotationActive = true; 
			ScanArea();
			HandleMovement();
			HandleEncounter();
			HandleAction();
			
			if(Input.GetMouseButtonDown(1)) ChangeMode();
		}
		else if(playerMode == Mode.UNLOCK) 
		{
			if(unlockMode == UnlockMode.MOUSE) HandleUnlockModeMouse();
			else if(unlockMode == UnlockMode.KEYBOARD) HandleUnlockModeKeyboard();
			
			if(Input.GetMouseButtonDown(1)) ChangeMode();
		}
		else if(playerMode == Mode.BATTLE)
		{
			if(AutoFade.Fading) // This is called when returning to world map.
			{
				if(mCurrTileScript.mAreaType == HexScript.AreaType.BATTLE_TOUGH_ENEMY && InfoExchange.IsWinBattle)			
				{ mCurrTileScript.ClearedRedTile (); }
				else if(!InfoExchange.IsWinBattle) 
				{
					Debug.Log ("Receive debuff.");
					mIsInteractionAgain = true;
				}
				playerMode = Mode.EXPLORE;
			}
			else
			{
				isUpdate = false;
				mSceneManager.ChangeScene (mSceneManager.worldMapScene, mSceneManager.battleScene);
			}
		}
		else if(playerMode == Mode.DIALOG) HandleDialog();
		else if(playerMode == Mode.SHOP) HandleShop();
		else if(playerMode == Mode.NEXT_FLOOR) HandleNextFloor();
	}
	
	public ShopMode Shop
	{
		get{ return this.mShopMode; }
		set{ this.mShopMode = value; }
	}
	
	public void NoEncRate()
	{
		mSavedEncRate = encounterRate;
		encounterRate = 0;
		mIsNoBattle = true;
	}
	
	public void MandatoryEncRate()
	{
		encounterRate = 100;
		mIsMandatoryBattle = true;
	}
	
	// Determine type of tile the player is standing on. 
	// Also handles Treasure Hunter skill.
	void ScanArea()
	{
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		mRayCastDistance = 2.5f;
		
		if(Physics.Raycast (ray, out hit, mRayCastDistance, mTileLayerMask))
		{
			if(mIsOnNewTile) mIsOnNewTile = false;
			
			HexScript tile = hit.collider.gameObject.GetComponent<HexScript>();
			if(mPlayerTileIDCurr != tile.TileID)
			{
				if(isNewFloor) 
				{
					tile.HandleUnlock();
					isNewFloor = false;
				}
				
				mPlayerTileIDPrev = mPlayerTileIDCurr;
				mPlayerTileIDCurr = tile.TileID;
				mIsOnNewTile = true;

				if(mSavedEncRate != -1) 
				{
					encounterRate = mSavedEncRate;
					mIsNoBattle = false;
					mSavedEncRate = -1;
				}
				tile.GetEncounterRate();
			}
			
			//! TODO: If acquire treasure hunter skill, then replace 'Space' key below.
			if (Input.GetKeyDown(KeyCode.Space)) 
			{
				if(!isTreasureHunter) 
				{
					isTreasureHunter = true; 
					mIsActivateTH = true;
				}
				else if(isTreasureHunter) 
				{
					ClearTreasureHunterList();
					isTreasureHunter = false; 
					mIsActivateTH = false;
				}
			}
			
			if(isTreasureHunter && (mIsOnNewTile || mIsActivateTH))
			{
				ClearTreasureHunterList();
				
				// Change to extended search texture.
				foreach(int id in hit.collider.gameObject.GetComponent<HexScript>().NeighbourIDList)
				{
					TreasureHunterList.Add(id);
					mFloor.mTileArray[id].tile.GetComponent<HexScript>().EnableExtendedSearch();
				}
				
				if(mIsActivateTH) mIsActivateTH = false;
			}
		}
	}
	
	// Handle simple movement.
	void HandleMovement()
	{
		mMoveDirection = Vector3.zero;
		if (Input.GetKey(KeyCode.W)) mMoveDirection += transform.TransformDirection(Vector3.forward);
		if (Input.GetKey(KeyCode.S)) mMoveDirection += transform.TransformDirection(Vector3.back);
		if (Input.GetKey(KeyCode.A)) mMoveDirection += transform.TransformDirection(Vector3.left);
		if (Input.GetKey(KeyCode.D)) mMoveDirection += transform.TransformDirection(Vector3.right);

        mMoveDirection *= moveSpeed;
        charController.Move(mMoveDirection * Time.deltaTime);
		
		mCurPos = transform.position;
		if(mCurPos != mPrevPos)
		{
			isMove = true;
			mPrevPos = mCurPos;
		}
		else isMove = false;
	}
	
	// Handle action.
	void HandleAction()
	{
		if(mIsOnNewTile || mIsInteractionAgain) 
		{ 
			mIsInteractionAgain = false;
			mCurrTileScript = mFloor.mTileArray[mPlayerTileIDCurr].tile.GetComponent<HexScript>(); 
			
			if(mCurrTileScript.mAreaType == HexScript.AreaType.NONE
				&& mCurrTileScript.mTreasureChest == HexScript.Treasure.NONE)
			{ 
				if(mUIScript.IsLabelShowing) mUIScript.DisableInteraction(); 
				else if(!mUIScript.IsLabelShowing) return;
			}
			
			if(mCurrTileScript.mAreaType != HexScript.AreaType.NONE)
			{ 
				mUIScript.ShowInteraction(mCurrTileScript.InteractionMsg); 
			}
			else if(mCurrTileScript.mTreasureChest == HexScript.Treasure.SHOWN)
			{
				mUIScript.ShowInteraction("Open!"); 
			}
		}
		
		if(Input.GetMouseButtonDown(0))
		{
			if(mCurrTileScript.mAreaType != HexScript.AreaType.NONE)
			{ 
				if(mCurrTileScript.mAreaType == HexScript.AreaType.PIXIE)
				{ 
					mUIScript.DisableInteraction(); 
					mIsInteractionAgain = true;
				}
				
				mCurrTileScript.HandleTrigger(); 
			}
			else if(mCurrTileScript.mTreasureChest == HexScript.Treasure.SHOWN)
			{ 
				mCurrTileScript.OpenTreasureChest(); 
				mUIScript.DisableInteraction(); 
			}
		}
	}
	
	// Handle encounter.
	void HandleEncounter()
	{
		if(isMove && !mIsNoBattle)
		{
			mInvulnerableTimer += Time.deltaTime;
			
			if(mInvulnerableTimer > invulnerableTime || mIsMandatoryBattle)
			{
				mIntervalTimer += Time.deltaTime;
				
				if(mIntervalTimer > checkInterval || mIsMandatoryBattle)
				{
					float number = Random.Range(0.0f, 1.0f);
					float mEncounterPercent = (float)encounterRate / 100.0f;
					
					Debug.Log (number + "  " + mEncounterPercent);
					if(number <= mEncounterPercent) 
					{
						playerMode = Mode.BATTLE;
						encounterRate = DEFAULT_ENCOUNTER_RATE;
						mInvulnerableTimer = 0.0f;
						mIsMandatoryBattle = false;
					}
					else // Increase encounter rate.
					{
						mCurrCheckInterval += 1;
						if(mCurrCheckInterval == noOfCheckInterval)
						{
							encounterRate += incDecRate;
							mCurrCheckInterval = 0;
						}
					}
					mIntervalTimer = 0.0f;
				}
			}
		}
	}
	
	void HandleUnlockModeKeyboard()
	{
		if(!isCursorVisible) 
		{
			if(mCameraScript.isMovingPlayer) return;
			DisplayCursor();
		}
		if(isCursorVisible) BlinkingAlpha(mTHoverCursor.gameObject);
		
		if (Input.GetKeyDown(KeyCode.W)) HandleTileMovementKeyboard(transform.forward);
		if (Input.GetKeyDown(KeyCode.S)) HandleTileMovementKeyboard(-transform.forward);
		if (Input.GetKeyDown(KeyCode.A)) HandleTileMovementKeyboard(-transform.right);
		if (Input.GetKeyDown(KeyCode.D)) HandleTileMovementKeyboard(transform.right);
		
		if(Input.GetMouseButtonDown(0)) KeyboardUnlock ();
	}
	
	void DisplayCursor()
	{
		// mHoverIDCurr == 0 is the default value.
		if(mHoverIDCurr == 0) mHoverIDCurr = mPlayerTileIDCurr;
		
		Vector3 hoverTilePos = mFloor.mTileArray[mHoverIDCurr].tile.transform.position;
		hoverTilePos.y += hoverCursorY_Offset;

		mTHoverCursor.GetComponent<MeshRenderer>().enabled = true;
		mTHoverCursor.position = hoverTilePos;

		isCursorVisible = true;
	}
	
	void BlinkingAlpha(GameObject go)
	{
		Color color = go.renderer.material.color;
		
		if(color.a >= 1.2f) isDecreaseAlpha = true;
		else if(color.a <= -0.2f) isDecreaseAlpha = false;
		
		if(isDecreaseAlpha) color.a -= 0.01f * hoverBlinkSpeed;
		else if(!isDecreaseAlpha) color.a += 0.01f * hoverBlinkSpeed;
		
		go.renderer.material.color = color;
	}
	
	void HandleTileMovementKeyboard(Vector3 dirRelativeToPlayer)
	{
		float rayCastHeight = 1.5f;
		
		Vector3 currTilePos = mFloor.mTileArray[mHoverIDCurr].tile.transform.position;
		Vector3 startPos = currTilePos + (dirRelativeToPlayer * mHexWidth);
		startPos.y += rayCastHeight;
		
		Ray ray = new Ray(startPos, -transform.up);
		RaycastHit hit;
		mRayCastDistance = 2.5f;
		//Debug.DrawRay(startPos, -transform.up * mRayCastDistance, Color.red, 2.0f);
		
		// If hit a tile, handle hover.
		if(Physics.Raycast (ray, out hit, mRayCastDistance, mTileLayerMask))
		{
			if(!IsObscuredByFOW(hit, rayCastHeight)) HandleHover(hit); 
			else RaycastSurroundingArea(ray);
		}
		else RaycastSurroundingArea(ray); // For a more flexible tile movement.
	}
	
	// This will raycast in a circle of 4 points. (Circle raycast)
	void RaycastSurroundingArea(Ray ray)
	{
		RaycastHit hit;
		int segments = 4;
		float segmentAngle = (2.0f * Mathf.PI) / (float)segments;
		float offset = mHexWidth * 0.15f;

		for(int i = 0; i < segments; i++)
		{
			float angle = segmentAngle * i;
			Vector3 pos = new Vector3(ray.origin.x + (Mathf.Cos(angle) * offset), ray.origin.y, ray.origin.z + (Mathf.Sin(angle) * offset));
			
			Ray newRay = new Ray(pos, ray.direction);
			
			Debug.DrawRay(pos, -transform.up * mRayCastDistance, Color.green, 2.0f);
			if(Physics.Raycast (newRay, out hit, mRayCastDistance, mTileLayerMask))
			{
				float rayCastHeight = 1.5f;
				if(!IsObscuredByFOW(hit, rayCastHeight)) 
				{ 
					if(mHoverIDCurr != hit.collider.GetComponent<HexScript>().TileID)
					{
						HandleHover(hit); 
						break;
					}
				}
			}
		}
	}
	
	bool IsObscuredByFOW(RaycastHit hit, float rayCastHeight)
	{
		Vector3 pos = hit.collider.gameObject.transform.position;
		pos.y += rayCastHeight;
		Ray ray = new Ray(pos, -transform.up);
		
		if(Physics.Raycast (ray, out hit, mRayCastDistance, mFOWLayerMask))
		{
			int[] trianglesArray = mFOWScript.mesh.triangles;
			
			for(int i = 0; i < 3; i++)
			{
				int vertex = trianglesArray[hit.triangleIndex * 3 + i];
				
				if(mFOWScript.colors[vertex].a > 0.75f) return true;
			}
		}
		return false;
	}
	
	// Handle Unlock Mode using mouse.
	void HandleUnlockModeMouse()
	{
		//if(isSpawnCursor) SpawnCursor();
			
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		mRayCastDistance = 100.0f;
		
		if(Physics.Raycast (ray, out hit, mRayCastDistance, mTileLayerMask))
		{
			float rayCastHeight = 1.5f;
			if(!IsObscuredByFOW(hit, rayCastHeight)) HandleHover(hit);
			MouseUnlock(hit);
		}
		else // If not pointing at the hexgrid, revert tile to original texture.
		{
			if(mHoverIDPrev != -1) // If previously already pointed to a tile.
			{
				mHoverTilePrev.GetComponent<HexScript>().SetDefaultTexture ();
				mHoverIDPrev = -1;
			}
		}
	}
	
	// Handle hover texture during Unlock Mode.
	void HandleHover(RaycastHit hit)
	{
		mHoverTileCurr = hit.collider.gameObject;
		HexScript tile = mHoverTileCurr.GetComponent<HexScript>();
		
		// Change the tile texture to hovering for the tile you're pointing.
		mHoverIDCurr = tile.TileID;		
		if(mHoverIDCurr != mHoverIDPrev) // If pointing to a different tile.
		{
			if(unlockMode == UnlockMode.KEYBOARD) 
			{ mCameraScript.MovePlayerPos (hit.transform.position); }
			else if(unlockMode == UnlockMode.MOUSE) 
			{
				if(mHoverIDPrev != -1) mHoverTilePrev.GetComponent<HexScript>().SetDefaultTexture ();
				tile.EnableHoverTexture ();
			}

			mHoverTilePrev = mHoverTileCurr;
			mHoverIDPrev = mHoverIDCurr;
		}
	}
	
	void MouseUnlock(RaycastHit hit)
	{
		HexScript tile = hit.collider.gameObject.GetComponent<HexScript>();
		Unlock(tile);
	}
	
	void KeyboardUnlock()
	{
		HexScript tile = mFloor.mTileArray[mHoverIDCurr].tile.GetComponent<HexScript>();
		Unlock(tile);
	}
	
	// Handle unlocking tiles during Unlock Mode.
	void Unlock(HexScript tile)
	{
		HexScript.TileType tileType = tile.mTileType;
		
		// Check to see whether can be unlocked.
		if (Input.GetMouseButtonDown(0) && !tile.isOpen)
		{
			if((tileType == HexScript.TileType.NORMAL && normalTile == 0)
				|| tileType == HexScript.TileType.STORY_LOCK && storyTile == 0)
			{
				Debug.Log ("No more tiles.");
				return;
			}
			else if(tileType == HexScript.TileType.STORY_LOCK && storyTile < tile.StoryTileNeeded)
			{
				Debug.Log ("Insufficient tiles.");
				return;
			}
			
			// Check if the clicked tile neighbours are opened. 
			// This is to ensure players can only unlock connecting tiles.
			foreach(int id in tile.NeighbourIDList)
			{
				if(mFloor.mTileArray[id].tile.GetComponent<HexScript>().isOpen)
				{
					if(tileType == HexScript.TileType.NORMAL) normalTile -= 1;
					else if(tileType == HexScript.TileType.STORY_LOCK) storyTile -= tile.StoryTileNeeded;
					tile.HandleUnlock();
					Debug.Log ("ID: " + tile.TileID);
					break;
				}
			}
		}
	}
	
	// Change between Explore Mode and Unlock Mode.
	void ChangeMode()
	{
		if(playerMode == Mode.EXPLORE && !mCameraScript.isChangingCam) // Change mode to Unlock.
		{ 
			playerMode = Mode.UNLOCK; 
			mCameraScript.ChangeCamMode();
			mCameraScript.MovePlayerPos (mFloor.mTileArray[mPlayerTileIDCurr].tile.transform.position); 
			mUIScript.DisableInteraction ();
			ResetHoverToDefaultVal();
			//isSpawnCursor = true;
			Debug.Log("Unlock tile mode."); 
		}
		else if(playerMode == Mode.UNLOCK && !mCameraScript.isChangingCam) // Change mode to Explore.
		{ 
			playerMode = Mode.EXPLORE; 
			mCameraScript.ChangeCamMode();
			mIsInteractionAgain = true;
			if(!mCameraScript.RotationActive) mCameraScript.RotationActive = true; 
			
			if(unlockMode == UnlockMode.MOUSE && mHoverTileCurr != null)
			{  mHoverTileCurr.GetComponent<HexScript>().SetDefaultTexture (); }
			
			ResetHoverToDefaultVal();
			//mCursor.GetComponent<MeshRenderer>().enabled = false;
			Debug.Log("Explore mode"); 
		}
	}
	
	void HandleDialog()
	{
		if(mDialogScript == null)
		{
			DialogScript[] dialogScriptArray = mFloor.mTileArray[mPlayerTileIDCurr].tile.GetComponents<DialogScript>();
			foreach(DialogScript script in dialogScriptArray)
			{
				if(mShopMode == ShopMode.NONE && script.mTalkScene == DialogScript.TalkScene.NORMAL)
				{ mDialogScript = script; break; }
				else if(mShopMode == ShopMode.TALK && script.mTalkScene == DialogScript.TalkScene.SHOP)
				{
					mDialogScript = script; 
					mIsReturnToShopMenu = true;
					break;
				}
			}
			
			if(mShopMode == ShopMode.TALK && mDialogScript == null)
			{
				mIsReturnToShopMenu = true;
				mCurrTileScript.IsTriggerMain = true;
				mCurrTileScript.HandleTrigger ();
				Debug.Log ("There are no dialog script for shop. Please attach a new dialog script.");
				return;
			}
			
			if(mDialogScript.IsDialogFirst) mUIScript.ShowDialog (mDialogScript.GetDialog);
			else if(!mDialogScript.IsDialogFirst) mUIScript.ShowDialogInteraction (mDialogScript.GetInteraction);
		}
		else if(Input.GetMouseButtonDown(0) || mUIScript.IsAnswered)
		{
			if(TypeWriterStyle.IsTexting) 
			{
				mUIScript.ShowFullText ();
				return;
			}
			
			// If still haven't log in any replies, refrain from getting next dialog.
			if(mUIScript.IsAwaitingReply) return;
			else if(mUIScript.IsAnswered) 
			{
				mDialogScript.RegisterAnsweredIndex(mUIScript.GetAnswerIndex);
				
				if(!mDialogScript.IsLastResponse) mIsGetResponse = true;
				else if(mDialogScript.IsLastResponse) mIsGetEffect = true;

				mUIScript.IsAnswered = false;
			}
			
			if(!mDialogScript.IsTalkEnd || mIsGetResponse) 
			{
				// Get response after logging in answer.
				if(mIsGetResponse)
				{
					string response = mDialogScript.GetResponse();
					mUIScript.ShowDialog (response);
					
					if(mDialogScript.IsLastResponse) 
					{
						mIsGetResponse = false;
						mIsGetEffect = true;
						return;
					}
				}
				// Get next dialog. If null, get interaction.
				else if(mUIScript.NextDialog (mDialogScript.GetDialog) == null)
				{
					mUIScript.ShowDialogInteraction(mDialogScript.GetInteraction);
				}
			}
			else if(mDialogScript.IsTalkEnd)
			{
				if(mIsGetEffect) EffectOfAnswer(mDialogScript.GetAnswerEffect);
				else if(!mIsGetEffect) mCurrTileScript.HandleTrigger ();
				ResetDialog(); 
			}
		}
	}
	
	void EffectOfAnswer(Response.AnswerEffect effect)
	{
		if(effect == Response.AnswerEffect.NONE) 
		{ 
			playerMode = Mode.EXPLORE; 
			mCurrTileScript.IsTriggerMain = false;
		}
		else if(effect == Response.AnswerEffect.TILE) 
		{ 
			mCurrTileScript.HandleTrigger ();
		}
		else if(effect == Response.AnswerEffect.BATTLE) 
		{ 
			if(mCurrTileScript.mAreaType == HexScript.AreaType.BATTLE_AREA ||
				mCurrTileScript.mAreaType == HexScript.AreaType.BATTLE_TOUGH_ENEMY)
			{
				mCurrTileScript.HandleTrigger ();
			}
			else playerMode = Mode.BATTLE; 
		}
		else if(effect == Response.AnswerEffect.ITEM) { Debug.Log ("Get Item!"); }
	}
	
	void HandleShop()
	{
		if(mUIScript.GetUIType == UIScript.UIType.SHOP)
		{ if(Input.GetMouseButtonDown(1)) mShopMode = ShopMode.EXIT; }
		else if(mUIScript.GetUIType != UIScript.UIType.SHOP) 
		{
			SetOverheadUIVisible(false);
			mUIScript.ShowUI (UIScript.UIType.SHOP);
		}
		
		if(mIsReturnToShopMenu) 
		{
			mUIScript.UIButtonVisible (true);
			mShopMode = ShopMode.NONE;
			mIsReturnToShopMenu = false;
		}
		
		if(mShopMode == ShopMode.TALK)
		{
			mUIScript.UIButtonVisible (false);
			mCurrTileScript.HandleTrigger ();
		}
		else if(mShopMode == ShopMode.COMBINE)
		{
			
		}
		else if(mShopMode == ShopMode.BUFF)
		{
			
		}
		else if(mShopMode == ShopMode.EXIT)
		{
			playerMode = Mode.EXPLORE;
			mShopMode = ShopMode.NONE;
			mUIScript.DisableUI();
			SetOverheadUIVisible(true);
		}
	}
	
	void HandleNextFloor()
	{
		mSceneManager.FadeScreen ();
		if(AutoFade.IsFadeOutSolid) 
		{
			MoveUpNextFloor();
			mFOWScript = GameObject.FindGameObjectWithTag("FOW").GetComponent<FOWScript>();
			mPlayerTileIDCurr = -1;
			mPlayerTileIDPrev = -1;
			mFOWRevealer.CacheNewFOW ();
			playerMode = Mode.EXPLORE;
			isNewFloor = true;
		}
	}
	
	void MoveUpNextFloor()
	{
		mFloor.gameObject.SetActive (false);
		
		GameObject nextFloor = GetFloorGO(mFloor.tag, true);
		nextFloor.SetActive (true);
		mFloor = nextFloor.GetComponent<HexGenerator>();
		mCurrFloorGO = nextFloor;
		
		transform.position = CenterPlayerToCurrTile();
	}
	
	Vector3 CenterPlayerToCurrTile()
	{	
		Vector3 pos = transform.position;
		pos.y = mFloor.transform.position.y + transform.collider.bounds.size.y / 2;
		transform.position = pos;
		
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		mRayCastDistance = 2.5f;
		
		if(Physics.Raycast (ray, out hit, mRayCastDistance, mTileLayerMask))
		{
			HexScript tile = hit.collider.gameObject.GetComponent<HexScript>();
			
			Vector3 currPos = transform.position;
			currPos.x = tile.transform.position.x;
			currPos.z = tile.transform.position.z;
			
			return currPos;
		}
		Debug.Log ("Centering player to curr tile failed!");
		return Vector3.zero;
	}
	
	/*void SpawnCursor()
	{
		mCursor.GetComponent<MeshRenderer>().enabled = true;
		
		Vector3 pos = transform.position;
		pos.y += 1.5f;
		mCursor.transform.position = pos;
		
		isSpawnCursor = false;
	}*/
	
	void ClearTreasureHunterList()
	{
		if(TreasureHunterList.Count == 0) return;
		// Revert to original texture.

		foreach(int id in TreasureHunterList)
		{
			mFloor.mTileArray[id].tile.GetComponent<HexScript>().DisableExtendedSearch();
		}
		TreasureHunterList.Clear();
	}
	
	void ResetHoverToDefaultVal()
	{
		isCursorVisible = false;
		mTHoverCursor.GetComponent<MeshRenderer>().enabled = false;
		mHoverIDPrev = -1;
		mHoverIDCurr = 0;
		mHoverTilePrev = null;
		mHoverTileCurr = null;
	}
	
	void ResetDialog()
	{
		mIsGetEffect = false;
		mUIScript.DisableDialog(); 
		mDialogScript.ResetIndexCount();
		mDialogScript = null;
	}
	
	void OnGUI()
	{
		GUI.skin.label.fontStyle = FontStyle.Bold;
		
		GUI.skin.label.fontSize = 17;
		GUI.Label(new Rect(0.85f * Screen.width, 0.06f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "Floor 1");
			
		if(playerMode == Mode.EXPLORE) 
		{
			GUI.skin.label.fontSize = 20;
			GUI.Label(new Rect(0.76f * Screen.width, 0.022f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "Explore Mode");
		}
		else if(playerMode == Mode.UNLOCK) 
		{
			GUI.skin.label.fontSize = 20;
			GUI.Label(new Rect(0.76f * Screen.width, 0.022f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "Unlock Mode");
			
			// Handle texture.
			GUI.Box (new Rect(0.05f * Screen.width, 0.85f * Screen.height, 0.06f * Screen.width, 0.075f * Screen.height), "");
			GUI.DrawTexture (new Rect(0.05f * Screen.width, 0.85f * Screen.height, 0.06f * Screen.width, 0.075f * Screen.height), mTStoryTile);
			GUI.Box (new Rect(0.25f * Screen.width, 0.85f * Screen.height, 0.06f * Screen.width, 0.075f * Screen.height), "");
			GUI.DrawTexture (new Rect(0.25f * Screen.width, 0.85f * Screen.height, 0.06f * Screen.width, 0.075f * Screen.height), mTNormalTile);
			
			GUI.skin.label.fontStyle = FontStyle.Normal;
			GUI.skin.label.fontSize = 12;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(0.12f * Screen.width, 0.85f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "Story Tiles : " + storyTile.ToString ());	
			GUI.Label(new Rect(0.32f * Screen.width, 0.85f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "Normal Tiles : " + normalTile.ToString ());
		}
	}
			
	void OnDrawGizmos()
	{
		if(isDrawGizmo)
		{
			Gizmos.color = Color.cyan;
        	Gizmos.DrawRay(transform.position, -transform.up);
		}
	}
	
	public void Save()
	{
		PlayerPrefsX.SetVector3(transform.name, CenterPlayerToCurrTile());
		PlayerPrefs.SetString ("mFloor", mFloor.tag.ToString());
		
		PlayerPrefs.SetFloat("moveSpeed", moveSpeed);
		PlayerPrefs.SetInt("normalTile", normalTile);
		PlayerPrefs.SetInt("storyTile", storyTile);
		
		PlayerPrefs.SetInt("encounterRate", encounterRate);
		PlayerPrefs.SetInt("incDecRate", incDecRate);
		PlayerPrefs.SetInt("noOfCheckInterval", noOfCheckInterval);
		PlayerPrefs.SetFloat("checkInterval", checkInterval);
		PlayerPrefs.SetFloat("invulnerableTime", invulnerableTime);
		
		PlayerPrefs.SetInt("unlockMode", (int)unlockMode);
		PlayerPrefsX.SetBool ("isTreasureHunter", isTreasureHunter);
	}
	
	public void Load()
	{
		transform.position = PlayerPrefsX.GetVector3(transform.name);
		
		string floorString = PlayerPrefs.GetString("mFloor");
		
		if(mFloor == null) // When loading at Start screen.
		{
			mFloor = GameObject.FindGameObjectWithTag (floorString).GetComponent<HexGenerator>();
			mFOWRevealer = GameObject.FindGameObjectWithTag("Player").GetComponent<FOWRevealer>();
			
			mCurrFloorGO = mFloor.gameObject;
		}
		else if(mFloor.tag.ToString () != floorString) // When loading in-game.
		{
			mFloor.gameObject.SetActive (false);
			
			GameObject loadFloor = GetFloorGO(floorString, false);
			loadFloor.gameObject.SetActive(true);
			mFloor = loadFloor.GetComponent<HexGenerator>();
			mCurrFloorGO = mFloor.gameObject;
		}
		mFOWRevealer.CacheNewFOW ();
		
		moveSpeed = PlayerPrefs.GetFloat("moveSpeed");
		normalTile = PlayerPrefs.GetInt("normalTile");
		storyTile = PlayerPrefs.GetInt("storyTile");
		
		encounterRate = PlayerPrefs.GetInt("encounterRate");
		incDecRate = PlayerPrefs.GetInt("incDecRate");
		noOfCheckInterval = PlayerPrefs.GetInt("noOfCheckInterval");
		checkInterval = PlayerPrefs.GetFloat("checkInterval");
		invulnerableTime = PlayerPrefs.GetFloat("invulnerableTime");
		
		unlockMode = (UnlockMode) PlayerPrefs.GetInt("unlockMode");
		isTreasureHunter = PlayerPrefsX.GetBool ("isTreasureHunter");
	}
	
	public GameObject CurrFloorGO
	{
		get { return mCurrFloorGO; }
	}
	
	void CacheAndDeactivateFloors()
	{
		string[] floorTag = {"Floor1", "Floor2", "Floor3"};
		mFloorsGOArray = new GameObject[floorTag.Length];
		
		for(int i = 0; i < floorTag.Length; i++)
		{
			mFloorsGOArray[i] = GameObject.FindGameObjectWithTag (floorTag[i]).gameObject;
			if(mFloorsGOArray[i] != null) 
			{
				//CacheTileWithOverheadUI(mFloorsGOArray[i]);
				if(floorTag[i] == "Floor1") continue;
				mFloorsGOArray[i].SetActive (false);
			}
		}
	}
	
	void SetOverheadUIVisible(bool active)
	{
		foreach(HexScript hex in HexBaseScript.IsOverheadUIList)
		{
			hex.SetUIVisible (active);
		}
	}
	
	GameObject GetFloorGO(string floor, bool isFloorUp)
	{
		GameObject floorGo = null;
		int incrementVal = 0;
		
		if(isFloorUp) incrementVal += 1;
		
		if(floor == "Floor1") floorGo = mFloorsGOArray[0 + incrementVal];	// Assign floor 1.
		else if(floor == "Floor2") floorGo = mFloorsGOArray[1 + incrementVal]; // Assign floor 2.
		else if(floor == "Floor3")
		{
			int count = mFloorsGOArray.Length - 1;
			if(count == count + incrementVal) 
			{
				Debug.Log ("Cannot go up anymore floors.");
				return null;
			}
			
			floorGo = mFloorsGOArray[2 + incrementVal]; // Assign floor 3.
		}
		
		return floorGo;
	}
}
