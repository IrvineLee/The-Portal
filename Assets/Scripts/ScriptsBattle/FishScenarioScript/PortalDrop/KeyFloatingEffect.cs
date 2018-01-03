using UnityEngine;
using System.Collections;

public class KeyFloatingEffect : MonoBehaviour 
{
	public float mFloatHeight = 1.0f;
	public float mLerpOverSecond, mLerpCounter;
	bool mGoingUp = false;
	public bool mStartHeading;
	Vector3 mStartLoc, mFloatLoc;
	// Use this for initialization
	void Start () 
	{
		mLerpCounter = 0.0f;
		SetHeading(mStartHeading);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//transform.position = Vector3.Lerp(mStartLoc, mFloatLoc, mLerpCounter);
		transform.position = Vector3.Lerp(mStartLoc, mFloatLoc, Mathf.SmoothStep(0.0f, 1.0f, mLerpCounter));
		if(mLerpCounter < 1.0f)
		{
			mLerpCounter += Time.deltaTime / mLerpOverSecond;
		}
		else if(mLerpCounter >= 1.0f)
		{
			SetHeading(!mGoingUp);
			mLerpCounter = 0.0f;
		}
	}
	
	void SetHeading(bool headingUp)
	{//! if bool is true, dir is up, else dir is down
		mGoingUp = headingUp;
		mStartLoc = mFloatLoc = transform.position;
		if(headingUp)
			mFloatLoc.y += mFloatHeight;
		else
			mFloatLoc.y -= mFloatHeight;
	}
}