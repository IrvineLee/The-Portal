using UnityEngine;
using System.Collections;

public class InGameMenuScript : MonoBehaviour 
{
	enum Mode
	{
		NONE = 0,
		MAIN_MENU,
		ITEM,
		EQUIP,
		SKILL,
		STATUS,
		OPTION
	};
	Mode mMode = Mode.NONE;
	
	int mAccIndex = -1, mCurrPage = 1, mMaxPage = 2, mMaxSelectionPerPage = 5;
	string mPageArrow;
	bool mIsShowAccInventory = false;
	bool mIsShowSolInventory = false;
	
	int mCurrWeaponIndex;
	
	bool isPause = false;
	
	PlayerController mPlayerController;
	PlayerStatus mPlayerStatus;
	FOWScript mFOWScript;
	FOWRevealer mFOWRevealer;
	Inventory mInventory;
	SaveManager mSaveManager;

	void Start () 
	{
		mPlayerController = transform.GetComponent<PlayerController>();
		mPlayerStatus = transform.GetComponent<PlayerStatus>();
		mFOWRevealer = transform.GetComponent<FOWRevealer>();
		mFOWScript = GameObject.FindGameObjectWithTag("FOW").GetComponent<FOWScript>();
		mSaveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
		mInventory = transform.GetComponent<Inventory>();
		
		ItemList.RegisterItems();
		Weapons.RegisterWeapons ();
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			if(mMode == Mode.NONE) { if(!isPause) Pause(true); }
			else if(mMode == Mode.MAIN_MENU) { if(isPause) Pause(false); }
			else if(mMode == Mode.EQUIP) 
			{ 
				if(mIsShowAccInventory) mIsShowAccInventory = false;
				else mMode = Mode.MAIN_MENU;
			}
		}
		
		if(Input.GetMouseButtonDown(1))
		{
			if(mMode == Mode.EQUIP)
			{
				if(mIsShowAccInventory) mIsShowAccInventory = false;
				if(mIsShowSolInventory) mIsShowSolInventory = false;
				//else mMode = Mode.MAIN_MENU;
			}
		}
		
