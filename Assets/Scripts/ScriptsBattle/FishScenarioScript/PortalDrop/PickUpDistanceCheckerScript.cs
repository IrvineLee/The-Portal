using UnityEngine;
using System.Collections;

public class PickUpDistanceCheckerScript : MonoBehaviour 
{
	PickUpGlowScript mGlowScript;
	public GameObject mTarget, mLightSource;
	public float mDistance, mYOffset = 1.0f, mCastRadius = 1.0f;
	public LayerMask mPlayerLayer;
	public bool mTargetIsClose = false;
	Vector3 mTargetPos;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		mTargetPos = mTarget.transform.position;
		mTargetPos.y += mYOffset;
		
		Vector3 dir = Vector3.Normalize(mTarget.transform.position - transform.position);
		Debug.DrawRay(transform.position, dir * mDistance);
		Ray ray = new Ray(transform.position, dir);
		if(Physics.SphereCast(ray, mCastRadius, mDistance, mPlayerLayer))
		{
			if(!mTargetIsClose)
			{
				mTargetIsClose = true;
				mLightSource.SetActive(mTargetIsClose);
				mLightSource.GetComponent<PickUpGlowScript>().InitializeScript();
			}
			//run
		}
		else
		{
			if(mTargetIsClose)
			{
				mTargetIsClose = false;
				mLightSource.GetComponent<PickUpGlowScript>().mPlayerLeftProximity = true;
				//run
			}
		}
	}
}
