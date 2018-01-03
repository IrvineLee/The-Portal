using UnityEngine;
using System.Collections;

public class ScenarioDescentEventScript : MonoBehaviour 
{
	ScenarioDescentSkyDimScript mSkyDimScript;
	ScenarioGlowingLightScript mGlowingLightScript;
	public GameObject mPrimaryObjective;
	
	public float mTimer = 0.0f, mCriticalTime = 60.0f, mDimTime = 1.5f;
	
	public bool mStageOne = false, mStageTwo = false;
	// Use this for initialization
	void Start () 
	{
		mSkyDimScript = GetComponent<ScenarioDescentSkyDimScript>();
		mGlowingLightScript = GetComponent<ScenarioGlowingLightScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mPrimaryObjective && !mStageOne)
		{
			mSkyDimScript.enabled = true;
			mSkyDimScript.mLerpOverSecond = mDimTime;
			mStageOne = true;
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
}
