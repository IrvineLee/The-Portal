using UnityEngine;
using System.Collections;

public class AlphaEnabler : MonoBehaviour 
{
	public Material mMaterial;
	public float Duration = 1.0f;
	public bool OnTrigger = false;
	
	Color mDefaultColor;
	int mColorCount;
	float mTimer;
	bool mIsTriggered = false;
	
	void Start()
	{
		Color tempColor = mMaterial.color;
		tempColor.a = 0.0f;
		mDefaultColor = tempColor;
	}
	
	void Update () 
	{
		if(!mIsTriggered) return;
		
		mTimer += Time.deltaTime * 2.0f;
		float t = mTimer / Duration;
		if(mColorCount == 1) t = 1.0f - t;
		
		Color tempColor = mMaterial.color;
		tempColor.a = t;
		mMaterial.color = tempColor;
		
		if(t >= 1.0f) 
		{
			mColorCount += 1;
			mTimer = 0.0f;
			if(mColorCount == 2) enabled = false;
		}
	}
	
	void OnTriggerEnter(Collider other) 
	{
		if(!OnTrigger || other.tag != "Player") return;
		
		mIsTriggered = true;
	}
	
	void OnDestroy()
	{
		mMaterial.color = mDefaultColor;
	}
}
