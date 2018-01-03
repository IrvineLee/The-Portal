using UnityEngine;
using System.Collections;

public class LightningAOE : MonoBehaviour 
{
	public GameObject mAOE_GO;
	public int Dmg = 1;
	public int Heal = 5;
	public bool Is1HitDmg = true;
	
	float mEndHeight;
	bool mIsGetEndHeight = true;
	bool mIsHitPlayer = false;
	
	PlayerScript mPlayerScript;
	
	void Start()
	{
		mPlayerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
	}
	
	void Update()
	{
		float explodeRad = mAOE_GO.renderer.bounds.size.x / 2.0f;
		Collider[] targets = Physics.OverlapSphere(transform.position, explodeRad, 1 << 10 | 1 << 12);
		
		// Damage player and heal enemies.
		for (int i = 0; i < targets.Length; ++i)
		{
			if (targets[i].transform.CompareTag("Player")) 
			{
				if(!mIsHitPlayer) 
				{
					if(Is1HitDmg) 
					{
						Dmg = 1;
						mIsHitPlayer = true;
					}
					
					mPlayerScript.GetDamaged(Dmg);
				}
			}
			else if (targets[i].transform.CompareTag("Enemy"))
			{
				EnemyScript enemy = targets[i].transform.GetComponent<EnemyScript>();
				enemy.HealHp (Heal);
			}
		}
	}
}
