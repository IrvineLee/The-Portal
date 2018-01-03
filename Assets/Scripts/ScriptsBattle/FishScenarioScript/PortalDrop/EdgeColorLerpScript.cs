using UnityEngine;
using System.Collections;

public class EdgeColorLerpScript : MonoBehaviour 
{
	public Material mMaterial, mParentCopy;
	public Color mColor;
	Color mDefaultColor;
	public float mLerpCounter, mLerpOverSecond , mColorAlpha = 145.0f;
	public bool mGlow = true;
	// Use this for initialization
	void Start () 
	{
		mMaterial.color = mParentCopy.color;
		mMaterial.color.a.Equals(mColorAlpha);
		mDefaultColor = mMaterial.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mGlow)
			mMaterial.color = Color.Lerp(mDefaultColor, mColor, Mathf.SmoothStep(0.0f, 1.0f, mLerpCounter));
		else
			mMaterial.color = Color.Lerp(mColor, mDefaultColor, Mathf.SmoothStep(0.0f, 1.0f, mLerpCounter));
		
		if(mLerpCounter < 1.0f)
		{
			mLerpCounter += (Time.deltaTime / mLerpOverSecond);
		}
		else if(mLerpCounter >= 1.0f && mGlow)
		{
			ResetGlow();
		}
	}
	
	public void ResetGlow()
	{
		mLerpCounter = 0.0f;
		mGlow = !mGlow;
	}
	
	public void InitializeTimer(float time)
	{
		mLerpOverSecond = time * 0.5f;
	}
}
