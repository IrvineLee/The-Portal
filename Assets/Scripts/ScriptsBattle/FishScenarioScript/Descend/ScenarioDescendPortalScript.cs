using UnityEngine;
using System.Collections;

public class ScenarioDescendPortalScript : MonoBehaviour 
{
	ScenarioDescendEventScript mEventScript;
	public bool mObjDetected, mPortalOpen;
	public float mCastRadius = 0.5f, mCastDistance = 0.5f;
	public float mTimerMax = 1.5f, mTimer;
	// Use this for initialization
	void Start () 
	{
		mEventScript = GameObject.Find("ScenarioEventManager").GetComponent<ScenarioDescendEventScript>();
	}
	
	// Update is called once per frame
//	void Update () 
//	{
//		Debug.DrawRay(transform.position,Vector3.up * (mCastDistance + mCastRadius));
//		mPortalOpen = (bool)mEventScript.mPrimaryObjective;
//		Ray ray = new Ray(transform.position,Vector3.up);
//		RaycastHit hit;
//		if(Physics.SphereCast(ray, mCastRadius, out hit, mCastDistance))
//		{
//			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !mPortalOpen)
//			{
//				Debug.Log("portal not open!");
//			}
//		}
//	}
	void Update () 
	{
		Debug.DrawRay(transform.position,Vector3.up * (mCastDistance + mCastRadius));
		mPortalOpen = !(bool)mEventScript.mPrimaryObjective;
		Ray ray = new Ray(transform.position,Vector3.up);
		RaycastHit hit;
		if(Physics.SphereCast(ray, mCastRadius, out hit, mCastDistance))
		{
			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") && mPortalOpen)
			{
				mEventScript.mEndGame = true;
			}
		}
//		
//		if(mObjDetected)
//		{
//			mTimer -= Time.deltaTime;
//			if(mTimer <= 0.0f)
//				mObjDetected = false;
//		}
	}
}