		// Handle Loading...
		if(isPause && AutoFade.IsFadeOutSolid)
		{
			mFOWScript.DefaultFOW ();
			mSaveManager.LoadGame ();
			mFOWRevealer.BeginInitialFOW();
			Pause(false);
		}
	}
	
	public int GetAccIndex
	{
		get { return mAccIndex; }
	}
	
	void Pause(bool active)
	{
		if(active)
		{
			///*Delete*/mInventory = transform.GetComponent<Inventory>();
			mPlayerController.isUpdate = false;
			mFOWRevealer.enabled = false;
			isPause = true;
			
			mMode = Mode.MAIN_MENU;
			GetPagingInfo(); // For equipment screen.
		}
		else if(!active)
		{
			mPlayerController.isUpdate = true;
			mFOWRevealer.enabled = true;
			isPause = false;
			
			mMode = Mode.NONE;
		}
	}
	
	void OnGUI()
	{
		if(isPause)
		{
			if(mMode == Mode.MAIN_MENU)
			{
				GUI.skin.button.alignment = TextAnchor.MiddleCenter;
				if(GUI.Button(new Rect(0.15f * Screen.width, 0.1f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "ITEM"))
				{
					mMode = Mode.ITEM;
				}
				else if(GUI.Button(new Rect(0.15f * Screen.width, 0.2f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "EQUIP"))
				{
					mMode = Mode.EQUIP;
				}
				else if(GUI.Button(new Rect(0.15f * Screen.width, 0.3f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "SKILL"))
				{
					mMode = Mode.SKILL;
				}
				else if(GUI.Button(new Rect(0.15f * Screen.width, 0.4f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "STATUS"))
				{
					mMode = Mode.STATUS;
				}
				else if(GUI.Button(new Rect(0.15f * Screen.width, 0.5f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "OPTION"))
				{
					mMode = Mode.OPTION;
				}
				else if(GUI.Button(new Rect(0.15f * Screen.width, 0.6f * Screen.height, 0.15f * Screen.width, 0.075f * Screen.height), "LOAD"))
				{
					AutoFade.LoadLevel ("", 0.5f, 0.5f, Color.black);
				}
			}
			else if(mMode == Mode.EQUIP)
			{
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				
				//------------------//
				//	      TOP       //
				//------------------//
				
				GUI.skin.label.fontSize = 35;
				GUI.Label(new Rect(0.05f * Screen.width, 0.01f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "EQUIP");
				GUI.Box(new Rect(0.0f * Screen.width, 0.09f * Screen.height, 1.0f * Screen.width, 0.025f * Screen.height), "");
				
				// Character Portrait //
				GUI.Box(new Rect(0.24f * Screen.width, 0.16f * Screen.height, 0.15f * Screen.width, 0.2f * Screen.height), "");
				
				GUI.skin.label.fontSize = 20;
				// LVL //
				GUI.Label(new Rect(0.35f * Screen.width, 0.31f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "Lvl");
				GUI.Label(new Rect(0.4f * Screen.width, 0.31f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.Level.ToString());
				
				// HP // 
				GUI.Label(new Rect(0.5f * Screen.width, 0.15f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "HP");
				GUI.Label(new Rect(0.55f * Screen.width, 0.15f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.CurrHp.ToString() + " / " + mPlayerStatus.MaxHp.ToString());
				
				// ATK / DEF // 
				// Label
				GUI.Label(new Rect(0.525f * Screen.width, 0.23f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "ATK");
				GUI.Label(new Rect(0.525f * Screen.width, 0.28f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "DEF");
				// Data
				GUI.Label(new Rect(0.59f * Screen.width, 0.23f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.mAtk.ToString());
				GUI.Label(new Rect(0.59f * Screen.width, 0.28f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.mDef.ToString());
				
				GUI.skin.label.fontSize = 15;
				// STATS // 
				// Label
				GUI.Label(new Rect(0.73f * Screen.width, 0.18f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "Str");
				GUI.Label(new Rect(0.73f * Screen.width, 0.22f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "End");
				GUI.Label(new Rect(0.73f * Screen.width, 0.26f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "Agi");
				GUI.Label(new Rect(0.73f * Screen.width, 0.3f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "Lck ");
				// Data
				GUI.Label(new Rect(0.8f * Screen.width, 0.18f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.Str.ToString());
				GUI.Label(new Rect(0.8f * Screen.width, 0.22f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.End.ToString());
				GUI.Label(new Rect(0.8f * Screen.width, 0.26f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.Agi.ToString());
				GUI.Label(new Rect(0.8f * Screen.width, 0.3f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), mPlayerStatus.Lck.ToString());
				
				GUI.Box(new Rect(0.1f * Screen.width, 0.385f * Screen.height, 0.8f * Screen.width, 0.01f * Screen.height), "");
				
				//------------------//
				//	   LEFT SIDE    //
				//------------------//
				
				GUI.skin.label.fontSize = 17;
				GUI.skin.button.alignment = TextAnchor.MiddleLeft;
				// WEAPON SELECTION //
				GUI.Label(new Rect(0.14f * Screen.width, 0.4f * Screen.height, 0.18f * Screen.width, 0.075f * Screen.height), "Weapon");
				// Label
				//GUI.Box(new Rect(0.16f * Screen.width, 0.45f * Screen.height, 0.16f * Screen.width, 0.075f * Screen.height), "Primary");
				//GUI.Box(new Rect(0.16f * Screen.width, 0.5f * Screen.height, 0.16f * Screen.width, 0.075f * Screen.height), "Secondary");
				// Data
				if(GUI.Button(new Rect(0.15f * Screen.width, 0.462f * Screen.height, 0.25f * Screen.width, 0.09f * Screen.height), Weapons.WeaponList[0].name)) mCurrWeaponIndex = 0;
				if(GUI.Button(new Rect(0.15f * Screen.width, 0.552f * Screen.height, 0.25f * Screen.width, 0.09f * Screen.height), Weapons.WeaponList[1].name)) mCurrWeaponIndex = 1;
				
				//if(GUI.Button(new Rect(0.48f * Screen.width, 0.485f * Screen.height, 0.06f * Screen.width, 0.05f * Screen.height), "Swap")) Debug.Log ("Switch");
				
				// ACCESSORIES //
				GUI.Label(new Rect(0.14f * Screen.width, 0.63f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Accessories");
				DisplayAccNumber();
				if(!mIsShowAccInventory) DisplayAccessoriesSlot();
				else if(mIsShowAccInventory) OpenSelectionItem();
				
				//------------------//
				//	  RIGHT SIDE    //
				//------------------//
				
				// WEAPON PARAMETER //
				GUI.Label(new Rect(0.59f * Screen.width, 0.4f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Parameter");
				//if(GUI.Button(new Rect(0.59f * Screen.width, 0.472f * Screen.height, 0.1f * Screen.width, 0.05f * Screen.height), "Overview")) {}
				//if(GUI.Button(new Rect(0.69f * Screen.width, 0.472f * Screen.height, 0.1f * Screen.width, 0.05f * Screen.height), "SolCryst")) {}
				
				// Label
				GUI.Label(new Rect(0.61f * Screen.width, 0.45f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Atk Power");
				GUI.Label(new Rect(0.61f * Screen.width, 0.5f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Crit. Rate");
				GUI.Label(new Rect(0.61f * Screen.width, 0.55f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Effect");
				// Data
				GUI.Label(new Rect(0.76f * Screen.width, 0.45f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), Weapons.WeaponList[mCurrWeaponIndex].atk.ToString());
				GUI.Label(new Rect(0.76f * Screen.width, 0.5f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), Weapons.WeaponList[mCurrWeaponIndex].criticalRate.ToString() + " %");
				
				// SOL CRYST //
				GUI.Label(new Rect(0.59f * Screen.width, 0.63f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Sol Cryst");
				// Label
				GUI.Label(new Rect(0.61f * Screen.width, 0.68f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "1.");
				GUI.Label(new Rect(0.61f * Screen.width, 0.73f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "2.");
				
				DisplaySolCrystSlot();
				if(mIsShowSolInventory) OpenSelectionItem2();
				// Data
				//GUI.Label(new Rect(0.65f * Screen.width, 0.73f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Piercing");
				//GUI.Label(new Rect(0.65f * Screen.width, 0.68f * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), "Str +3");
				
				GUI.Box(new Rect(0.0f * Screen.width, 0.95f * Screen.height, 1.0f * Screen.width, 0.01f * Screen.height), "");
			}
		}
	}
	
	void DisplayAccNumber()
	{
		for(int i = 0; i < mPlayerStatus.MaxAccSlot; i++)
		{
			GUI.Label(new Rect(0.16f * Screen.width, (0.68f + (i * 0.05f)) * Screen.height, 0.25f * Screen.width, 0.075f * Screen.height), (i+1) + ".");
		}
	}
	
	void DisplaySolCrystSlot()
	{
		GUI.skin.button.fontSize = 15;
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		
		Weapons.WeaponInfo currWeapon = Weapons.WeaponList[mCurrWeaponIndex];
		int slotCount = Weapons.WeaponList[mCurrWeaponIndex].maxSolCrystSlot;
		for(int i = 0; i < slotCount; i++)
		{
			if(GUI.Button(new Rect(0.65f * Screen.width, (0.692f + (i * 0.05f)) * Screen.height, 0.25f * Screen.width, 0.05f * Screen.height), currWeapon.equippedSolCrystList[i]))
			{
				if(Input.GetMouseButtonUp(0)) 
				{
					mAccIndex = i;
					mIsShowSolInventory = true;
				}
				else if(Input.GetMouseButtonUp(1)) 
				{
					//mInventory.UnEquippedItem (i);
				}
			}
		}
	}
	
	void DisplayAccessoriesSlot()
	{
		GUI.skin.button.fontSize = 15;
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		
		int currEquippedCount = mInventory.AccEquippedList.Count;
		string accName;
		
		for(int i = 0; i < mPlayerStatus.MaxAccSlot; i++)
		{
			if(i >= currEquippedCount) accName = "";
			else accName = mInventory.AccEquippedList[i];
			
			if(GUI.Button(new Rect(0.2f * Screen.width, (0.692f + (i * 0.05f)) * Screen.height, 0.25f * Screen.width, 0.05f * Screen.height), accName)) 
			{
				if(Input.GetMouseButtonUp(0)) 
				{
					mAccIndex = i;
					mIsShowAccInventory = true;
				}
				else if(Input.GetMouseButtonUp(1)) 
				{
					mInventory.UnEquippedItem (ItemList.Type.ACCESSORIES, i);
				}
			}
		}
	}
	
	void OpenSelectionItem()
	{
		string itemName;
		
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		for(int i = 0; i < mMaxSelectionPerPage; i++)
		{
			if(i <= mInventory.AccSelectionList.Count - 1) itemName = mInventory.AccSelectionList[i];
			else itemName = "";
			
			if(GUI.Button(new Rect(0.2f * Screen.width, (0.692f + (i * 0.05f)) * Screen.height, 0.25f * Screen.width, 0.05f * Screen.height), itemName))
			{
				if(itemName == "") continue;
				mInventory.ChangeAcc(mAccIndex, itemName);
				
				mIsShowAccInventory = false;
			}
		}
		
		// Handle next/previous page.
		GUI.skin.button.alignment = TextAnchor.MiddleRight;
		if(GUI.Button(new Rect(0.45f * Screen.width, (0.692f + ((mMaxSelectionPerPage - 1) * 0.05f)) * Screen.height, 0.08f * Screen.width, 0.05f * Screen.height), "Pg " + mCurrPage + "/" + mMaxPage + mPageArrow))
		{
			if(mMaxPage == 1) return;
			
			if(mCurrPage < mMaxPage) mCurrPage += 1;
			else if(mCurrPage == mMaxPage) mCurrPage -= 1;
			
			if(mCurrPage < mMaxPage) mPageArrow = " >";
			else if(mCurrPage == mMaxPage) mPageArrow = " <";
		}
	}
	
	void OpenSelectionItem2()
	{
		string itemName;
		
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		for(int i = 0; i < mMaxSelectionPerPage; i++)
		{
			if(i <= mInventory.SolSelectionList.Count - 1) itemName = mInventory.SolSelectionList[i];
			else itemName = "";
			
			if(GUI.Button(new Rect(0.65f * Screen.width, (0.692f + (i * 0.05f)) * Screen.height, 0.25f * Screen.width, 0.05f * Screen.height), itemName))
			{
				if(itemName == "") continue;
				//mInventory.ChangeAcc(mAccIndex, itemName);
				
				mIsShowSolInventory = false;
			}
		}
		
		// Handle next/previous page.
		GUI.skin.button.alignment = TextAnchor.MiddleRight;
		if(GUI.Button(new Rect(0.9f * Screen.width, (0.692f + ((mMaxSelectionPerPage - 1) * 0.05f)) * Screen.height, 0.08f * Screen.width, 0.05f * Screen.height), "Pg " + mCurrPage + "/" + mMaxPage + mPageArrow))
		{
			if(mMaxPage == 1) return;
			
			if(mCurrPage < mMaxPage) mCurrPage += 1;
			else if(mCurrPage == mMaxPage) mCurrPage -= 1;
			
			if(mCurrPage < mMaxPage) mPageArrow = " >";
			else if(mCurrPage == mMaxPage) mPageArrow = " <";
		}
	}
	
	void GetPagingInfo()
	{
		mMaxPage = Mathf.CeilToInt((float)mInventory.GetAccSelectionCount / 5);
		if(mMaxPage == 1) mPageArrow = "";
		else mPageArrow = " >";
	}
}
