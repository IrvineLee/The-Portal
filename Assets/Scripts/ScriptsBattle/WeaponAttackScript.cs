using UnityEngine;
using System.Collections.Generic;

public class WeaponAttackScript : MonoBehaviour 
{
	public static WeaponAttackScript Instance { get; set; }
	
	public int BaseWeaponDmg = 10;
	public float mDetectionRadius = 10.0f;
	
	GameObject mPlayer;
	List<DestructableObjectBaseScript> mHitList = new List<DestructableObjectBaseScript>();
	int mWeaponPower;
	bool mIsAttacked = false;
	public LayerMask mMask;
	
	public GameObject mTrail;
	
	void Start () 
	{
		mTrail = transform.GetChild(0).gameObject;
		if (Instance != null && Instance != gameObject)
			Destroy(gameObject);

		Instance = this;
		mPlayer = GameObject.FindGameObjectWithTag("Player");
	}
	
	void Update()
	{
		mTrail.SetActive(collider.enabled);
		Collider[] hitColliders = Physics.OverlapSphere(mPlayer.transform.position, mDetectionRadius, mMask);
		int currDetectedEnemy = 0;
		for(int i = 0; i < hitColliders.Length; i++)
		{
			if(hitColliders[i].tag == "Enemy" || hitColliders[i].tag == "Enemy2")
			{
				currDetectedEnemy++;
			}
		}
		
		int dmgReduction = currDetectedEnemy;
		if(currDetectedEnemy < 2) 
		{
			dmgReduction = 0;
		}
		mWeaponPower = Mathf.Clamp(BaseWeaponDmg - dmgReduction, 1, BaseWeaponDmg);
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(!mIsAttacked) return;
//		Debug.Log(collider.name);
		GameObject collidedGO = collider.gameObject;
		//if(collidedGO.tag == "Untagged" || collidedGO.tag == "Player" || collidedGO.tag == "Floor1") return;
		
		DestructableObjectBaseScript destructScript = collidedGO.GetComponent<DestructableObjectBaseScript>();
		if(destructScript != null && !destructScript.IsHit)
		{
			destructScript.IsHit = true;
			mHitList.Add (destructScript);
			destructScript.DealDamage(mWeaponPower);
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(!mIsAttacked) return;
		Debug.Log(collision.gameObject.name);
		GameObject collidedGO = collision.gameObject;
		//if(collidedGO.tag == "Untagged" || collidedGO.tag == "Player" || collidedGO.tag == "Floor1") return;
		
		DestructableObjectBaseScript destructScript = collidedGO.GetComponent<DestructableObjectBaseScript>();
		if(destructScript != null && !destructScript.IsHit)
		{
			destructScript.IsHit = true;
			mHitList.Add (destructScript);
			destructScript.DealDamage(mWeaponPower);
		}
	}
	
	public bool IsAtk
	{
		get { return mIsAttacked; }
		set { mIsAttacked = value; }
	}
	
	public void ClearHitList()
	{
		foreach(DestructableObjectBaseScript destructScript in mHitList)
		{
			destructScript.IsHit = false;
		}
		mHitList.Clear ();
		mIsAttacked = false;
	}
}
