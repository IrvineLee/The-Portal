using UnityEngine;
using System.Collections;

public class TriggerFall : MonoBehaviour 
{
	// Drop plaforms.
	public GameObject DropableGO;
	public GameObject EnableRoofGO;
	public GameObject DisableRoofGO;
	
	public enum TriggerMode
	{
		NONE = 0,
		PLAYER_COLLISION,
	};
	public TriggerMode mTriggerMode = TriggerMode.PLAYER_COLLISION;
	
	public bool IsTimer = false;
	public float FallDuration = 10.0f;
	public bool IsRandDur = false;
	public float MinDur = 0.8f;
	public float MaxDur = 3.0f;
	
	public bool IsDisableFloat = false;
	public bool IsRoof = false;
	
	bool mIsCountdown = false;
	
	float mFallTimer;
	
	void Update()
	{
		if(!mIsCountdown) return;
		
		mFallTimer += Time.deltaTime;
		if(mFallTimer >= FallDuration)
		{
			Activate(true);
			mIsCountdown = false;
		}
	}
	
	void OnTriggerEnter(Collider other) 
	{
		if(other.tag != "Player" || mTriggerMode != TriggerMode.PLAYER_COLLISION) return;
		
		if(IsRandDur) 
		{
			foreach(Transform child in DropableGO.transform)
			{ 
				TimerDrop fallPlatform = child.gameObject.AddComponent<TimerDrop>();
				fallPlatform.setFallDuration(Random.Range (MinDur, MaxDur));
			}
			return;
		}
		else if(IsTimer) 
		{
			mIsCountdown = true;
		}
		
		if(IsRoof)
		{
			if(EnableRoofGO != null)
			{
				if(EnableRoofGO.GetComponent<ObjectDropScript>() == null)
				{
					foreach(Transform child in EnableRoofGO.transform)
					{
						child.GetComponent<ObjectDropScript>().enabled = true;
					}
				}
				else
				{
					EnableRoofGO.GetComponent<ObjectDropScript>().enabled = true;
				}
			}
			
			if(DisableRoofGO != null) DisableRoofGO.GetComponent<ObjectDropScript>().enabled = false;
//			return;
		}
		if(DropableGO != null && !IsTimer) Activate(true);
		if(IsDisableFloat) DisableFloatPlatform();
	}
	
	public void Activate(bool unFreezePosLock)
	{
		foreach(Transform child in DropableGO.transform)
		{ 
			Rigidbody rigid = child.GetComponent<Rigidbody>();
			if(rigid == null) 
			{
				if(child.name == "Plane")
				{
					if(child.GetComponent<MeshCollider>() != null) child.GetComponent<MeshCollider>().enabled = false;
					child.GetComponent<MeshRenderer>().enabled = false;
					continue;
				}
				else 
				{
					rigid = child.gameObject.AddComponent<Rigidbody>();
				}
			}
			rigid.useGravity = true; 
			if(unFreezePosLock) 
			{
				rigid.constraints = ~RigidbodyConstraints.FreezeAll;
				rigid.AddForce(-Vector3.up * 500.0f);
			}
			
			// Scripted event at the edge scene. Falling platform.
			BoxCollider boxCol = child.GetComponent<BoxCollider>();
			if(boxCol == null) continue;
			if(!boxCol.enabled) boxCol.enabled = true;
		}
	}
	
	void DisableFloatPlatform()
	{
		GameObject[] elevatorArray = GameObject.FindGameObjectsWithTag ("Float");
		foreach(GameObject elevator in elevatorArray)
		{
			elevator.SetActive (false);
		}
	}
}
