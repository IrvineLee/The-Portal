using UnityEngine;
using System.Collections;

public class EnemyMorpherScript : MonoBehaviour 
{
	int mMorphType;
	public GameObject mManagerObject;
	float mFloatHeight, mFloatHeightMagnitude, mFloatSpeed;
	float mMorphTimer,mMorphTimerMax;
	bool mFloatDone;
	// Use this for initialization
	void Start () 
	{
		mFloatSpeed = 10.0f;
		mFloatHeightMagnitude = 2.5f;
		mFloatHeight = transform.position.y + mFloatHeightMagnitude;
		mFloatDone = false;
		
		mMorphTimer = 0.0f;
		mMorphTimerMax = 10.0f;
		gameObject.GetComponent<DestructableObjectBaseScript>().CurrHp = gameObject.GetComponent<DestructableObjectBaseScript>().CurrHp * 2;
		
		this.GetComponent<EnemyScript>().enabled = false;
		//Debug.Log(name + " : EnemyScript disabled!");
		this.GetComponent<EnemyPounceScript>().enabled = false;
		//Debug.Log(name + " : EnemyPounceScript disabled!");
		
	}
	// Update is called once per frame
	void Update () 
	{
		Vector3 temp = transform.position;
		temp.y = mFloatHeight;
		
			if(transform.position.y < temp.y)
			{
				rigidbody.AddForce(transform.up * mFloatSpeed,ForceMode.Acceleration);
			}
			mMorphTimer += Time.deltaTime;
			if(mMorphTimer > mMorphTimerMax)
			{
				SpawnEnemy();
				Destroy(this.gameObject);
			}
//		}
	}
	
	public void SetMorphType(int type)
	{
		mMorphType = type;
	}
	
	public void SpawnEnemy()
	{
		switch (mMorphType)
		{
		case 0:
		{
			Debug.Log("No enemy of such index!");
			break;
		}
		case 1:
		{
			GameObject instance = (GameObject)Instantiate(Resources.Load("SquadLeader"),transform.position,transform.rotation);
			mManagerObject.GetComponent<EnemyCheckerScript>().mEnemyPLList.Add(instance);
			//mManagerObject.GetComponent<EnemyCheckerScript>().mEnemyGruntList.Remove(this.gameObject);
			break;
		}
		}
	}
}
