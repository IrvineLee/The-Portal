using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : DestructableObjectBaseScript
{
	public static List<EnemyScript> EnemyList = new List<EnemyScript>();
	EnemyCheckerScript mEnemyCheckerScript;
	public GameObject mTarget;
	public bool mIsDead , mFoundTarget, mStunned, mLostTarget;
	public float mStunTimerCurr, mStunTimerDuration;
	public float mKnockbackDuration;
	public float mKnockbackForce;
	public Vector3 mPlayerLastKnownSpot;
	public float Dmg = 1;
	public float mProximityLimit;
	public bool mPounce = false, mHitPlayer = false;
	
	float mTrackTimeLimit = 2.0f,mTrackTime = 0.0f;
	
	public LayerMask mPlayerLayerMask;
	public LayerMask mEnemyLayer;
	
	public enum EnemyState
	{
		ES_Idle,
		ES_FoundTarget,
		ES_Stunned,
		ES_Knockback,
		ES_LostTarget,
		ES_Total
	};
	public EnemyState mEnemyState;
	
	TextMesh mHPText, mStatusText;
	public float mDetectionRadius, mMaxAllowedVelocityMagnititude, mMoveSpeed = 20.0f;
	float mKnockbackTimer;
	
	// Use this for initialization
	void Start () 
	{
		MaxHp = CurrHp = 10;
		mHPText = transform.FindChild("HPText").GetComponent<TextMesh>();
		mStatusText = transform.FindChild("StatusText").GetComponent<TextMesh>();
		mDetectionRadius = 10.0f;
		mFoundTarget = false;
		mMaxAllowedVelocityMagnititude = 50.0f;
		mEnemyState = EnemyState.ES_Idle;
		mStunTimerCurr = mStunTimerDuration = 0.0f;
		mKnockbackTimer = mKnockbackDuration;
		mEnemyCheckerScript = GameObject.Find("ManagerObject").GetComponent<EnemyCheckerScript>();
		EnemyList.Add(this);
		mPlayerLayerMask = (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("IgnorePhysics"));
		mEnemyLayer = 1<< LayerMask.NameToLayer("Enemy");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mStunned && !mKnockback)
		{
			if(!mFoundTarget)
			{
				////block of code to check for player in proximity
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, mDetectionRadius, mPlayerLayerMask);
				if(hitColliders.Length != 0)
				{
					for(int i = 0 ; i < hitColliders.Length ; i++)
					{
						if(LOSToPlayerCheck(hitColliders[i].transform))
						{
							mTarget = hitColliders[i].gameObject;
							mFoundTarget = true;
							mLostTarget = false;
						}
					}
					
					////////disable from here if do not want group swarm
					if(mFoundTarget)
					{
						hitColliders = Physics.OverlapSphere(transform.position, mDetectionRadius * 1.5f, mEnemyLayer);
						for(int i =0; i < hitColliders.Length; i++)
						{
							if(!hitColliders[i].CompareTag("Enemy2"))
							{
								hitColliders[i].GetComponent<EnemyScript>().mFoundTarget = true;
								hitColliders[i].GetComponent<EnemyScript>().mTarget = mTarget;
							}
						}
					}
					///////
				}
			}
			else
			{////if player leaves proximity or break line of sight, nullify target and stop chasing
				if(Vector3.Distance(transform.position,mTarget.transform.position) >  mDetectionRadius * 2.0f || !LOSToPlayerCheck(mTarget.transform))
				{
					mTarget = null;
					mFoundTarget = false;
					//mEnemyState = EnemyState.ES_Idle;
				}
			}
		}
		else if (mKnockback == true)
		{
			mEnemyState = EnemyState.ES_Knockback;
			mKnockbackTimer -= Time.deltaTime;
			
			if (mKnockbackTimer <= 0)
			{
				mKnockback = false;
				mEnemyState = EnemyState.ES_Idle;
				mKnockbackTimer = mKnockbackDuration;
			}
		}
		else
		{
			mEnemyState = EnemyState.ES_Stunned;
			mStunTimerCurr += Time.deltaTime;
			if(mStunTimerCurr > mStunTimerDuration)
			{
				mStunned = false;
				mFoundTarget = false;
				mEnemyState = EnemyState.ES_Idle;
			}
		}
		
		/////////previous addforce area
		
		
		
		mHPText.text = CurrHp.ToString();
		if(mEnemyState == EnemyState.ES_Idle)
		{
			mStatusText.text = ".";
		}
		else if(mEnemyState == EnemyState.ES_FoundTarget)
		{
			mStatusText.text = "!";
		}
		else if(mEnemyState == EnemyState.ES_Stunned)
		{
			mStatusText.text = "?";
		}
		else if (mEnemyState == EnemyState.ES_Knockback)
		{
			mStatusText.text = "Knockback";
		}
		else if (mEnemyState == EnemyState.ES_LostTarget)
		{
			mStatusText.text = "!!??";
		}
		
		
		
		if(CurrHp <= 0 && !mIsDead)
		{
			mIsDead = true;
		}
		if(mIsDead)
		{
			GameObject.Destroy(gameObject);
		}
	}
	
	void FixedUpdate()
	{
		if(mHitPlayer)
		{
			Vector3 tempvec	= transform.up + (-transform.forward * 2.0f);
			rigidbody.AddForce(tempvec * 5.0f,ForceMode.VelocityChange);
			mHitPlayer = false;
		}
		
		if(mTarget && !mStunned && !mKnockback)
		{///if player is found, simulate chase (look at player and add forward velocity)
			if(mTarget.tag == "Player" && !GetComponent<EnemyPounceScript>().enabled) GetComponent<EnemyPounceScript>().enabled = true;
			mEnemyState = EnemyState.ES_FoundTarget;
			//----- Fish : changes made (2-11-13), tweaked direction setting to eliminate vertical look rotation
			Vector3 targetDir = mTarget.transform.position;
			targetDir.y = transform.position.y;
			targetDir = targetDir  - transform.position;
			//----- end change
	        float step = 50.0f * Time.deltaTime;
	        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
	        Debug.DrawRay(transform.position, newDir, Color.white);
	        transform.rotation = Quaternion.LookRotation(newDir);
			
			if(transform.rigidbody.velocity.magnitude < mMaxAllowedVelocityMagnititude && (Vector3.Distance(transform.position,mTarget.transform.position) > mProximityLimit))
			{
				transform.rigidbody.AddForce(transform.forward * mMoveSpeed,ForceMode.Acceleration);
			}
		}
		if(mEnemyState == EnemyState.ES_LostTarget)
		{
			mTrackTime += Time.deltaTime;
			transform.LookAt(mPlayerLastKnownSpot);
			if(transform.rigidbody.velocity.magnitude < mMaxAllowedVelocityMagnititude && (Vector3.Distance(transform.position,mPlayerLastKnownSpot) > mProximityLimit))
			{
				transform.rigidbody.AddForce(transform.forward * mMoveSpeed,ForceMode.Acceleration);
			}
			if(mTrackTime > mTrackTimeLimit)
			{
				mEnemyState = EnemyState.ES_Idle;
				mTrackTime = 0.0f;
			}
		}
		
		IsGroundChecker();
	}
	
	bool LOSToPlayerCheck(Transform objectToCheck)
	{
		Vector3 tempvec	= objectToCheck.position;
		tempvec.y += 1.0f;
		Debug.DrawLine(transform.position,tempvec,Color.red);
		RaycastHit hit;
		if(Physics.Linecast(transform.position,tempvec,out hit,~(1 << LayerMask.NameToLayer("Enemy"))))
		{
			if(hit.collider.CompareTag("Player"))
			{
				return true;
			}
		}
		if(mTarget)
		{	
			mEnemyState = EnemyState.ES_LostTarget;
			tempvec.y = transform.position.y;
			mPlayerLastKnownSpot = tempvec;
		}
		return false;
	}
	
	public void StunEnemy(float duration)
	{
		mStunTimerDuration = duration;
		mStunTimerCurr = 0.0f;
		mStunned = true;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{///if collide with player, reduce player health. does nothing as of now.
			collision.gameObject.GetComponent<PlayerScript>().GetDamaged(Dmg);
			mHitPlayer = true;
			//Debug.Log(collision.gameObject.GetComponent<PlayerScript>().mCurrHealth);
		}
		
		if (mEnemyState == EnemyState.ES_Knockback)
		{
			if (collision.gameObject.CompareTag("Enemy"))
			{
				collision.gameObject.GetComponent<DestructableObjectBaseScript>().mKnockback = true;
			}
		}
	}
	
	void IsGroundChecker()
	{
		if(!Physics.Raycast(transform.position,Vector3.down, 0.5f))
		{
			rigidbody.AddForce((rigidbody.mass * Physics.gravity),ForceMode.Acceleration);
		}
		Debug.DrawRay(transform.position,Vector3.down * 0.5f);
	}
	
	void OnDestroy()
	{
		mEnemyCheckerScript.OnGruntDeath(gameObject);
		EnemyList.Remove (this);
	}
}
