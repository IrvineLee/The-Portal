using UnityEngine;
using System.Collections;

public class CursorAndRestartScript : MonoBehaviour 
{
	PlayerScript mPlayerScript;
	HealthUI mHealthUI;
	
	GameObject[] leaderArray = new GameObject[10];
	GameObject[] gruntArray = new GameObject[20];
	public bool isLocked = false;
	void Start()
	{
		mPlayerScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerScript>();
		mHealthUI = GameObject.FindGameObjectWithTag ("Player").GetComponent<HealthUI>();
		Screen.lockCursor = isLocked;	
	}
	
	
   
	void Update() 
	{
        if (Input.GetKeyDown(KeyCode.F1))
            isLocked = !isLocked;
		if(!isLocked && Screen.lockCursor)
			Screen.lockCursor = false;
		if(isLocked && !Screen.lockCursor)
			Screen.lockCursor = true;
		
		if(Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel(Application.loadedLevel);
		
		if(Input.GetKeyDown(KeyCode.F2)) SwapHPType();
		if(Input.GetKeyDown(KeyCode.F5)) Application.LoadLevel("Fish_Nuggets");
		if(Input.GetKeyDown(KeyCode.F6)) Application.LoadLevel("Nuggets");
		if(Input.GetKeyDown(KeyCode.F7)) Application.LoadLevel("Fish_Scenario");
		if(Input.GetKeyDown(KeyCode.F8)) Application.LoadLevel("Scenario1");
    }
	
	void SwapHPType()
	{
		if(mHealthUI.DisplayType == HealthUI.Type.BAR)
		{
			mHealthUI.DisplayType = HealthUI.Type.LIFE;
			mHealthUI.mLifeNo = 5;
			mPlayerScript.mMaxHealth = 5;
			mPlayerScript.mCurrHealth = 5;
			
			leaderArray = GameObject.FindGameObjectsWithTag ("Enemy2");
			foreach(GameObject leader in leaderArray)
			{
				leader.GetComponent<SquadLeaderScript>().Is1HitDmg = true;
			}
			
			gruntArray = GameObject.FindGameObjectsWithTag ("Enemy");
			foreach(GameObject grunt in gruntArray)
			{
				grunt.GetComponent<EnemyScript>().Dmg = 1;
			}
		}
		else if(mHealthUI.DisplayType == HealthUI.Type.LIFE)
		{
			mHealthUI.DisplayType = HealthUI.Type.BAR;
			mHealthUI.Hp_Ratio = 1.0f;
			mPlayerScript.mMaxHealth = 100;
			mPlayerScript.mCurrHealth = 100;
			
			leaderArray = GameObject.FindGameObjectsWithTag ("Enemy2");
			foreach(GameObject leader in leaderArray)
			{
				leader.GetComponent<SquadLeaderScript>().Is1HitDmg = false;
			}
			
			gruntArray = GameObject.FindGameObjectsWithTag ("Enemy");
			foreach(GameObject grunt in gruntArray)
			{
				grunt.GetComponent<EnemyScript>().Dmg = 10;
			}
		}
	}
}