using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour 
{
	public GameObject ObjToShake;
	
	GameObject mPlayerGO;
	Vector3 mOriPos = Vector3.zero;
	float mTimer;
	float mDuration;
	bool mIsShake = false;
	
	void Start()
	{
		mPlayerGO = GameObject.FindGameObjectWithTag ("Player");
		mOriPos = ObjToShake.transform.position;
	}
	
	void Update () 
	{
		if(!mIsShake) return;
		
		float randNrX = Random.Range(0.1f, -0.1f);
		float randNrY = Random.Range(0.1f, -0.1f);
		float randNrZ = Random.Range(0.1f, -0.1f);
		
		Vector3 randVec = new Vector3(randNrX,randNrY,randNrZ);
		ObjToShake.transform.position = ObjToShake.transform.position + CapIfOverVal(randVec, 0.5f);
		
		mTimer += Time.deltaTime;
		if(mTimer >=  mDuration) 
		{
			transform.position = mOriPos;
			mIsShake = false;
			GetComponent<Scenario2Script>().EnableCam();
			mPlayerGO.GetComponent<PlayerScript>().LockMovement (false);
			mPlayerGO.GetComponent<AnimScript>().LockAnim (false);
		}	
	}
	
	public bool IsShaking
	{
		get { return mIsShake; }
	}
	
	Vector3 CapIfOverVal(Vector3 vec, float val)
	{
		if(vec.x > val) vec.x = val;
		else if(vec.x < -val) vec.x = -val;
		else if(vec.y > val) vec.y = val;
		else if(vec.y < -val) vec.y = -val;
		else if(vec.z > val) vec.z = val;
		else if(vec.z < -val) vec.z = -val;
		
		return vec;
	}
	
	public void Activate(float duration)
	{
		mIsShake = true;
		this.enabled = true;
		mOriPos = ObjToShake.transform.position;
		mDuration = duration;
	}
}
