using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{	
	public Skills[] SkillArray = new Skills[4];
	
	public enum Skills
	{
		NONE = 0,
		SHADOW_CLONE,
		GUARDIAN_FORCE,
		IAIJUTSU,
		IMPACT_SWING
	};
	
	CameraBattleScript mCameraScript;
	CharacterController controller;
	
	float mMovespeed, mMovespeedDefault, mMoveSpeedMultiplier;
	public int mActionPoint, mDashAPConsumption, mDashAlterAPConsumption;
	float mActionPointReplenish;
	public float mExp;
	public float mMaxHealth;
	public float mCurrHealth;
	public float mAwakening;
	float mJumpSpeed, mGravity, mDashStunRadius, mDashStunDuration, mAlternateDashDuration, mMaxFallSpeed;
	public int mLvl;
	int mIgnorPhysicsLayer,mDefaultLayer,mGhostLayer;
	int mWallJumpcount, mWallJumpMax;
	bool mDashModeDoubleTap, mIsDashing, mAlternateDashSkill, mIsGrounded;
	Vector3 moveDir;
	
	Color mDefaultColor;
	public Material mPlayerMaterial;
	
	public Vector3 moveDirection = Vector3.zero;
	
	float mDashTimer, mDashDuration;
	
	public float mPickupRadius;
	public LayerMask mItemLayer;
	
	bool mIsUpdate = true;
	bool mLockMovement = false;
	public float mSphereCastDistGround = 0.5f, mSphereCastDistAir = 0.5f;
	public GameObject mTrail;
	public enum PlayerWeapon
	{
		PW_Sword,
		PW_Spear,
		PW_Gauntlet,
		PW_Scythe,
		PW_Total
	};
	
	public static PlayerWeapon mPlayerWeapon;
	
	public enum PlayerState
	{
		PS_Idle,
		PS_Moving,
		PS_Jumping,
		PS_Dashing,
		PS_Attacking,
		PS_Total
	};
	
	public static PlayerState mPlayerState;
	
	AnimScript mAnimScript;
	ShadowClone mShadowClone;
	GuardianForce mGuardianForce;
	Iaijutsu mIaijutsu;
	ImpactSwing mImpactSwing;
	HealthUI mHealthUI;
	
	void OnKeyPress(KeyCode key)
	{
		if(key == KeyCode.Space)
		{
			Jump();
		}
		//Debug.Log("running down key :" + key);		
		if(key == KeyCode.Mouse0)
		{
			//PlayerShootEvent();
		}
		if(/*key == KeyCode.LeftShift && !mDashModeDoubleTap*/ key == KeyCode.Mouse1 && !mIsDashing)
		{
			mAlternateDashSkill = false;
			DashPlayer();
		}
//		if(key == KeyCode.Alpha1 && !mIsDashing)
//		{
//			mDashModeDoubleTap = !mDashModeDoubleTap;
//		}
		if(key == KeyCode.Alpha2 && !mIsDashing)
		{
			mAlternateDashSkill = true;
			DashPlayer();
		}
		if(key == KeyCode.Alpha3 && !mIsDashing)
		{
			if(SkillArray.Length >= 1) ActivateSkill((int) SkillArray[0]);
		}
		if(key == KeyCode.Alpha4 && !mIsDashing)
		{
			if(SkillArray.Length >= 2) ActivateSkill((int) SkillArray[1]);
		}
		if(key == KeyCode.Alpha5 && !mIsDashing)
		{
			if(SkillArray.Length >= 3) ActivateSkill((int) SkillArray[2]);
		}
		if(key == KeyCode.Alpha6 && !mIsDashing)
		{
			if(SkillArray.Length >= 4) ActivateSkill((int) SkillArray[3]);
		}
		
		
	}
	
	void ActivateSkill(int index)
	{
		switch (index)
		{
			case 0: { Debug.Log ("No skill"); break; }
		    case 1: { mShadowClone.IsUpdate = true; break; }
		    case 2: { mGuardianForce.IsUpdate = true; break; }
			case 3: { mIaijutsu.IsUpdate = true; break; }
			case 4: {mImpactSwing.IsUpdate = true; break; }
		}
	}
	
	void OnKeyHold(KeyCode key)
	{
		if(mLockMovement) return;
		
		if(key == KeyCode.W) MovePlayer(transform.forward);
		if(key == KeyCode.S) MovePlayer(-transform.forward);
		if(key == KeyCode.A) MovePlayer(-transform.right);
		if(key == KeyCode.D) MovePlayer(transform.right);
	}
	
	void OnKeyRelease(KeyCode key)
	{
		//Debug.Log("running release key :" + key);
		
		mPlayerState = PlayerState.PS_Idle;
	}
	
	void OnDoubleTap(KeyCode key)
	{
		if((key == KeyCode.W || key == KeyCode.A || key == KeyCode.S || key == KeyCode.D)/* && mDashModeDoubleTap*/)
		{
			/*DashPlayer();*/
			Debug.Log("D-tap");
		}
	}
	public void SetGravity(float g)
	{
		mGravity = Physics.gravity.y * g;
	}
	
	public bool IsGrounded
	{
		get { return mIsGrounded; }
		set { mIsGrounded = value; }
	}
	
	
	// Use this for initialization
	void Start () 
	{
		if(!mCameraScript)
		{
			mCameraScript = GetComponent<CameraBattleScript>();
		}
		transform.parent = null;
		controller = GetComponent<CharacterController>();
		mActionPoint = 10;
		mExp = 0.0f;
		mCurrHealth = 100.0f;
		mAwakening = 100.0f;
		mMovespeed = 10.0f;
		mMovespeedDefault = mMovespeed;
		mMoveSpeedMultiplier = 1.0f;
		mJumpSpeed = 25.0f;
		mGravity = Physics.gravity.y * 3.0f;
		
		mWallJumpMax = 2;
		mWallJumpcount = 0;
		
		mDashStunRadius = 5.0f;
		mDashStunDuration = 2.0f;
		mAlternateDashDuration = 2.0f;
		mAlternateDashSkill = false;
		
		mDashTimer = 0.0f;
		mDashDuration = 0.1f;
		
		mDashAlterAPConsumption = 5;
		mDashAPConsumption = 2;
		
		//mDefaultColor = mPlayerMaterial.color;
		mIsGrounded = false;
		
		mDashModeDoubleTap = false;
		mIsDashing = false;
		mMaxFallSpeed = -Physics.gravity.y * 30.0f;
		mIgnorPhysicsLayer = LayerMask.NameToLayer("IgnorePhysics");
		mDefaultLayer = LayerMask.NameToLayer("Player");
		mGhostLayer = LayerMask.NameToLayer("Ghost");
		//Debug.Log(mPlayerMaterial.color);
		
		mPlayerState = PlayerState.PS_Idle;
		
		
		mAnimScript = transform.GetComponent<AnimScript>();
		mShadowClone = transform.GetComponent<ShadowClone>();
		mGuardianForce = transform.GetComponent<GuardianForce>();
		mIaijutsu = transform.GetComponent<Iaijutsu>();
		mImpactSwing = transform.GetComponent<ImpactSwing>();
		mHealthUI = transform.GetComponent<HealthUI>();
		
		if(mHealthUI.DisplayType == HealthUI.Type.LIFE)
		{
			mCurrHealth = mMaxHealth = 5;
		}
		EnableRelevantSkills();
		
		InputManager.KeyPressEvent += OnKeyPress;
		InputManager.KeyHoldEvent += OnKeyHold;
		InputManager.KeyReleaseEvent += OnKeyRelease;
		InputManager.DoubleTapEvent += OnDoubleTap;
	}

	void IsGroundChecker()
	{
		Vector3 tempVec = controller.transform.position;
		tempVec.y += 1.0f;
		Ray ray = new Ray(tempVec, -controller.transform.up);
		
		if(!mIsGrounded)
		{
			if(Physics.SphereCast(ray, controller.radius, mSphereCastDistGround, ~(LayerMask.NameToLayer("Enemy"))))
			{
				moveDirection.y = 0.0f;
				mIsGrounded = true;
//				Debug.Log ("ON");
			}
		}
		else
		{
			if(Physics.SphereCast(ray, controller.radius, mSphereCastDistAir, ~(LayerMask.NameToLayer("Enemy"))))
			{
				mIsGrounded = true;
//				Debug.Log ("Player ground");
			}
			else
			{
				mIsGrounded = false;
//				Debug.Log ("Offffffffff");
			}
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if(!mIsUpdate) return;
		if(GetComponent<CharacterController>().enabled == false) return;
		IsGroundChecker();
		
		if(transform.parent && transform.parent.rigidbody)
		{
			controller.Move(transform.parent.rigidbody.velocity * Time.deltaTime);
		}
		
		controller.Move(moveDirection * Time.deltaTime);
		if(!mIsGrounded) 
		{
			if(moveDirection.y < mMaxFallSpeed)
			{
				moveDirection.y += mGravity * Time.deltaTime;
			}
		}
		if(mIsDashing)
		{
			if(!mAlternateDashSkill)
			{
				mDashTimer += Time.deltaTime;
				if(mDashTimer > mDashDuration)
				{
					controller.gameObject.layer = mDefaultLayer;
					mPlayerState = PlayerState.PS_Idle;
					mMoveSpeedMultiplier = 1.0f;
					mIsDashing = false;
					mTrail.SetActive(false);
				}
			}
			else
			{
				mDashTimer += Time.deltaTime;
				float temp = mAlternateDashDuration - mDashTimer;
				
				if(mDashTimer > (mAlternateDashDuration - 0.5f) && mDashTimer < mAlternateDashDuration)
				{
					/*Color tempCol = mPlayerMaterial.color;
					tempCol.a += (mAlternateDashDuration/temp) * Time.deltaTime;
					mPlayerMaterial.color = tempCol;	*/				
				}
				if(mDashTimer > mAlternateDashDuration)
				{
					controller.gameObject.layer = mDefaultLayer;
					mMoveSpeedMultiplier = 1.0f;
					mIsDashing = false;
					mAlternateDashSkill = !mAlternateDashSkill;
					mTrail.SetActive(false);
				}
			}
		}
		if(mActionPoint < 10)
		{
			mActionPointReplenish += Time.deltaTime;
			if(mActionPointReplenish > 1.0f)
			{
				mActionPointReplenish = 0.0f;
				mActionPoint++;
			}
		}
		if(mPlayerState != PlayerState.PS_Idle)
		{
			mCameraScript.mTrackHead = true;
			//mPlayerState = PlayerState.PS_Idle;
		}
//		if(mHealth <= 0)
//		{
//			mAnimScript.PlayDeath();
//			gameObject.layer = LayerMask.NameToLayer("Dead");
//			mIsUpdate = false;
//			//Debug.Log("Died!");
//		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit col)
	{
	}
	
	public void PickUp()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, mPickupRadius, mItemLayer);
		float tempDist = mPickupRadius;
		GameObject tempGO = null;
		for(int i = 0; i < hitColliders.Length; i++)
		{
			Debug.Log("check " + hitColliders[i].name);
			float tempf = Vector3.Distance(hitColliders[i].transform.position,transform.position);
			if(tempf < tempDist)
			{
				tempDist = tempf;
				tempGO = hitColliders[i].gameObject;
			}
		}
		if(tempGO)
		{
			Debug.Log("close " + tempGO.name);
			tempGO.GetComponent<PickupFeedbackScript>().enabled = true;
			tempGO.GetComponent<PickupFeedbackScript>().mTimer = mAnimScript.mPickUpSpeed * 0.5f;
		}
	}
	
	public void MovePlayer(Vector3 dir)
	{
		moveDir = dir;
		controller.Move(moveDir * mMovespeed * mMoveSpeedMultiplier * Time.deltaTime);
		//PlayerMoveEvent(transform.position);
		if(mPlayerState != PlayerState.PS_Moving && mPlayerState != PlayerState.PS_Dashing)
		{
			mPlayerState = PlayerState.PS_Moving;
		}
	}
	
	public void DashPlayer()
	{
		if(!mIsDashing)
		{
			if(!mAlternateDashSkill && (mActionPoint - mDashAPConsumption) >= 0)
			{
				Debug.Log("Dash!");
				mDashTimer = 0.0f;
				mIsDashing = true;
				mMoveSpeedMultiplier = 6.0f;
				controller.gameObject.layer = mIgnorPhysicsLayer;
				mActionPoint-= mDashAPConsumption;
				mTrail.SetActive(true);
			}
			else if(mAlternateDashSkill && (mActionPoint - mDashAlterAPConsumption) >= 0)
			{
				Collider[] targets = Physics.OverlapSphere(transform.position, mDashStunRadius, (1 << 12));
				for(int i = 0; i < targets.Length; i++)
				{
					targets[i].GetComponent<EnemyScript>().StunEnemy(mDashStunDuration);
					Debug.Log("stun");
				}
				mMoveSpeedMultiplier = 2.0f;
				mDashTimer = 0.0f;
				mIsDashing = true;
				controller.gameObject.layer = mGhostLayer;
				
				// material change
				/*Color tempCol = mPlayerMaterial.color;
				tempCol.a = 0.25f;
				mPlayerMaterial.color = tempCol;*/
				mActionPoint-=mDashAlterAPConsumption;
				mTrail.SetActive(true);
			}
			
		}
	}
	
	public void Jump()
	{
		if(mIsGrounded) 
		{
//			Debug.Log("Jump!");
			moveDirection.y = 0.0f;
			moveDirection.y = mJumpSpeed;
			mPlayerState = PlayerState.PS_Moving;
			mWallJumpcount = 0;
		}
		else
		{
			if(mWallJumpcount < mWallJumpMax && (controller.collisionFlags & CollisionFlags.Sides) != 0)
			{
//				Debug.Log("Wall Jump!");
				moveDirection.y = mJumpSpeed;
				mWallJumpcount++;
			}
		}
		
	}
	
	public void ExpIncrease(int expAmount, int multiplier)
	{
		mExp += (expAmount * multiplier);		
	}
	
	public void ChangePlayerState(PlayerState state)
	{
		mPlayerState = state;
	}
	
	public void LockMovement(bool active)
	{
		if(active) mLockMovement = true;
		else if(!active) mLockMovement = false;
	}
	
	public void GetDamaged(float val)
	{
		mCurrHealth -= val;
		mHealthUI.Hp_Ratio = GetHpRatio(mCurrHealth);
	}
	
	float GetHpRatio(float currHp)
	{
		return mCurrHealth / mMaxHealth;
	}
	
	void EnableRelevantSkills()
	{
		for(int i = 0; i < SkillArray.Length; i++)
		{
			EnableSkill(SkillArray[i]);
		}
	}
	
	void EnableSkill(Skills skill)
	{
		if(skill == Skills.SHADOW_CLONE) mShadowClone.enabled = true;
		else if(skill == Skills.GUARDIAN_FORCE) mGuardianForce.enabled = true;
		else if (skill == Skills.IAIJUTSU) mIaijutsu.enabled = true;
		else if (skill == Skills.IMPACT_SWING) mImpactSwing.enabled = true;
	}
	
	void OnGUI()
	{
		string inputMode = "";
		string skillMode = "";
		string skillCoolDown = "";
		if(mDashModeDoubleTap)
		{
			inputMode = "D-Tap";
		}
		else
		{
			inputMode = "L-Shift";
		}
		
		if(mAlternateDashSkill)
		{
			skillMode = "Ghost";
		}
		else
		{
			skillMode = "Flash";
		}

//		GUI.Box(new Rect(10,10,70,20),"HP : " + mHealth.ToString());
//		GUI.Box(new Rect(90,10,70,20),"AP : " + mActionPoint.ToString());
//		GUI.Box(new Rect(10,40,50,20),inputMode);
		if(!mIsDashing)
		{
			skillCoolDown = "2: Ghost";
		}
		else
		{
			skillCoolDown = "On CD";
		}
		GUI.Box(new Rect(Screen.width * 0.6f, Screen.height * 0.03f, 80, 20), skillCoolDown);
//		if(mHealth <= 0)
//		{
//			GUI.Box(new Rect(Screen.width/2-50,Screen.height/2-20,100,40),"You Are Dead");
//		}
	}
	
	void OnDestroy()
	{
		InputManager.KeyPressEvent -= OnKeyPress;
		InputManager.KeyHoldEvent -= OnKeyHold;
		InputManager.KeyReleaseEvent -= OnKeyRelease;
		InputManager.DoubleTapEvent -= OnDoubleTap;
	}
}
