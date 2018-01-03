using UnityEngine;
using System.Collections;

public class PickUpGlowScript : MonoBehaviour 
{
	PickUpDistanceCheckerScript mDistanceCheckerScript;
	public bool mPlayerLeftProximity = false, mLerpDone = false, mLerpOut = false, mActive = false;
	public Light mLightComponent;
	public float mCurrIntensity, mLerpIntensity, mIntensityDiff;
	public float mLerpCounter, mLerpOverSecond;
	// Use this for initialization
	void Start () 
	{
		mDistanceCheckerScript = GetComponent<PickUpDistanceCheckerScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mLerpCounter < 1.0f)
		{
			mLightComponent.intensity = Mathf.Lerp(mCurrIntensity, mLerpIntensity, Mathf.SmoothStep(0.0f, 1.0f, mLerpCounter));
			mLerpCounter += Time.deltaTime / mLerpOverSecond;
		}
		
		else if(mLerpCounter >= 1.0f && mLerpOut)
		{
			gameObject.SetActive(false);
		}
		
		
		if(mPlayerLeftProximity && !mLerpOut)
		{
			ChangeLerpDir();
			mLerpCounter = 0.0f;
			mLerpOut = true;
		}
	}
	
	void ChangeLerpDir()
	{
		mCurrIntensity = mLightComponent.intensity;
		mLerpIntensity = mCurrIntensity - mIntensityDiff;
		mLerpIntensity = Mathf.Clamp(mLerpIntensity, 0.0f,1.0f);
	}
	
	public void InitializeScript()
	{
		mPlayerLeftProximity =  mLerpDone = mLerpOut = false;
		mLerpCounter = 0.0f;
		mCurrIntensity = mLightComponent.intensity;
		mLerpIntensity = mCurrIntensity + mIntensityDiff;
	}
}
