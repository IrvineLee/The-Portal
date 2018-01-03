using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalDropScript : MonoBehaviour 
{
	public GameObject mDeathzone;
	public List<GameObject>mInhibitors;
	public float mForce;
	public float mHeight = 0.0f;
	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < mInhibitors.Count; i++)
		{
			mHeight += mInhibitors[i].transform.position.y;
		}
		mHeight = mHeight / mInhibitors.Count;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.position.y < mHeight)
		{
			for(int i = 0; i < mInhibitors.Count; i++)
			{
				if(mInhibitors[i].gameObject)
				{
					GameObject.Destroy(mInhibitors[i]);
					mInhibitors.Remove(mInhibitors[i]);
				}
			}
			mDeathzone.SetActive(false);
		}
		for(int i = 0; i < mInhibitors.Count; i++)
		{
			if(!mInhibitors[i].gameObject)
				mInhibitors.Remove(mInhibitors[i]);
		}
		
		if(mInhibitors.Count == 4)
			mForce = 12.99f;
		
		else if(mInhibitors.Count == 3)
			mForce = 12.95f;
		
		else if(mInhibitors.Count == 2)
			mForce = 12.3f;
		
		else if(mInhibitors.Count == 1)
			mForce = -3.5f;
	}
	
	void FixedUpdate()
	{
		rigidbody.AddForce(Vector3.up * (rigidbody.mass * mForce),ForceMode.Force);
	}
}
