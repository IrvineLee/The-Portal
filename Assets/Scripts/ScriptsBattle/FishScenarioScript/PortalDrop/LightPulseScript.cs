using UnityEngine;
using System.Collections;

public class LightPulseScript : MonoBehaviour 
{
	public Light mLight;
	public float mDefaultIntensity, mLerpIntensity;
	public float mLerpCounter, mLerpOverSecond;
	public bool mGlow = true;
	// Use this for initialization
	void Start () 
	{
		mDefaultIntensity = mLight.intensity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mGlow)
			mLight.intensity = Mathf.Lerp(mDefaultIntensity, mLerpIntensity, Mathf.SmoothStep(0.0f, 1.0f, mLerpCounter));
		else
			mLight.intensity = Mathf.Lerp(mLerpIntensity, mDefaultIntensity, Mathf.SmoothStep(0.0f, 1.0f, mLerpCounter));
		
		if(mLerpCounter < 1.0f)
		{
			mLerpCounter += (Time.deltaTime / mLerpOverSecond);
		}
		else if(mLerpCounter >= 1.0f)
		{
			ResetGlow();
		}
	}
	
	public void ResetGlow()
	{
		mLerpCounter = 0.0f;
		mGlow = !mGlow;
	}
}
