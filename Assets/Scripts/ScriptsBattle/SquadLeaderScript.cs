using UnityEngine;
using System.Collections.Generic;

public class SquadLeaderScript : DestructableObjectBaseScript 
{
	EnemyCheckerScript mEnemyCheckerScript;
	public float DetectRadius = 12.0f;
	public float AtkRadius = 10.0f;
	public float FleeRadius = 5.0f;
	public float CallHelpRadius = 10.0f;
	public float MoveSpd = 5.0f;
	public float RotateSpd = 50.0f;
	public float ChargeTime = 2.0f;
	public float FleeDuration = 5.0f;
	public float TiredDuration = 2.0f;
	public float FStep_Range = 5.0f;
	public float FStep_Spd = 2.5f;
	public int Dmg = 2;
	public int Heal = 1;
	public GameObject P_AOE_Base;
	public GameObject P_ATK;
	public GameObject P_Death;
	public float AOE_RotateSpd = 100.0f;
	public float Cd_NextAtk = 2.0f;
	public bool IsDrawGizmo = false;
	public bool IsObjective = false;
	
	// Hacks
	public bool IsLockState = false;
	public bool IsLockChase = false;
	public bool IsLockAtk = false;
	public bool IsLockFlee = false;
	public bool IsTargEnemy = false;
	public bool IsStayHidden = false;
	public bool Is1HitDmg = false;
	
	public enum State
	{
		IDLE = 0,
		FOUND_TARGET,
		CHANTING,
		ATTACK,
		COOLDOWN,
		FLEE,
		TIRED,
		F_STEP,
		STATUS_EFFECT,
		DEAD
	};
	public State mState = State.IDLE;
	
	enum StatusEffect
	{
		FLINCH,
		KNOCKBACK,
		STUN
	};
	StatusEffect mStatusEffect = StatusEffect.FLINCH;
	
	enum FleeMode
	{
		BASIC = 0,
		A_STAR
	};
	FleeMode mFleeMode = FleeMode.BASIC;
	
	// Handle A Star pathfinding.
	int mPathIndex = 0;
	List<Vector3> mPathList = new List<Vector3>();
	
	GameObject mPlayerGO, mTarget, mAOE_GO;
	Vector3 mAOE_Pos;
	TextMesh mHPText, mStatusText;
	float mChargeTimer, mCdTimer, mKbTimer, mStunTimer, mFleeTimer, mTiredTimer, mAlphaSpd, mDefaultAOESpd;
	bool mIsAOE = false;
	bool mIsHitPlayer = false;
	bool mHackMove = false;
	LayerMask mWallLayer;
	
	float mLightningTimer, mLightningDur = 0.09f;
	
	PlayerScript mPlayerScript;
	AStar mAStar;
	PL_Anim mPLAnim;
	
	void Start () 
	{
		CurrHp = MaxHp;
		mDefaultAOESpd = AOE_RotateSpd;
		mHPText = transform.FindChild("HPText").GetComponent<TextMesh>();
		mStatusText = transform.FindChild("StatusText").GetComponent<TextMesh>();
		
		mPlayerGO = GameObject.FindGameObjectWithTag ("Player").gameObject;
		mAStar = GameObject.FindGameObjectWithTag ("Floor1").GetComponent<AStar>();
		mPlayerScript = mPlayerGO.GetComponent<PlayerScript>();
		mEnemyCheckerScript = GameObject.Find("ManagerObject").GetComponent<EnemyCheckerScript>();
		mTarget = mPlayerGO;
		
		mPLAnim = GetComponent<PL_Anim>();
		mWallLayer = 1 << 13 | 1 << 14;
	}
	
	void Update () 
	{
		HandleStates();
		DrawStateAndStatus();
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		if(!WeaponAttackScript.Instance.IsAtk) return;
		
		GameObject collidedGO = collision.collider.gameObject;
		if(collidedGO.tag == "Weapon") mState = State.F_STEP;
    }
	
	public GameObject Target
	{
		set { mTarget = value; }
	}
	
