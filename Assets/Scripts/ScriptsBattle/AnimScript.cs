using UnityEngine;
using System.Collections.Generic;

public class AnimScript : MonoBehaviour 
{
	public GameObject playerGO;
	public float AtkTime = 1.0f;
	List<char> keyPressedList = new List<char>();
	
	enum Mode
	{
		NONE = 0,
		MOVE,
		ATK,
		JUMP,
		COLLECT
	};
	Mode mMode = Mode.NONE;
	
	GameObject mWeapon;
	Quaternion mEndRotation;
	float mStartTime;
	float mLerpSpd = 6.0f;
	float mForwardEular = 0.0f;
	float mAtkTimer;
	bool mIsUpdate = true;
	bool mIsAction = false;
	bool mIsJumping = false;
	bool mIsLockMovement = false;
	public float mPickUpSpeed;
	
	float mJumpTimer;
	
	PlayerScript mPlayerScript;
	CameraBattleScript mCameraBattleScript;
	
	void Start () 
	{
		mCameraBattleScript = transform.GetComponent<CameraBattleScript>();
		mPlayerScript = transform.GetComponent<PlayerScript>();
		mWeapon = GameObject.FindGameObjectWithTag("Weapon");
		
		SetAnimWrapMode();
		playerGO.animation["BW_Attack"].speed = 1.5f;
		playerGO.animation["BW_PickUp"].speed = 3.0f;
		mPickUpSpeed = playerGO.animation["BW_PickUp"].length / playerGO.animation["BW_PickUp"].speed;
	}
	
	void Update () 
	{
		if(!mIsUpdate) return;
		
		HandleInput();
		if (!Input.anyKey) ResetToDefault();
	}
	
	public void PlayDeathAnim()
	{
		mIsUpdate = false;
		playerGO.animation.wrapMode = WrapMode.Once;
		playerGO.animation.CrossFade("BW_Death");
	}
	
	public void SetToCollectAnim(bool toggle)
	{
		if(toggle) mMode = Mode.COLLECT;
		else if(!toggle) mMode = Mode.NONE;
	}
	
//	public bool IsJump
//	{
//		set { mIsJumping = value; }
//		get { return mIsJumping; }
//	}
	
	public void LockAnim(bool toggle)
	{
		if(toggle) mIsUpdate = false;
		else if(!toggle) mIsUpdate = true;
	}
	
	void HandleInput()
	{
		HandleActions();
		
		if(!mIsLockMovement && !mIsJumping) 
		{
			HandleMovement();
			if(keyPressedList.Count == 2) Handle2InputMovement();
		}
	}
	
	void HandleActions()
	{
		if(mIsLockMovement && CountDownTimer(ref mAtkTimer))
		{
			Debug.Log ("Unlock!");
			mIsLockMovement = false;
			mPlayerScript.LockMovement (false);
			WeaponAttackScript.Instance.ClearHitList();
			
			BoxCollider boxCol = mWeapon.GetComponent<BoxCollider>();
			if(!boxCol == null) boxCol.enabled = false;
		}
		
		if(Input.GetMouseButtonDown(0)) 
		{
			if(mMode == Mode.NONE) Attack();
			else if(mMode == Mode.COLLECT) Collect();
		}
		else if(Input.GetMouseButtonDown(1)) Dash();
		
		if(Input.GetKeyDown(KeyCode.Space) && !mIsJumping) Jump ();
		
		if(Input.GetKeyDown(KeyCode.E)) PickUp();
		
		if(mIsAction && !playerGO.animation.IsPlaying("BW_Attack") && !playerGO.animation.IsPlaying("BW_PickUp"))
		{
			mIsAction = false;
		}
		
		if(mIsJumping && mPlayerScript.IsGrounded)
		{
			mJumpTimer += Time.deltaTime;
			if(mJumpTimer >= 0.2f)
			{
				mJumpTimer = 0.0f;
				mIsJumping = false;
			}
		}
	}
	
