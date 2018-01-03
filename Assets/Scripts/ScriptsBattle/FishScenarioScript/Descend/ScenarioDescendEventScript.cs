using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenarioDescendEventScript : MonoBehaviour 
{
	ScenarioDescendSkyDimScript mSkyDimScript;
	ScenarioDescendGlowingLightScript mGlowingLightScript;
	public GameObject mPrimaryObjective, mPortal;
	public List<ScenarioDescendBarrierScript> mPlatformBarrierScripts;
	public float mTimer = 0.0f, mCriticalTime = 60.0f, mDimTime = 1.5f;
	
	public bool mStageOne = false, mStageTwo = false, mEndGame = false , mPortalInhibitorDestroyed = false;
	// Use this for initialization
	void Start () 
	{
		mSkyDimScript = GetComponent<ScenarioDescendSkyDimScript>();
		mGlowingLightScript = GetComponent<ScenarioDescendGlowingLightScript>();
		
		for(int i = 0; i < mPlatformBarrierScripts.Count; i++)
		{
			mPlatformBarrierScripts[i].enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mPrimaryObjective && !mStageOne)
		{
			mPortalInhibitorDestroyed = true;
			mSkyDimScript.enabled = true;
			mSkyDimScript.mLerpOverSecond = mDimTime;
			mStageOne = true;
			mPortal.SetActive(true);
			for(int i = 0; i < mPlatformBarrierScripts.Count; i++)
			{
				mPlatformBarrierScripts[i].enabled = true;
			}
		}
		
		if(mStageOne && !mStageTwo)
		{
			mTimer += Time.deltaTime;
		}
		
		if(!mStageTwo && mTimer > mDimTime)
		{
			mGlowingLightScript.enabled = true;
			mGlowingLightScript.mLerpOverSecond = mCriticalTime;
			mStageTwo = true;
		}
	}
	
	void OnGUI()
	{
		if(mEndGame)
		{
			GUI.Box(new Rect(Screen.width/2-50,Screen.height/2-20,100,40),"You Win!");
		}
	}
}
