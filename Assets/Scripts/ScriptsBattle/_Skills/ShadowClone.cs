using UnityEngine;
using System.Collections;

public class ShadowClone : MonoBehaviour 
{
	public float CoolDown = 5.0f;
	public float Radius = 6.0f;
	public float TimeToExplode = 2.0f;
	public float StunDuration = 2.0f;
	public float ChangeTargetPerc = 75.0f;
	public bool IsDrawGizmo = false;
	
	public enum Type
	{
		PLAYER = 0,
		CLONE
	};
	public Type mType = Type.PLAYER;
	
	enum State
	{
		CONFUSE = 0,
		STUN
	};
	State mState = State.CONFUSE;
	
	GameObject mPlayerDummy;
	float mCDTimer;
	float mExplodeTimer = 0.0f;
	public bool mIsUpdate = false;
	bool mIsCD = false;
	
	void Start () 
	{
		mCDTimer = CoolDown;
		mPlayerDummy = Resources.Load ("ShadowClone") as GameObject; 
	}
	
	void Update () 
	{
		if(!mIsUpdate) return;
		
		if(mType == Type.PLAYER)
		{
			// Countdown for player.
			mCDTimer -= Time.deltaTime;
			if(mCDTimer <= 0) 
			{
				mCDTimer = CoolDown;
				mIsCD = false;
				mIsUpdate = false;
			}
		}
		else if(mType == Type.CLONE)
		{
			// Bomb timer. Only happens in clone.
			mExplodeTimer += Time.deltaTime;
			if(mExplodeTimer >= TimeToExplode) AffectTargetWithinRange(State.STUN);
		}
	}
	
	public bool IsUpdate
	{
		set 
		{
			this.mIsUpdate = value; 
			ActivateClone();
			AffectTargetWithinRange(State.CONFUSE);
		}
	}
	
	public void EnableDummy(float timeToExplode, float stunDuration)
	{
		mType = Type.CLONE;
		mIsUpdate = true;
		IsDrawGizmo = true;
		TimeToExplode = timeToExplode;
		StunDuration = stunDuration;
	}
	
	void ActivateClone()
	{
		mIsCD = true;
		
		Instantiate (mPlayerDummy, transform.position, transform.rotation);
		mPlayerDummy.transform.position = transform.position;
		
		ShadowClone dummy = mPlayerDummy.GetComponent<ShadowClone>();
		dummy.enabled = true;
		dummy.EnableDummy(TimeToExplode, StunDuration);
	}
	
	void AffectTargetWithinRange(State state)
	{
		mState = state;
		
		Collider[] targets = Physics.OverlapSphere(mPlayerDummy.transform.position, Radius, 1 << 12);
				
		for (int i = 0; i < targets.Length; ++i)
		{
			if (targets[i].transform.CompareTag("Enemy"))
			{
				EnemyScript enemy = targets[i].transform.GetComponent<EnemyScript>();
				if(mState == State.CONFUSE) RandomizeTarget(enemy);
				else if(mState == State.STUN) enemy.StunEnemy(StunDuration);
			}
			else if (targets[i].transform.CompareTag("Enemy2"))
			{
				SquadLeaderScript enemy = targets[i].transform.GetComponent<SquadLeaderScript>();
				if(mState == State.CONFUSE) RandomizeTarget(enemy);
				else if(mState == State.STUN) enemy.Stun(StunDuration);
			}
			
		}
		
		if(mState == State.STUN)
		{
			DestroyObject(GameObject.FindGameObjectWithTag("Dummy").gameObject);
			ResetToDefault();
			Debug.Log ("Boom Stun!!");
		}
	}
	
	void RandomizeTarget(EnemyScript enemy)
	{
		// ChangeTargetPerc chance of enemy targeting the clone.
		float randNo = Random.Range(0.0f, 1.0f);
		float percent = ChangeTargetPerc / 100.0f;
		
		if(randNo <= percent) enemy.mTarget = mPlayerDummy.transform.gameObject;
	}
	
	void RandomizeTarget(SquadLeaderScript enemy)
	{
		// ChangeTargetPerc chance of enemy targeting the clone.
		float randNo = Random.Range(0.0f, 1.0f);
		float percent = ChangeTargetPerc / 100.0f;
		
		if(randNo <= percent) enemy.Target = mPlayerDummy.transform.gameObject;
	}
	
	void ResetToDefault()
	{
		mExplodeTimer = 0.0f;
		mState = State.CONFUSE;
		mIsUpdate = false;
	}
	
	void OnGUI()
	{
		string str = "";
		
		if(!mIsCD) str = "3: S.Clone";
		else if(mIsCD)
		{
			int cd = (int)mCDTimer;
			str = "CD : " + cd.ToString();
		}
		
		GUI.Box(new Rect(Screen.width * 0.7f, Screen.height * 0.03f,80,20), str);
	}
	
	void OnDrawGizmos()
	{
		if(IsDrawGizmo)
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = transform.localToWorldMatrix;
		
			int segments = 32;
			float segmentAngle = 360 * Mathf.Deg2Rad / (float)segments;
			Vector3 prev = new Vector3(1.0f, 0.2f, 0.0f) * Radius;
			
			for(int i = 1; i <= segments; i++)
			{
				float angle = segmentAngle * i;
				Vector3 next = new Vector3(Mathf.Cos (angle), 0.2f, Mathf.Sin(angle)) * Radius;
				Gizmos.DrawLine(prev, next);
				prev = next;
			}
		}
	}
}
