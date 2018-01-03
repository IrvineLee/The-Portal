using UnityEngine;
using System.Collections;

public class ScenarioDescendAidPlatformScript : MonoBehaviour 
{
	public bool mObjDetected;
	public float mBoostPower = 50.0f, mCastRadius = 0.5f, mCastDistance = 0.5f, mMaxBoostSpeed = 50.0f;
	public float mTimerMax = 1.5f, mTimer;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.DrawRay(transform.position,Vector3.up * (mCastDistance + mCastRadius));
		Ray ray = new Ray(transform.position,Vector3.up);
		RaycastHit hit;
		if(Physics.SphereCast(ray, mCastRadius, out hit, mCastDistance))
		{
			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !mObjDetected)
			{
				Vector3 tempVec = Vector3.zero;
				tempVec.y += mBoostPower;
				hit.collider.gameObject.GetComponent<PlayerScript>().moveDirection = tempVec;
				mObjDetected = true;
				mTimer = mTimerMax;
			}
		}
		
		if(mObjDetected)
		{
			mTimer -= Time.deltaTime;
			if(mTimer <= 0.0f)
				mObjDetected = false;
		}
	}
}
