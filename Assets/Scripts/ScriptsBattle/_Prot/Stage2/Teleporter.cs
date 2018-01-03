using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour 
{
	public GameObject Target;
	public GameObject ParticleEffect;
	public float Spd = 1.0f;
	
	GameObject mPlayerGO;
	GameObject mWeaponGO;
	Vector3 mPlayerStartPos;
	float mLerpTimer;
	bool mIsTeleport = false;
	bool mIsTriggered = false;
	
	GameObject mPEInstance;
	bool mIsPE = false;
	
	UIScript mUIScript;
	
	void Start()
	{
		mWeaponGO = GameObject.FindGameObjectWithTag ("Weapon").gameObject;
	}
	
	void Update()
	{
		if(mIsPE)
		{
			// After particle effect got destroyed.
			if(mPEInstance == null) 
			{
				mIsPE = false;
				mIsTeleport = true;
			}
		}
		else if(mIsTeleport)
		{
			mLerpTimer += Time.deltaTime;
			float t = mLerpTimer * Spd;
		    mPlayerGO.transform.position = Vector3.Lerp(mPlayerStartPos, Target.transform.position, t);
			
			if(t >= 1.0f) 
			{
				mIsTeleport = false;
				
				Vector3 particlePos = Target.transform.position;
				particlePos.y += 1.5f;
				Instantiate (ParticleEffect, particlePos, Quaternion.identity);
				
				TogglePlayerActive(true);
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag != "Player" || mIsTriggered) return;
		
		mPlayerGO = other.gameObject;
		mUIScript = mPlayerGO.GetComponent<UIScript>();
		mUIScript.ShowInteraction("Teleport!");
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag != "Player" || !Input.GetMouseButtonUp (0) || mIsTriggered) return;
		
		Vector3 particlePos = transform.position;
		particlePos.y += 2.0f;
		mPEInstance = (GameObject) Instantiate (ParticleEffect, particlePos, Quaternion.identity);
		mIsPE = true;
		
		mPlayerStartPos = other.transform.position;
		mIsTriggered = true;
		TogglePlayerActive(false);
		mUIScript.DisableInteraction();
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag != "Player" || mIsTriggered) return;
		
		mUIScript.DisableInteraction();
	}
	
	void TogglePlayerActive(bool toggle)
	{
		mPlayerGO.GetComponent<PlayerScript>().LockMovement (!toggle);
		mPlayerGO.GetComponent<CharacterController>().enabled = toggle;
		mPlayerGO.transform.FindChild ("Blade_Warrior_Base_All").gameObject.SetActive (toggle);
		mWeaponGO.GetComponent<BoxCollider>().enabled = false;
	}
}