	public void Stun(float duration)
	{
		mStunTimer = duration;
		mState = State.STATUS_EFFECT;
		mStatusEffect = StatusEffect.STUN;
	}
	
	public void Knockback(float duration)
	{
		mKbTimer = duration;
		mState = State.STATUS_EFFECT;
		mStatusEffect = StatusEffect.KNOCKBACK;
	}
	
	public void HoldPositionWithGrunts()
	{
		Collider[] targets = Physics.OverlapSphere(transform.position, FleeRadius, 1 << 12);
		
		foreach(Collider col in targets)
		{
			if(col.tag == "Enemy") 
			{
				col.GetComponent<EnemyScript>().mTarget = null;
				col.GetComponent<EnemyPounceScript>().enabled = true;
			}
		}
	}
	
	
	void HandleStates()
	{
		// If is dead, destroy game object.
		HandleDeath();
		
		// If infected with status effects, skip all the other states.
		if(mState == State.STATUS_EFFECT || mState == State.TIRED) 
		{
			// Status effect.
			if(mStatusEffect == StatusEffect.STUN) CountDownTimer(ref mStunTimer);
			if(mStatusEffect == StatusEffect.KNOCKBACK) CountDownTimer(ref mKbTimer);
			
			// Tired.
			if(mState == State.TIRED) CountDownTimer(ref mTiredTimer);

			return;
		}
		
		if(mState == State.F_STEP) 
		{
			FlashStep();
			return;
		}
		
		// If within flee radius, set enemy to flee.
		if(mState != State.FLEE && IsWithinRad(FleeRadius) && !IsLockFlee) 
		{
			mState = State.FLEE;
			mFleeTimer = FleeDuration;
		}
		
		if(mState == State.IDLE)
		{
			mPLAnim.SetAnim (PL_Anim.State.IDLE);
			if(IsWithinRad(DetectRadius)) 
			{
				mTarget = mPlayerGO;
				
				// This happens when player is teleporting. We don't want PL to atk player.
				if(!mTarget.GetComponent<CharacterController>().enabled) return;
				
				mState = State.FOUND_TARGET;
			}
			
			if(IsTargEnemy) 
			{
				Collider[] targets = Physics.OverlapSphere(transform.position, DetectRadius, 1 << 12);
				
				mTarget = targets[0].gameObject;
				mState = State.FOUND_TARGET;
			}
		}
		else if(mState == State.FOUND_TARGET)
		{
			if(transform.FindChild (P_AOE_Base.name) != null)
			{
				// Prev AOE.
				mAOE_GO.GetComponent<LightningAOE>().enabled = false;
			}
			
			bool isLineOfSight = LookAtTarg(true);
			float dist = Vector3.Distance(transform.position, mTarget.transform.position);
			
			if(dist > AtkRadius && dist < DetectRadius && !IsLockChase)
			{
				if(!IsStayHidden && mFleeMode == FleeMode.BASIC && IsWalll(transform.forward, 8.0f, mWallLayer))
				{
					if(mAStar != null)
					{
						Vector3 nextTarg = transform.position + transform.forward * 20.0f;
						//Debug.Log ("AA" + nextTarg);
						mFleeMode = FleeMode.A_STAR;
						mPathList = mAStar.SetDestination(transform.gameObject, nextTarg);
					}
				}
				
				if(mFleeMode == FleeMode.BASIC)
				{
					mPLAnim.SetAnim (PL_Anim.State.WALK);
					mHackMove = true;
//					transform.Translate (transform.forward * 0.1f);
//					transform.rigidbody.AddForce(transform.forward * 60.0f, ForceMode.Acceleration);
				}
			}
			else if(isLineOfSight/* && !IsWalll(transform.forward, 8.0f, mWallLayer)*/)
			{
				mHackMove = false;
				mState = State.CHANTING;
				if(IsLockAtk) mState = State.IDLE;
				mChargeTimer = ChargeTime;
			}
			
			if(mFleeMode == FleeMode.A_STAR)
			{
				Vector3 tempDirection = mPathList[mPathIndex] - transform.position;
				tempDirection.Normalize();
				MovePlayer(tempDirection, MoveSpd);
				
				if(V3Equal(transform.position, mPathList[mPathIndex])) 
				{
					if(mPathIndex < mPathList.Count - 1) mPathIndex += 1;
					else 
					{
						mFleeMode = FleeMode.BASIC;
						mPathIndex = 0;
					}
					
				}
			}
		}
		else if(mState == State.CHANTING && !IsLockAtk)
		{
			if(!mIsAOE) 
			{
				mPLAnim.SetAnim (PL_Anim.State.ATK);
				
				// Show area of effect.
				Vector3 playerPos = mTarget.transform.position;
				playerPos.y += 0.5f;
				
				GameObject temp = Instantiate (P_AOE_Base, playerPos, transform.rotation) as GameObject;
				temp.name = P_AOE_Base.name;
				temp.transform.parent = transform;
				mAOE_GO = temp.transform.FindChild ("AOE").gameObject;
				mAOE_Pos = temp.transform.position;
				
				mIsAOE = true;
			}
			
			mChargeTimer -= Time.deltaTime;
			if (mChargeTimer <= 0) 
			{
				Instantiate (P_ATK, mAOE_Pos, transform.rotation);
				mState = State.ATTACK;
				mIsAOE = false;
			}
		}
		else if(mState == State.ATTACK)
		{
			mLightningTimer += Time.deltaTime;
			if(mLightningTimer >= mLightningDur)
			{
				mAOE_GO.GetComponent<LightningAOE>().enabled = true;
//				Debug.Log ("PL enabled");
				mState = State.COOLDOWN;
				mCdTimer = Cd_NextAtk;
				mLightningTimer = 0.0f;
				if(Is1HitDmg) mIsHitPlayer = false;
			}
			
//				mCdTimer = Cd_NextAtk;
//				mAOE_GO.name = P_AOE.name;
//				mAOE_GO.transform.parent = transform;
//			}
//			else 
//			{
//				mAOE_GO.transform.position = playerPos;
//				mAOE_GO.SetActive (true);
//			}
			// Simulate explosion on particular area.
//			Vector3 aoePos = mAOE_GO.transform.position;
//			aoePos.y += 0.3f;
//			mAOE_GO.transform.position = aoePos;
//			
//			float aoeEndYPos = transform.position.y + 3.0f;
//			
//			if(aoePos.y >= aoeEndYPos) 
//			{
//				mState = State.COOLDOWN;
//				mCdTimer = Cd_NextAtk;
//				mAOE_GO.SetActive (false);
//				//GameObject.Destroy(mAOE_GO);
//				if(Is1HitDmg) mIsHitPlayer = false;
//			}
//			else
//			{
//				float explodeRad = mAOE_GO.renderer.bounds.size.x / 2.0f;
//				Vector3 aoePos = transform.FindChild ("AOE").transform.position;
//				Collider[] targets = Physics.OverlapSphere(aoePos, explodeRad, 1 << 10 | 1 << 12);
//				
//				// Damage player and heal enemies.
//				for (int i = 0; i < targets.Length; ++i)
//				{
//					if (targets[i].transform.CompareTag("Player")) 
//					{
//						if(!mIsHitPlayer) 
//						{
//							if(Is1HitDmg) 
//							{
//								Dmg = 1;
//								mIsHitPlayer = true;
//							}
//							
//							mPlayerScript.GetDamaged(Dmg);
//						}
//					}
//					else if (targets[i].transform.CompareTag("Enemy"))
//					{
//						EnemyScript enemy = targets[i].transform.GetComponent<EnemyScript>();
//						enemy.HealHp (Heal);
//					}
//				}
//			}
		}
		else if(mState == State.COOLDOWN)
		{
			// Cooldown for next attack.
			mCdTimer -= Time.deltaTime;
			if (mCdTimer <= 0) mState = State.IDLE;
		}
		else if(mState == State.FLEE)
		{
			// If chanting spell, stop it.
			if(mAOE_GO != null) 
			{
				mIsAOE = false;
				mAOE_GO.SetActive (false);
				//GameObject.Destroy(mAOE_GO);
			}
			
			// Call for help.
			Collider[] enemyList = Physics.OverlapSphere(transform.position, CallHelpRadius, 1 << 12);
			foreach(Collider enemy in enemyList)
			{
				if(enemy.tag == "Enemy") enemy.GetComponent<EnemyScript>().mTarget = mPlayerGO;
			}
			
			// Become tired after timer is up.
			mFleeTimer -= Time.deltaTime;
			if (mFleeTimer <= 0 && !IsLockState) 
			{
				mState = State.TIRED;
				mTiredTimer = TiredDuration;
			}
			
			LookAtTarg(false);
			
			if(mFleeMode == FleeMode.BASIC && !IsWalkableDir(transform.forward, 15.0f))
			{
				Vector3 nextTarg = Vector3.zero;
				
				// Check left.
				if(IsWalkableDir(-transform.right, 5.0f)) nextTarg = transform.position - (transform.right * 10.0f);
				// Check right.
				else if(IsWalkableDir(transform.right, 5.0f)) nextTarg = transform.position + (transform.right * 10.0f);
				
				if(mAStar != null)
				{
					mFleeMode = FleeMode.A_STAR;
					mPathList = mAStar.SetDestination(transform.gameObject, nextTarg);
				}
			}
			
			if(mFleeMode == FleeMode.BASIC)
			{
				// Basic behavior : Move away from player.
				Vector3 tempDirection = transform.position - mPlayerGO.transform.position;
				tempDirection.y = 0.0f;
				tempDirection.Normalize ();
				MovePlayer(tempDirection, MoveSpd);
			}
			else if(mFleeMode == FleeMode.A_STAR)
			{
				Vector3 tempDirection = mPathList[mPathIndex] - transform.position;
				tempDirection.Normalize();
				MovePlayer(tempDirection, MoveSpd);
				
				if(V3Equal(transform.position, mPathList[mPathIndex])) 
				{
					if(mPathIndex < mPathList.Count - 1) mPathIndex += 1;
					else 
					{
						mFleeMode = FleeMode.BASIC;
						mPathIndex = 0;
					}
					
				}
			}
			
			if(!IsWithinRad(FleeRadius + 5.0f)) mState = State.IDLE;
		}
	}
	
