using UnityEngine;
using System.Collections;

public class TimerDrop : MonoBehaviour 
{
	public float FallDuration;
	public float WarningTime;
	public float WarningDuration = 1.0f;
	public float DropVal = 1.0f;
	
	float mFallTimer;
	float mTotalDrop;
	bool mIsWarned = false;
	
//	void Start()
//	{
//		Activate(false);
//	}
	
	void Update () 
	{
		mFallTimer += Time.deltaTime;
		if(!mIsWarned && mFallTimer >= WarningTime)
		{
			Vector3 pos = transform.position;
			float dropVal = DropVal * Time.deltaTime * 2.5f;
			pos.y -= DropVal * Time.deltaTime * 2.5f;
			transform.position = pos;
			
			mTotalDrop += dropVal;
			if(mTotalDrop >= DropVal) 
			{
				mIsWarned = true;
			}
		}
		else if(mFallTimer >= FallDuration)
		{
			Activate(true);
		}
	}
	
	public void Activate(bool unFreezePosLock)
	{
		Rigidbody rigid = transform.GetComponent<Rigidbody>();
		if(rigid == null) 
		{
			rigid = transform.gameObject.AddComponent<Rigidbody>();
//			rigid.constraints = RigidbodyConstraints.FreezeAll;
		}
		
//		if(unFreezePosLock)
//		{
//			rigid.constraints = ~RigidbodyConstraints.FreezeAll;
//		}
	}
	
	public void setFallDuration(float val)
	{
		FallDuration = val;
		WarningTime = FallDuration - WarningDuration;
	}
}
