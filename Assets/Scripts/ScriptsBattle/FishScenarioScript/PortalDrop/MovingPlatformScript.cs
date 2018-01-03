using UnityEngine;
using System.Collections;

public class MovingPlatformScript : MonoBehaviour 
{
	public GameObject mManager;
	public AreaSpawnerScript mSpawnerScript;
	public SceneStage2Script mStageEffectScript;
	public Vector3 mHeading, mOrigin, mDestination;
	bool mOscillate = false, mActive = false;
	public bool mInitialSetup = true;
	public float mDistance = 5.0f, mLerpCounter, mLerpOverSecond;
	// Use this for initialization
	void Start () 
	{
		mHeading = transform.TransformDirection(mHeading.normalized);
		if(mInitialSetup)ResetDir();
		if(!mInitialSetup)
		{
			mStageEffectScript = mManager.GetComponent<SceneStage2Script>();
			mActive = mStageEffectScript.mPlatformActivate;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mInitialSetup)
		{
			if(mActive)
			{
				UpdatePlatform();
			}
			else
			{
				if(mManager.activeSelf)
				{
					mActive = true;
					ResetDir();
				}
			}
		}
		else
		{
			UpdatePlatform();
		}
	}
	
	public void ResetDir()
	{
		mOrigin = transform.position;
		if(!mOscillate)
			mDestination = mOrigin + (mHeading * mDistance);
		else
			mDestination = mOrigin + (-mHeading * mDistance);
		mLerpCounter = 0.0f;
		mOscillate = !mOscillate;
	}
	
	void UpdatePlatform()
	{
		transform.localPosition = Vector3.Lerp(mOrigin, mDestination, Mathf.SmoothStep(0.0f, 1.0f, mLerpCounter));
				
		if(mLerpCounter < 1.0f)
		{
			mLerpCounter += (Time.deltaTime / mLerpOverSecond);
		}
		else if(mLerpCounter >= 1.0f)
		{
			ResetDir();
		}
	}
}