	void FixedUpdate()
	{
		if(mHackMove)
		{
			transform.rigidbody.AddForce(transform.forward * 30.0f, ForceMode.Acceleration);
		}
	}
	
	// Flashstep away upon getting hit.
	void FlashStep()
	{
		Vector3 targetDir = transform.position - mPlayerGO.transform.position;
		targetDir.y = 0.0f;
		float dist = targetDir.magnitude;
		targetDir.Normalize();
		
		transform.position += targetDir * FStep_Spd * Time.deltaTime;
		if(dist >= FStep_Range) 
		{
			mState = State.IDLE;
			if(IsLockState) mState = State.TIRED;
		}
	}
	
	void MovePlayer(Vector3 dir, float Speed)
	{
		transform.Translate (dir * Time.deltaTime * Speed, Space.World);
	}
	
	void HandleDeath()
	{
		if(CurrHp <= 0) mState = State.DEAD;
		if(mState == State.DEAD) 
		{
			GameObject.Destroy(mAOE_GO);
			
			Vector3 pos = transform.position;
			pos.y += 3.5f;
			Instantiate (P_Death, pos, Quaternion.identity);
			
			GameObject.Destroy (gameObject);
		}
	}
	
	// Re-route path if dir is not a floor. Return false if hit nothing.
	bool IsWalkableDir(Vector3 dir, float dist)
	{
		Vector3 rayStartPos = transform.position + (dir * dist);
		
		Ray ray = new Ray(rayStartPos, -transform.up);
		RaycastHit hit;
		float rayCastDistance = 1.0f;
		
		Debug.DrawRay(rayStartPos, -transform.up * 1.0f, Color.white);
		
		if(!Physics.Raycast (ray, out hit, rayCastDistance)) return false;
		return true;
	}
	