	void Attack()
	{
		if(!playerGO.animation.IsPlaying("BW_Attack") && !mIsLockMovement) 
		{
			mWeapon.GetComponent<BoxCollider>().enabled = true;
			mIsAction = true;
			mIsLockMovement = true;
			mPlayerScript.LockMovement (true);
			WeaponAttackScript.Instance.IsAtk = true;
			playerGO.animation.Play("BW_Attack");
//			mAtkTimer = AtkTime;
			mAtkTimer = playerGO.animation["BW_Attack"].length / playerGO.animation["BW_Attack"].speed;
		}
	}

	void Collect()
	{
		mMode = Mode.NONE;
		mIsAction = true;
		mIsLockMovement = true;
		playerGO.animation.CrossFade("BW_PickUp");
	}
	
	void Jump()
	{
		mIsJumping = true;
		playerGO.animation.CrossFade("BW_Jump");
	}
	
	void PickUp()
	{
		mIsAction = true;
		mIsLockMovement = true;
		mPlayerScript.LockMovement (true);
		mPlayerScript.PickUp();
		mAtkTimer = mPickUpSpeed;
		playerGO.animation.CrossFade("BW_PickUp");
	}
	
	void Dash()
	{
		mIsAction = true;
	}
	
	void HandleMovement()
	{
		mForwardEular = mCameraBattleScript.mXAxis;
		
		if(Input.GetKey(KeyCode.W)) 
		{
			if(Input.GetKeyDown(KeyCode.W)) RegisterInput('W', "BW_Run");
			
			if(keyPressedList.Count < 2 && !mIsAction)
			{
				playerGO.animation.CrossFade("BW_Run");
				if(Mathf.FloorToInt(playerGO.transform.eulerAngles.y) != Mathf.FloorToInt(mForwardEular))
				{ ChangeAngle(playerGO, mForwardEular); }
			}
		}
		else keyPressedList.Remove('W');

		if(Input.GetKey(KeyCode.A)) 
		{
			if(Input.GetKeyDown(KeyCode.A)) RegisterInput('A', "BW_L_Run");

			if(keyPressedList.Count < 2 && !mIsAction)
			{
				playerGO.animation.CrossFade("BW_L_Run");
				if(Mathf.FloorToInt(playerGO.transform.eulerAngles.y) != Wrap0To360(Mathf.FloorToInt(mForwardEular - 45.0f)))
				{ ChangeAngle(playerGO, mForwardEular - 45.0f); }
			}
		}
		else keyPressedList.Remove('A');

		if(Input.GetKey(KeyCode.S)) 
		{
			if(Input.GetKeyDown(KeyCode.S)) RegisterInput('S', "BW_B_Run");

			if(keyPressedList.Count < 2 && !mIsAction)
			{
				playerGO.animation.CrossFade("BW_B_Run");
				if(Mathf.FloorToInt(playerGO.transform.eulerAngles.y) != Mathf.FloorToInt(mForwardEular)) 
				{ ChangeAngle(playerGO, mForwardEular); }
			}
		}
		else keyPressedList.Remove('S');

		if(Input.GetKey(KeyCode.D)) 
		{
			if(Input.GetKeyDown(KeyCode.D)) RegisterInput('D', "BW_R_Run");

			if(keyPressedList.Count < 2 && !mIsAction)
			{
				playerGO.animation.CrossFade("BW_R_Run");
				if(Mathf.FloorToInt(playerGO.transform.eulerAngles.y) != Mathf.FloorToInt(mForwardEular + 45.0f)) 
				{ ChangeAngle(playerGO, mForwardEular + 45.0f); }
			}
		}
		else keyPressedList.Remove('D');
	}
	
