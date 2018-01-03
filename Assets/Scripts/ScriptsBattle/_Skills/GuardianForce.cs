using UnityEngine;
using System.Collections;

public class GuardianForce : MonoBehaviour 
{
	public float CoolDown = 5.0f;
	public float Radius = 7.0f;
	public float Force = 30.0f;
	public float SlowDuration = 3.0f;
	public bool IsDrawGizmo = false;
	
	GameObject mPlayerDummy;
	float mCDTimer;
	bool mIsUpdate = false;
	bool mIsCD = false;
	
	void Start () 
	{
		mCDTimer = CoolDown;
	}
	
	void Update () 
	{
		if(!mIsUpdate) return;
		
		if(mIsCD)
		{
			mCDTimer -= Time.deltaTime;
			if(mCDTimer <= 0) 
			{
				mIsUpdate = false;
				mIsCD = false;
			}
		}
		else if(!mIsCD)
		{
			ExplodeImpact();
			mIsCD = true;
		}
	}
	
	public bool IsUpdate
	{
		set { this.mIsUpdate = value; }
	}
	
	void ExplodeImpact()
	{
		Collider[] targets = Physics.OverlapSphere(transform.position, Radius, 1 << 12);
				
		for (int i = 0; i < targets.Length; ++i)
		{
			Vector3 tempDirection = targets[i].transform.position - transform.position;
			float tempDistance = tempDirection.magnitude;
			tempDirection.y = 0.0f;
			
			if(tempDistance < Radius) 
			{
				targets[i].transform.rigidbody.AddForce (tempDirection.normalized * Force, ForceMode.Impulse);
				
				if(targets[i].tag == "Enemy") 
				{
					EnemyScript grunt = targets[i].GetComponent<EnemyScript>();
					grunt.mTarget = null;
					grunt.mEnemyState = EnemyScript.EnemyState.ES_Idle;
					grunt.mFoundTarget = false;
				}
				else if(targets[i].tag == "Enemy2") 
				{
					SquadLeaderScript leader =targets[i].GetComponent<SquadLeaderScript>();
					leader.Target = null;
					leader.mState = SquadLeaderScript.State.IDLE;
				}
			}
		}
		mCDTimer = CoolDown;
		Debug.Log ("Explode Impact!!");
	}
	
	void OnGUI()
	{
		string str = "";
		
		if(!mIsCD) str = "4: Guardian";
		else if(mIsCD)
		{
			int cd = (int)mCDTimer;
			str = "CD : " + cd.ToString();
		}
		
		GUI.Box(new Rect(Screen.width * 0.8f, Screen.height * 0.03f,80,20), str);
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