	bool IsWalll(Vector3 dir, float dist, LayerMask layer)
	{
		Vector3 rayStartPos = transform.position;
		rayStartPos.y += 1.0f;
		
		Ray ray = new Ray(rayStartPos, dir);
		RaycastHit hit;
		float rayCastDistance = dist;
		
		Debug.DrawRay(rayStartPos, dir * dist, Color.white);
		
		if(Physics.Raycast (ray, out hit, rayCastDistance, layer)) return true;
		return false;
	}
	
	bool LookAtTarg(bool isLookAtTarget)
	{
		// Rotate towards the target.
		Vector3 targetDir = mTarget.transform.position - transform.position;
		targetDir.y = 0.0f;
		if(!isLookAtTarget) targetDir = -targetDir;
		
        float step = RotateSpd * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        Debug.DrawRay(transform.position, newDir, Color.white);
        transform.rotation = Quaternion.LookRotation(newDir);
		
		// Check if within line of sight
        float angle = Vector3.Angle(targetDir, transform.forward);
		if(angle <= 0.2f) return true;
		return false;
	}
	
	bool IsWithinRad(float rad)
	{
		if(mPlayerGO == null) return false;
		
		Vector3 tempDirection = mPlayerGO.transform.position - transform.position;
		if(IsTargEnemy) tempDirection = GameObject.FindGameObjectWithTag("Enemy").transform.position - transform.position;
		
		float tempDistance = tempDirection.magnitude;
		
		if(tempDistance < rad) return true;
		return false;
	}
	