	void Handle2InputMovement()
	{
		if(Mathf.FloorToInt(playerGO.transform.eulerAngles.y) != Mathf.FloorToInt(mForwardEular)) 
		{ ChangeAngle(playerGO, mForwardEular); }
		
		if(IsKeyPresses('W', 'A') && !mIsAction) playerGO.animation.CrossFade("BW_L_Run");
		else if(IsKeyPresses('W', 'D') && !mIsAction) playerGO.animation.CrossFade("BW_R_Run");
		else if(IsKeyPresses('S', 'A') && !mIsAction) 
		{
			playerGO.animation["BW_R_Run"].speed = -1.0f;
			playerGO.animation.CrossFade("BW_R_Run");
		}
		else if(IsKeyPresses('S', 'D') && !mIsAction)
		{
			playerGO.animation["BW_L_Run"].speed = -1.0f;
			playerGO.animation.CrossFade("BW_L_Run");
		}
		else if(IsKeyPresses('W', 'S') || IsKeyPresses('A', 'D')) ResetToDefault();
	}
	
	void RegisterInput(char c, string animName)
	{
		keyPressedList.Add (c);
		playerGO.animation[animName].speed = 1.0f;
	}
	
	void ChangeAngle(GameObject go, float angle)
	{
		if(mStartTime == 0.0f) 
		{
			mStartTime = Time.time;
			
			Vector3 eularAngle = transform.rotation.eulerAngles;
			eularAngle.x = 0.0f;
			eularAngle.y = angle;
			mEndRotation = Quaternion.Euler(eularAngle);
		}
		
		float t = (Time.time - mStartTime) * mLerpSpd;
		go.transform.rotation = Quaternion.Lerp(go.transform.rotation, mEndRotation, t);
		
		if(Mathf.FloorToInt(go.transform.rotation.eulerAngles.y) == Mathf.FloorToInt(mEndRotation.eulerAngles.y)) 
		{ mStartTime = 0.0f; }
	}
	
	bool IsKeyPresses(char first, char second)
	{
		if((keyPressedList[0] == first || keyPressedList[0] == second) 
			&& (keyPressedList[1] == first || keyPressedList[1] == second))
		{ return true; }
		else return false;
	}
	
	void ResetToDefault()
	{
		if(Mathf.FloorToInt(playerGO.transform.eulerAngles.y) != Mathf.FloorToInt(mForwardEular)) 
		{
			ChangeAngle(playerGO, mForwardEular);
		}
		if(!mIsAction && !mIsJumping) playerGO.animation.CrossFade("BW_Attack_standy");
	}
	
	void SetAnimWrapMode()
	{
		AnimOnce("BW_Attack");
		AnimOnce("BW_Jump");
		AnimOnce("BW_PickUp");
		
		AnimLoop("BW_Run");
		AnimLoop("BW_L_Run");
		AnimLoop("BW_B_Run");
		AnimLoop("BW_R_Run");
	}
	
	void AnimOnce(string animName)
	{
		playerGO.animation[animName].wrapMode = WrapMode.Once;
	}
	
	void AnimLoop(string animName)
	{
		playerGO.animation[animName].wrapMode = WrapMode.Loop;
	}
	
	bool CountDownTimer(ref float timer)
	{
		timer -= Time.deltaTime;
		if (timer <= 0) return true;
		return false;
	}
	
	int Wrap0To360(int val)
	{
		if(val < 0) val = 360 + val;
		return val;
	}
	
	//	void MovePlayer(GameObject go, char axis, float amount, string animName)
//	{
//		Vector3 pos;
//		
//		pos = go.transform.position;
//		
//		if(axis == 'x') pos.x += amount;
//		else if(axis == 'z') pos.z += amount;
//		
//		go.transform.position = pos;
//		go.animation.CrossFade(animName);
//	}
	
//	void MovePlayer(GameObject go, float x, float z, string animName)
//	{
//		Vector3 pos;
//		
//		pos = go.transform.position;
//		pos.x += x;
//		pos.z += z;
//		
//		go.transform.position = pos;
//		go.animation.CrossFade(animName);
//		Debug.Log ("AAA");
//	
}
