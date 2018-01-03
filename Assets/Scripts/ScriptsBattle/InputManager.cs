using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour 
{
	List<KeyCode> mRegisteredKey;
	List<InputKeyAndTime>mPressedKey;
	KeyCode mForward,mBackward,mLeft,mRight,mJump,mAttack,mGuard,mDash;
	KeyCode mSkill1,mSkill2,mSkill3;//skills
	
	float mMaxAllowedDelay, mPrevTime;
	KeyCode mPreviousKey, mCurrKey;
	
	public delegate void OnKeyPress(KeyCode key);
	public delegate void OnKeyHold(KeyCode key);
	public delegate void OnKeyRelease(KeyCode key);
	public delegate void OnDoubleTap(KeyCode key);
	
	public static event OnKeyPress KeyPressEvent;
	public static event OnKeyHold KeyHoldEvent;
	public static event OnKeyRelease KeyReleaseEvent;
	public static event OnDoubleTap DoubleTapEvent;
	
	
	// Use this for initialization
	void Start () 
	{
		mRegisteredKey = new List<KeyCode>();
		mPressedKey = new List<InputKeyAndTime>();
		
		mRegisteredKey.Add(mForward = KeyCode.W);
		mRegisteredKey.Add(mBackward = KeyCode.S);
		mRegisteredKey.Add(mLeft = KeyCode.A);
		mRegisteredKey.Add(mRight = KeyCode.D);
		mRegisteredKey.Add(mJump = KeyCode.Space);
		mRegisteredKey.Add(mAttack = KeyCode.Mouse0);
		mRegisteredKey.Add(mGuard = KeyCode.Mouse1);
		mRegisteredKey.Add(mSkill1 = KeyCode.Alpha1);
		mRegisteredKey.Add(mSkill1 = KeyCode.Alpha2);
		mRegisteredKey.Add(mSkill1 = KeyCode.Alpha3);
		mRegisteredKey.Add(mSkill1 = KeyCode.Alpha4);
		mRegisteredKey.Add(mSkill1 = KeyCode.Alpha5);
		mRegisteredKey.Add(mSkill1 = KeyCode.Alpha6);
		mRegisteredKey.Add(mDash = KeyCode.LeftShift);
		
		mMaxAllowedDelay = 0.18f;
		mPrevTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKey)
		{
			for(int i = 0; i < mRegisteredKey.Count; i++)
			{
				if(Input.GetKey(mRegisteredKey[i]))
				{
					//Debug.Log("checking down");
					KeyHoldEvent(mRegisteredKey[i]);
				}
			}			
		}
		
		if(Input.anyKeyDown)
		{
		for(int i = 0; i < mRegisteredKey.Count; i++)
		{
				if(Input.GetKeyDown(mRegisteredKey[i]) )
				{
					mCurrKey = mRegisteredKey[i];
					//Debug.Log("checking down");
					KeyPressEvent(mRegisteredKey[i]);
					if(Time.time - mPrevTime < mMaxAllowedDelay)
					{
						DoubleTapPress(mPreviousKey, mRegisteredKey[i]);
					}
					mPreviousKey = mRegisteredKey[i];
					mPrevTime = Time.time;
				}
			}
		}
		
		for(int i = 0; i < mRegisteredKey.Count; i++)
		{
			if(Input.GetKeyUp(mRegisteredKey[i]))
			{
				KeyReleaseEvent(mRegisteredKey[i]);
			}
		}
		
		
//		if(Input.GetKeyDown(KeyCode.Z))
//		{
//			mPressedKey.Clear();
//			if(mPressedKey.Capacity > 16)
//			{
//				mPressedKey.TrimExcess();
//			}
//		}
//		Debug.Log(mPressedKey.Count + " , " + mPressedKey.Capacity);
	}
	
	public void DoubleTapPress(KeyCode key1, KeyCode key2)
	{
		if(key1 == key2)
		{
			DoubleTapEvent(key2);
		}
		if(key1 != key2)
		{
			
		}
	}
}



public class InputKeyAndTime
{
	public KeyCode mCapturedKey;
	public float mTime;
	
	public InputKeyAndTime(KeyCode key, float time)
	{
		mCapturedKey = key;
		mTime = time;
	}
};