	void CountDownTimer(ref float timer)
	{
		timer -= Time.deltaTime;
		if (timer <= 0 && !IsLockState) mState = State.IDLE;
	}
	
	bool V3Equal(Vector3 a, Vector3 b)
	{
    	return Vector3.SqrMagnitude(a - b) < 0.01f;
    }
	
	void DrawStateAndStatus()
	{
		mHPText.text = CurrHp.ToString();
		
		if(mState == State.IDLE) mStatusText.text = ".";
		else if(mState == State.FOUND_TARGET) mStatusText.text = "!";
		else if(mState == State.CHANTING) mStatusText.text = "~";
		else if(mState == State.ATTACK) mStatusText.text = "Atk";
		else if(mState == State.FLEE) mStatusText.text = "Run";
		else if(mState == State.TIRED) mStatusText.text = "Tired";
		else if(mState == State.STATUS_EFFECT)
		{
			if(mStatusEffect == StatusEffect.FLINCH) mStatusText.text = "Flinch";
			else if(mStatusEffect == StatusEffect.STUN) mStatusText.text = "?";
			else if (mStatusEffect == StatusEffect.KNOCKBACK) mStatusText.text = "Knockback";
		}
	}
	
	void OnDestroy()
	{
		if(Application.isPlaying && IsObjective) KillCount.KillNeeded -= 1;
	}
	
	void OnDrawGizmos()
	{
		if(!IsDrawGizmo) return;
		
		DrawCircle(DetectRadius, Color.red);
		DrawCircle(AtkRadius, Color.yellow);
		DrawCircle(FleeRadius, Color.blue);
		DrawCircle(CallHelpRadius, Color.green);
	}
	
	void DrawCircle(float radius, Color color)
	{
		Gizmos.color = color;
		Gizmos.matrix = transform.localToWorldMatrix;
	
		int segments = 32;
		float segmentAngle = 360 * Mathf.Deg2Rad / (float)segments;
		Vector3 prev = new Vector3(1.0f, 0.0f, 0.0f) * radius;
		
		for(int i = 1; i <= segments; i++)
		{
			float angle = segmentAngle * i;
			Vector3 next = new Vector3(Mathf.Cos (angle), 0.0f, Mathf.Sin(angle)) * radius;
			Gizmos.DrawLine(prev, next);
			prev = next;
		}
	}
}
