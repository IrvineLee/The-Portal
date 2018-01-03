//! This script updates in edit mode.
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class HexScript : HexBaseScript 
{
	public int TileID;
	public List<int> NeighbourIDList = new List<int>();
	public int StoryTileNeeded = 1;
	public bool isPreGenerateArea = false;
	public bool isOpen = false;
	public bool isDialog = false;
	
	// Handle on-screen UI for area-of-interest.
	public Camera mCamera;
	public string text;
	public float x_Offset; 
	public float y_Offset; 
	public float widthAnchor;
	public float width = DEFAULT_WIDTH;
	public float height = DEFAULT_HEIGHT;
	public bool isUIText = false;
	public Color GizmoColor = Color.yellow;
	
	public enum TileType
	{
		NORMAL = 0,
		STORY_LOCK,
		
	};
	public TileType mTileType = TileType.NORMAL;
	
	public enum AreaType
	{
		NONE = 0,
		SAVE_POINT,
		PIXIE,
		BATTLE_AREA,
		BATTLE_TOUGH_ENEMY,
		NEXT_FLOOR
	};
	public AreaType mAreaType = AreaType.NONE;
	
	public enum EncounterType
	{
		NONE = 0,
		NORMAL,
		MANDATORY
	};
	public EncounterType mEncounterType = EncounterType.NORMAL;
	
	public enum Treasure
	{
		NONE = 0,
		HIDDEN,
		SHOWN
	};
	public Treasure mTreasureChest = Treasure.NONE;
	
	public enum UIAnchorPoint
	{
		LEFT = 0,
		CENTER,
		RIGHT
	};
	public UIAnchorPoint mUIAnchorPoint = UIAnchorPoint.RIGHT;
	
	LayerMask mFOWLayerMask = 1 << 9;
	
	[SerializeField]
	Vector2 mUIScreenPos;
	
	const float DEFAULT_X_OFFSET = 0.0f;
	const float DEFAULT_Y_OFFSET = 30.0f;
	const float DEFAULT_WIDTH = 50.0f;
	const float DEFAULT_HEIGHT = 20.0f;
	
	[SerializeField]
	AreaType mPrevAreaType;
	MeshRenderer mExtSearchRenderer;
	bool mIsExtSearch = false;
	bool mHasExtSearch = false;
	
	string mInteractionMsg = "";
	bool isTriggerMain = false;
	
	bool mIsShowUI = true;
	bool mIsLoad = false;
	bool mIsAutoCreateDialog = true;
	bool mIsDestroyArea = false;
	
	static Material lineMaterial;
	
	void Start()
	{
		if(isUIText) IsOverheadUIList.Add (this);
		
		if(mAreaType == AreaType.NONE) return;
		else if(mAreaType == AreaType.SAVE_POINT) mInteractionMsg = "X: Save.";
		else if(mAreaType == AreaType.PIXIE) mInteractionMsg = "X: Talk.";
		else if(mAreaType == AreaType.BATTLE_AREA) mInteractionMsg = "X: Enter.";
		else if(mAreaType == AreaType.BATTLE_TOUGH_ENEMY) mInteractionMsg = "X: Check.";
		else if(mAreaType == AreaType.NEXT_FLOOR) mInteractionMsg = "X: Next Floor.";
	}
	
	void Update () 
	{
		if(mIsLoad)
		{
			if(mTreasureChest == Treasure.NONE && HaveHiddenTreasure ()) 
			{
				DestroyTreasureChest(pTreasure.name);
				DestroyTreasureChest(pHiddenTreasure.name);
			}
			if(isOpen) 
			{
				HandleUnlock ();
				if(mIsDestroyArea) DestroyArea();
			}
			else if(!isOpen) 
			{
				GenerateTileProperties();
				if(!isPreGenerateArea && transform.childCount != 0)
				{
					Transform area = transform.FindChild("Area");
					if(area != null) DestroyImmediate(area.gameObject);
				}
			}
			mIsLoad = false;
		}
		if(!Application.isPlaying)
		{
			mIsEditMode = true;
			SetDialog();
			GenerateTileProperties();
			
			if(!isPreGenerateArea) DestroyArea();
			else if(isPreGenerateArea) GenerateArea();
			
			if(mTreasureChest == Treasure.NONE) DestroyTreasureChest(pTreasure.name);
			else if(mTreasureChest == Treasure.SHOWN) CreateTreasureChest();
		}
	}
	
	public string InteractionMsg
	{
		get { return mInteractionMsg; }
	}
	
	public bool IsTriggerMain
	{
		set { isTriggerMain = value; }
	}
	
	public void SetUIVisible(bool active)
	{
		mIsShowUI = active;
	}
	
	public void GetEncounterRate()
	{
		if(mEncounterType == EncounterType.NORMAL) return;
		
		if(mEncounterType == EncounterType.NONE) mPlayerController.NoEncRate ();
		else if(mEncounterType == EncounterType.MANDATORY) 
		{
			mEncounterType = EncounterType.NORMAL;
			mPlayerController.MandatoryEncRate();
		}
	}
	
	// Handle trigger when player clicked on an Area tile.
	public void HandleTrigger()
	{
		mCameraScript.RotationActive = false;
		
		DialogScript script = transform.GetComponent<DialogScript>();
		if(script == null || script.fullTalkList.Count == 0) isTriggerMain = true;
		
		if(isTriggerMain)
		{
			if(mAreaType == AreaType.SAVE_POINT) mSaveManager.SaveGame ();
			else if(mAreaType == AreaType.BATTLE_AREA || mAreaType == AreaType.BATTLE_TOUGH_ENEMY) 
			{
				if(!AutoFade.Fading) 
				{
					mCameraScript.ZoomInArea (transform.position);
					mPlayerController.playerMode = PlayerController.Mode.BATTLE;
				}
			}
			else if(mAreaType == AreaType.NEXT_FLOOR) 
			{ mPlayerController.playerMode = PlayerController.Mode.NEXT_FLOOR; }
			else if(mAreaType == AreaType.PIXIE) 
			{ mPlayerController.playerMode = PlayerController.Mode.SHOP; }
			
			isTriggerMain = false;
			return;
		}
		mPlayerController.playerMode = PlayerController.Mode.DIALOG;
		isTriggerMain = true;
	}
	
	// Handle tile unlocking.
	public void HandleUnlock()
	{
		isOpen = true;
		if(mTileType == TileType.NORMAL) SetTileProperties(false, OpenTexture);
		else if(mTileType == TileType.STORY_LOCK) SetTileProperties(false, StoryOpenTexture);
		
		if(mTreasureChest == Treasure.HIDDEN) GetHiddenTreasure();
		if(!mIsDestroyArea) GenerateArea();
	}
	
	// Open Shown treasure chest.
	public void OpenTreasureChest()
	{
		if(mTreasureChest == Treasure.SHOWN) 
		{
			Transform treasure = transform.FindChild(pTreasure.name);
			if(treasure != null) 
			{
				Destroy (treasure.gameObject);
				mTreasureChest = Treasure.NONE;
				Debug.Log ("Get treasure chest!");
			}
		}
	}
	
	// Set texture back to default.
	public void SetDefaultTexture()
	{
		if(!mIsExtSearch)
		{
			if ((isOpen && renderer.material.mainTexture != OpenTexture) 
				|| (!isOpen && renderer.material.mainTexture != LockTexture))
			{
				if(isOpen) SetTileTexture(OpenTexture);
				else if(!isOpen) SetTileTexture(LockTexture);
			}
		}
		else if(mIsExtSearch) SetTileTexture(ExtSearchTexture);
	}
	
	// Enable hover texture.
	public void EnableHoverTexture()
	{
		if(isOpen && renderer.material.mainTexture != HoverOpenTexture
			|| !isOpen && renderer.material.mainTexture != HoverLockTexture)
		{
			if(isOpen) SetTileTexture(HoverOpenTexture);
			else if(!isOpen) SetTileTexture(HoverLockTexture);
		}
	}
	
	// Instantiate hidden treasure and change tile texture.
	public void EnableExtendedSearch()
	{
		//mIsExtSearch = true;
		if(mTreasureChest == Treasure.HIDDEN)
		{
			if(!HaveHiddenTreasure()) // Generate hidden treasure.
			{
				GameObject hiddenTreasure = (GameObject)Instantiate(pHiddenTreasure, transform.position, Quaternion.identity);
				hiddenTreasure.name = pHiddenTreasure.name;
				hiddenTreasure.transform.parent = transform;
			}
			else if(HaveHiddenTreasure())// Enable hidden treasure visibility.
			{ transform.FindChild(pHiddenTreasure.name).GetComponent<MeshRenderer>().enabled = true; }
		}
		
		if(!mIsExtSearch)
		{
			if(mHasExtSearch) mExtSearchRenderer.enabled = true;
			else if(!mHasExtSearch)
			{
				Vector3 pos = transform.position;
				pos.y = TH_Y_Offset;
				
				GameObject thTile = (GameObject)Instantiate(pTH, pos, Quaternion.identity);
				thTile.name = pTH.name;
				thTile.transform.parent = transform;
				mExtSearchRenderer = thTile.GetComponent<MeshRenderer>();
				
				Color color = thTile.renderer.material.color;
				color.a = 0.4f;
				thTile.renderer.material.color = color;
			}
			mIsExtSearch = true;
		}
	}
	
	// Disable the visibility of hidden treasure and revert back to default texture.
	public void DisableExtendedSearch()
	{
		if(mTreasureChest == Treasure.HIDDEN)
		{
			if(HaveHiddenTreasure()) // Disable hidden treasure visibility.
			{ transform.FindChild(pHiddenTreasure.name).GetComponent<MeshRenderer>().enabled = false; }
		}
		
		if(mIsExtSearch)
		{
			mExtSearchRenderer.enabled = false;
			mHasExtSearch = true;
			mIsExtSearch = false;
		}
	}
	
	public void ClearedRedTile()
	{
		mAreaType = HexScript.AreaType.NONE;
		DestroyArea();
		mIsDestroyArea = true;
	}
	
	// Destroy Area.
	void DestroyArea()
	{
		if(transform.childCount == 0) return;
		
		Transform area = transform.FindChild("Area");
		if(area != null) DestroyImmediate(area.gameObject);
	}
	
	// Add dialog script if required.
	void SetDialog()
	{
		if(mAreaType == AreaType.NONE) mIsAutoCreateDialog = true;
		else if((mAreaType == AreaType.BATTLE_TOUGH_ENEMY 
		|| mAreaType == AreaType.NEXT_FLOOR
		|| mAreaType == AreaType.PIXIE)
		&& gameObject.GetComponent<DialogScript>() == null)
		{
			if(mIsAutoCreateDialog) isDialog = true;
		}
		
		if(isDialog && gameObject.GetComponent<DialogScript>() == null) 
		{ 
			mIsAutoCreateDialog = false;
			gameObject.AddComponent<DialogScript>(); 
		}
		else if(!isDialog)
		{ DestroyImmediate(gameObject.GetComponent<DialogScript>()); }
	}
	
	// Generate Area.
	void GenerateArea()
	{
		if(mAreaType == AreaType.NONE) 
		{ DestroyArea(); return; }
		
		if(mAreaType != mPrevAreaType) DestroyArea();
		mPrevAreaType = mAreaType;
		
		Transform area = transform.FindChild("Area");
		if(area != null) return;
		
		GameObject empty = new GameObject();
		empty.name = "Area";
		empty.transform.position = transform.position;
		empty.transform.parent = transform;

		if(mAreaType == AreaType.SAVE_POINT)
		{
			GameObject savePoint = (GameObject)Instantiate(pSavePoint, transform.position, Quaternion.identity);
			savePoint.name = pSavePoint.name;
			savePoint.transform.parent = empty.transform;
		}
		else if(mAreaType == AreaType.PIXIE)
		{
			Vector3 pos = transform.position;
			pos.y += pPixie.renderer.bounds.size.y / 2;
			
			GameObject pixie = (GameObject)Instantiate(pPixie, pos, Quaternion.identity);
			pixie.name = pSavePoint.name;
			pixie.transform.parent = empty.transform;
		}
		else if(mAreaType == AreaType.BATTLE_AREA)
		{
			Vector3 pos = transform.position;
			pos.y += pBattleArea.renderer.bounds.size.y / 2;
			
			GameObject battleArea = (GameObject)Instantiate(pBattleArea, pos, Quaternion.identity);
			battleArea.name = pSavePoint.name;
			battleArea.transform.parent = empty.transform;
		}
		else if(mAreaType == AreaType.BATTLE_TOUGH_ENEMY) // Red overlay on tile.
		{
			Vector3 pos = transform.position;
			pos.y += RedOverlay_Y_Offset;
			
			GameObject redOverlay = (GameObject)Instantiate(pRedOverlay, pos, Quaternion.identity);
			redOverlay.name = pRedOverlay.name;
			redOverlay.transform.parent = empty.transform;
			
			/*Color color = redOverlay.renderer.sharedMaterial.color;
			color.a = 1.0f;
			redOverlay.renderer.sharedMaterial.color = color;*/
		}
		else if(mAreaType == AreaType.NEXT_FLOOR)
		{
			Vector3 pos = transform.position;
			pos.y += pNextFloor.renderer.bounds.size.y / 2;
			
			GameObject nextFloor = (GameObject)Instantiate(pNextFloor, pos, Quaternion.identity);
			nextFloor.name = pSavePoint.name;
			nextFloor.transform.parent = empty.transform;
		}
	}
	
	// Pre-generate tile properties.
	void GenerateTileProperties()
	{
		if ((isOpen && mTileType == TileType.NORMAL && renderer.sharedMaterial.mainTexture != OpenTexture) 
			|| (!isOpen && mTileType == TileType.NORMAL && renderer.sharedMaterial.mainTexture != LockTexture)
			|| (isOpen && mTileType == TileType.STORY_LOCK && renderer.sharedMaterial.mainTexture != StoryOpenTexture)
			|| (!isOpen && mTileType == TileType.STORY_LOCK && renderer.sharedMaterial.mainTexture != StoryLockTexture))
		{
			if(isOpen) 
			{
				if(mTileType == TileType.NORMAL) SetTileProperties(false, OpenTexture);
				else if (mTileType == TileType.STORY_LOCK) SetTileProperties(true, StoryOpenTexture);
			}
			else if(!isOpen) 
			{
				if(mTileType == TileType.NORMAL) SetTileProperties(true, LockTexture);
				else if (mTileType == TileType.STORY_LOCK) SetTileProperties(true, StoryLockTexture);
			}
			Resources.UnloadUnusedAssets ();
		}
	}
	
	// Set collider and texture.
	void SetTileProperties(bool isCollider, Texture2D texture)
	{
		Collider sphereCollider = null;
		Collider[] mColliderArray = GetComponents<Collider>();
		
		foreach (Collider col in mColliderArray) 
		{ if(col.GetType() == typeof(SphereCollider)) sphereCollider = col; }
		
		sphereCollider.enabled = isCollider;
		if(!mIsEditMode) renderer.material.mainTexture = texture;
		else if(mIsEditMode)
		{
			renderer.sharedMaterial = new Material(renderer.sharedMaterial);
			renderer.sharedMaterial.mainTexture = texture;
		}
	}
	
	// Set texture.
	void SetTileTexture(Texture2D texture)
	{
		renderer.material.mainTexture = texture;
	}
	
	// Set texture and alpha value.
	void SetTileTexture(Texture2D texture, float alpha)
	{
		renderer.material.mainTexture = texture;
		
		Color temp = renderer.material.color;
		temp.a = alpha;
		renderer.material.color = temp;
	}
	
	// Create visible treasure chest.
	void CreateTreasureChest()
	{
		Transform tTreasure = transform.FindChild(pTreasure.name);
		if(tTreasure != null) return;
		
		Vector3 pos = transform.position;
		pos.y += pTreasure.renderer.bounds.size.y / 2;
		
		GameObject treasure = (GameObject)Instantiate(pTreasure, pos, Quaternion.identity);
		treasure.name = pTreasure.name;
		treasure.transform.parent = transform;
	}
	
	// Check to see whether there is hidden treasure.
	bool HaveHiddenTreasure()
	{
		Transform hiddenTreasure = transform.FindChild(pHiddenTreasure.name);
		if(hiddenTreasure != null) return true;
		else return false;
	}
	
	// Get hidden treasure if tile has it.
	void GetHiddenTreasure()
	{
		if(HaveHiddenTreasure()) Destroy(transform.FindChild(pHiddenTreasure.name).gameObject);
		
		mTreasureChest = Treasure.NONE;
		Debug.Log ("Get tile fragment!");
	}
	
	/*void TriggerExtension(AreaType areaType)
	{
		if(isTriggerExt)
		{
			if(areaType == AreaType.BATTLE_AREA || areaType == AreaType.BATTLE_TOUGH_ENEMY) 
			{
				if(!AutoFade.Fading) 
				{
					mCameraScript.ZoomInArea (transform.position);
					mPlayerController.playerMode = PlayerController.Mode.BATTLE;
				}
			}
			
			//else if(areaType == AreaType.BATTLE_TOUGH_ENEMY) 
			//{ mPlayerController.playerMode = PlayerController.Mode.BATTLE; }
			else if(areaType == AreaType.NEXT_FLOOR) 
			{ mPlayerController.playerMode = PlayerController.Mode.NEXT_FLOOR; }
			else if(areaType == AreaType.PIXIE) 
			{ mPlayerController.playerMode = PlayerController.Mode.SHOP; }
			
			isTriggerExt = false;
			return;
		}
		mPlayerController.playerMode = PlayerController.Mode.DIALOG;
		isTriggerExt = true;
	}*/
	
	// Destroy treasure chest.
	void DestroyTreasureChest(string name)
	{
		if(transform.childCount == 0) return;
		
		Transform treasure = transform.FindChild(name);
		
		if(treasure != null)
		{
			if(mIsEditMode) DestroyImmediate(treasure.gameObject);
			else if(!mIsEditMode) Destroy(treasure.gameObject);
		}
	}
	
	void OnDestroy()
	{
		if(!Application.isPlaying)
		{
			HexGenerator parentTemp = transform.parent.GetComponent<HexGenerator>();
			TileInfo tileTemp = new TileInfo(TileID, NeighbourIDList, transform.position);
			parentTemp.mTileArray[TileID] = null;
			parentTemp.mDeletedTileList.Add(tileTemp);
			Resources.UnloadUnusedAssets ();
		}
	}
	
	bool IsObscuredByFOW()
	{
		Vector3 raycastPos = transform.position;
		raycastPos.y = 1.5f;
		
		Ray ray = new Ray(raycastPos, -transform.up);
		RaycastHit hit;
		float mRayCastDistance = 2.5f;
		
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
	
	void OnGUI()
	{
		if( (isUIText && !IsObscuredByFOW() ) && mIsShowUI)
		{
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;
			GUI.skin.box.fontSize = 12;
			
			Vector3 screenPosOfUI = mCamera.WorldToScreenPoint(transform.position);
			float x = (screenPosOfUI.x - widthAnchor) + x_Offset;
			float y = (Screen.height - screenPosOfUI.y) - y_Offset; // Screen.height minus because of inverted y axis.
			GUI.Box (new Rect(x, y, width, height), text);
			
			mUIScreenPos = screenPosOfUI;
			
			x = mUIScreenPos.x + x_Offset;
			y = mUIScreenPos.y + y_Offset;
			
			Vector3 currPos = transform.position;
			Vector3 worldPosOfUI = mCamera.ScreenToWorldPoint(new Vector3(x, y, mCamera.nearClipPlane * 1.5f));
	
			CreateLineMaterial();
	        lineMaterial.SetPass( 0 );
	        GL.Begin( GL.LINES );
	        GL.Color( GizmoColor );
	        GL.Vertex3(currPos.x, currPos.y, currPos.z);
	        GL.Vertex3(worldPosOfUI.x, worldPosOfUI.y, worldPosOfUI.z);
	        GL.End();
		}
	}
	
	/*void OnDrawGizmos()
	{
		if(isUIText && !IsObscuredByFOW())
		{
			float x = mUIScreenPos.x + x_Offset;
			float y = mUIScreenPos.y + y_Offset;
			
			Vector3 worldPosOfUI = mCamera.ScreenToWorldPoint(new Vector3(x, y, mCamera.nearClipPlane * 1.5f));
	        Gizmos.color = GizmoColor;
	        Gizmos.DrawSphere(worldPosOfUI, 0.008F);
			
			Vector3 currPos = transform.position;
			Vector3 dir = worldPosOfUI - currPos;
			Gizmos.DrawRay(currPos, dir);
		}
	}*/
	
	void CreateLineMaterial()
    {
        if( !lineMaterial ) {
            lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
                "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Off Fog { Mode Off } " +
                "    BindChannels {" +
                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                "} } }" );
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }
	
	/*bool GenerateSimpleGUI(string title, string choice1, 
		string choice2, string feedbck1, string feedbck2, ref bool isClicked)
	{
		GUIStyle myStyle = new GUIStyle(GUI.skin.box);
		myStyle.fontSize = 20;
		myStyle.alignment = TextAnchor.UpperLeft;
		GUI.Box(new Rect(0, 0.75f * Screen.height, Screen.width, 0.25f * Screen.height), title, myStyle);

		GUI.skin.box.fontSize = 12;
		if(GUI.Button(new Rect(0.4f * Screen.width, 0.15f * Screen.height, 0.1f * Screen.width, 0.075f * Screen.height), choice1))
		{
			Debug.Log (feedbck1);
			isClicked = true;
			return true;
		}
		else if(GUI.Button(new Rect(0.55f * Screen.width, 0.15f * Screen.height, 0.1f * Screen.width, 0.075f * Screen.height), choice2))
		{
			Debug.Log (feedbck2);
			isClicked = true;
		}
		return false;
	}*/
	
	public void SetAnchorPoint()
	{
		if(mUIAnchorPoint == UIAnchorPoint.LEFT) widthAnchor = width;
		else if(mUIAnchorPoint == UIAnchorPoint.CENTER) widthAnchor = width / 2;
		else if(mUIAnchorPoint == UIAnchorPoint.RIGHT) widthAnchor = 0.0f;
	}
	
	public void ResetUIField()
	{
		text = "";
		x_Offset = DEFAULT_X_OFFSET;
		y_Offset = DEFAULT_Y_OFFSET;
		width = DEFAULT_WIDTH;
		height = DEFAULT_HEIGHT;
		mUIAnchorPoint = UIAnchorPoint.RIGHT;
	}
	
	public void Save()
	{
		PlayerPrefsX.SetBool (SaveManager.UniqueName(transform.gameObject, "isOpen"), isOpen);
		PlayerPrefsX.SetBool (SaveManager.UniqueName(transform.gameObject, "mIsDestroyArea"), mIsDestroyArea);
		PlayerPrefs.SetInt (SaveManager.UniqueName(transform.gameObject, "mEncounterType"), (int)mEncounterType);
		PlayerPrefs.SetInt (SaveManager.UniqueName(transform.gameObject, "mTreasureChest"), (int)mTreasureChest);
	}
	
	public void Load()
	{
		mIsLoad = true;
		isOpen = PlayerPrefsX.GetBool (SaveManager.UniqueName(transform.gameObject, "isOpen"));
		mIsDestroyArea = PlayerPrefsX.GetBool (SaveManager.UniqueName(transform.gameObject, "mIsDestroyArea"));
		mEncounterType = (EncounterType) PlayerPrefs.GetInt (SaveManager.UniqueName(transform.gameObject, "mEncounterType"));
		mTreasureChest = (Treasure) PlayerPrefs.GetInt (SaveManager.UniqueName(transform.gameObject, "mTreasureChest"));
	}
}