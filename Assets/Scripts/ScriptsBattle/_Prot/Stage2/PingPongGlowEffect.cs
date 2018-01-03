using UnityEngine;
using System.Collections;

public class PingPongGlowEffect : MonoBehaviour 
{
	public Material mMaterial;
	public Color ToColor;
	public float IntervalDur;
	
	Color mDefaultColor, mCurrentColor;
	float mLerpTimer;
	
	void Start () 
	{
		mCurrentColor = transform.GetChild(0).renderer.sharedMaterial.color;
		mDefaultColor = mCurrentColor;
	}
	
	void Update () 
	{
		mLerpTimer += Time.deltaTime;
		float t = mLerpTimer / IntervalDur;
		mMaterial.color = Color.Lerp(mCurrentColor, ToColor, t);
		
		if(t >= 1.0f) 
		{
			mLerpTimer = 0.0f;
			Color tempColor = mCurrentColor;
			mCurrentColor = mMaterial.color;
			ToColor = tempColor;
		}
	}
	
	void OnDestroy()
	{
		mMaterial.color = mDefaultColor;
	}
}
