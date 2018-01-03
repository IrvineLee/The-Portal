using UnityEngine;
using System.Collections;

public class ObjectiveScript : MonoBehaviour 
{
	public enum Objectives
	{
		NONE = 0,
		DEFEAT_X_ENEMIES,
		SEARCH_AND_KILL_PL,
		DESTROY_GENERATORS
	};
	public Objectives mObjective = Objectives.NONE;
	
	public Transform Spawners;
	
	enum DisplayMsg
	{
		OBJ = 0,
		WIN
	};
	DisplayMsg mDisplayMsg = DisplayMsg.OBJ;
	
	GameObject mPLPrefab;
	float mWinMsgTime = 2.0f;
	float mWinMsgTimer = 0.0f;
	bool mIsActivateTrigger = false;
	bool mIsShowMsg = false;
	bool mIsTriggered = false;
	
	EnemyCheckerScript mEnemyCheckerScript;
	KillCount mKillCount;
	
	void Start()
	{
		GameObject cam = GameObject.FindGameObjectWithTag ("MainCamera");
		mEnemyCheckerScript = cam.GetComponent<EnemyCheckerScript>();
		mKillCount = transform.GetComponent<KillCount>();
		
		mPLPrefab = Resources.Load ("SquadLeader") as GameObject; 
	}
	
	void Update () 
	{
		if(!mIsActivateTrigger) return;
		
		if(mDisplayMsg == DisplayMsg.WIN)
		{
			mWinMsgTimer -= Time.deltaTime;
			if(mWinMsgTimer < 0.0f) 
			{
				mDisplayMsg = DisplayMsg.OBJ;
				mIsShowMsg = false;
				mIsActivateTrigger = false;
				gameObject.SetActive (false);
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(!mIsActivateTrigger || mIsTriggered || other.gameObject.tag != "Player") return;
		
		ActivateBarrier(true);
		mIsTriggered = true;
		mIsShowMsg = true;
		
		if(mObjective == Objectives.DEFEAT_X_ENEMIES)
		{
			SpawnFor("KillXEnemyObjective");
			mKillCount.SetActive (true, 6);
		}
		else if(mObjective == Objectives.SEARCH_AND_KILL_PL)
		{
			SpawnFor("SearchAndKillPLObjective");
			
			int randomIndex = Random.Range (0, Spawners.childCount);
			GameObject PL = (GameObject) GameObject.Instantiate (mPLPrefab, Spawners.GetChild (randomIndex).position, Quaternion.identity);
			SquadLeaderScript plScript = PL.GetComponent<SquadLeaderScript>();
			plScript.IsObjective = true;
			plScript.IsLockFlee = true;
			plScript.IsStayHidden = true;
			
			mKillCount.SetActive (true, 1);
		}
		else if(mObjective == Objectives.DESTROY_GENERATORS)
		{
			SpawnFor("EnemyGeneratorObjective");
			mEnemyCheckerScript.enabled = true;
		}
	}
	
	public bool IsActivateTrigger
	{
		set { mIsActivateTrigger = value; }
	}
	
	public void IsComplete()
	{
		mDisplayMsg = DisplayMsg.WIN;
		mWinMsgTimer = mWinMsgTime;
		ActivateBarrier(false);
		//mKillCount.SetActive (false);
		Debug.Log (mDisplayMsg);
	}
	
	void ActivateBarrier(bool enabled)
	{
		int count = transform.childCount;
		for(int i = 0; i < count; i++)
		{
			transform.GetChild (i).GetComponent<Collider>().enabled = enabled;
		}
	}
	
	void SpawnFor(string objectiveGOName)
	{
		foreach (Transform child in transform.parent)
	    {
	    	if(child.name == "KillXEnemyObjective" 
				|| child.name == "SearchAndKillPLObjective"
				|| child.name == "EnemyGeneratorObjective") 
			{
				child.gameObject.SetActive (true);
				break;
			}
	    }
	}
	
	void OnGUI()
	{
		if(!mIsShowMsg) return;
		
		if(mDisplayMsg == DisplayMsg.OBJ)
		{
			string str = "";
			GUI.skin.label.fontSize = 12;
			
			if(mObjective == Objectives.DEFEAT_X_ENEMIES) str = "Defeat " + KillCount.KillNeeded + " enemies";
			else if(mObjective == Objectives.SEARCH_AND_KILL_PL) str = "Search and kill the 'Platoon Leader'!";
			else if(mObjective == Objectives.DESTROY_GENERATORS) str = "Search and destroy the generators!";
			
			Rect rect = new Rect(32.0f, 64.0f, Screen.width * 0.3f, Screen.height * 0.05f);
			GUILayout.BeginArea(rect);
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			GUILayout.Label(str, GUILayout.Width(Screen.width * 0.3f));
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		else if(mDisplayMsg == DisplayMsg.WIN)
		{
			string str = "Objective Cleared!!";
			float width = 0.5f;
			float height = 0.15f;
			
			GUI.skin.box.fontSize = 40;
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;
			GUI.Box (new Rect(Screen.width * (width - (width / 2.0f)), Screen.height * ((1 - height) / 2), Screen.width * width, Screen.height * height), str);
			GUI.skin.box.fontSize = 12;
		}
	}
}
