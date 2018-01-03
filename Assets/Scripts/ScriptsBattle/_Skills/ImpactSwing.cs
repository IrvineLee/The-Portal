using UnityEngine;
using System.Collections;

public class ImpactSwing : MonoBehaviour 
{
	
	public float cooldownDuration;
	public float chargeDuration;
	public float range;
	public float force;
	public int damage;
	public ForceMode forceMode;
	
	bool mIsUpdate;
	bool mCharging;
	bool mReady = true;
	float mCooldownTimer;
	float mChargeTimer;
	
	// Use this for initialization
	void Start () 
	{
		mCooldownTimer = cooldownDuration;
		mChargeTimer = chargeDuration;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mIsUpdate == false) return;
		
		if (mReady == false)
		{
			mCooldownTimer -= Time.deltaTime;
			if (mCooldownTimer <= 0)
			{
				mIsUpdate = false;
				mReady = true;
			}
		}
		else if (mReady == true)
		{
			mCharging = true;
			mChargeTimer -= Time.deltaTime;
			
			if (mChargeTimer <= 0)
			{
				mCharging = false;
				RaycastHit hit;
		
				if (Physics.SphereCast(transform.position, 2.0f, transform.forward, out hit, range, ~(1 << LayerMask.NameToLayer("CompulsaryCollider")))) 
				{
					if(hit.rigidbody)
					{
						if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Enemy2"))
						{
							//hit.transform.gameObject.GetComponent<EnemyScript>().DealDamage(damage);
							hit.transform.gameObject.GetComponent<DestructableObjectBaseScript>().mKnockback = true;
							
							Debug.Log ("Impact Swing!");
						}
						hit.transform.rigidbody.AddForce(transform.forward * force, forceMode);
					}
				}
				
				mReady = false;
				mChargeTimer = chargeDuration;
			}
			
			mCooldownTimer = cooldownDuration;
		}
		
	}
	
	public bool IsUpdate
	{
		set { this.mIsUpdate = value; }
	}
	
	void OnGUI()
	{	
		string str = "";
		
		if (mReady == true) str = "5: B.Smash";
		else if(mReady == false)
		{
			str = "CD : " + (int)mCooldownTimer;
		}
		
		GUI.Box(new Rect(Screen.width * 0.9f, Screen.height * 0.03f, 80, 20), str);
		
		if (mCharging == true) GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.05f), "Charging: " + mChargeTimer);
	}
}
