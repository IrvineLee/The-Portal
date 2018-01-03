using UnityEngine;
using System.Collections;

public class Iaijutsu : MonoBehaviour 
{
	
	public float cooldownDuration;
	public float chargeDuration;
	public float radius;
	public float range;
	
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
				RaycastHit[] targets = Physics.SphereCastAll(transform.position, radius, transform.forward, range);
				
				for (int i = 0; i < targets.Length; ++i)
				{
					if (targets[i].transform.CompareTag("Enemy") || targets[i].transform.CompareTag("Enemy2"))
					{
						Debug.Log("Iaijutsu");
						Destroy(targets[i].transform.gameObject);
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
		
		if (mReady == true) str = "Iaijutsu";
		else if(mReady == false)
		{
			str = "CD : " + (int)mCooldownTimer;
		}
		
		GUI.Box(new Rect(Screen.width * 0.5f, 10, Screen.width * 0.1f, Screen.height * 0.04f), str);
		
		if (mCharging == true) GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.05f), "Charging: " + mChargeTimer);
	}
}
