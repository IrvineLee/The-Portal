using UnityEngine;
using System.Collections;

public class EnemyPounceScript : MonoBehaviour 
{
	EnemyScript mEnemyScript;
	public float mPounceChargeRate, mPounceChargeTimer, mPounceChargeMax,mProbRefreshTimer,mProbRefreshInterval, mPouncePower;
	public int mPounceProb;
	// Use this for initialization
	void Start () 
	{
		mEnemyScript = GetComponent<EnemyScript>();
		
		mPounceProb = Mathf.RoundToInt(Random.Range(0.0f,1.0f));
		mPounceChargeMax = Random.Range(3.0f,5.0f);
		mPounceChargeRate = Random.Range(1.0f,2.0f);
		mProbRefreshInterval = 15.0f;
		mPounceChargeTimer = Random.Range(0.0f, mPounceChargeMax);
	}
	
	// Update is called once per frame
	void Update () 
	{
		mProbRefreshTimer += Time.deltaTime;
		if(mProbRefreshTimer > mProbRefreshInterval)
		{
			mProbRefreshTimer = 0.0f;
			mPounceProb = Mathf.RoundToInt(Random.Range(0.0f,1.0f));
			if(mPounceProb > 0)mEnemyScript.mPounce = true;
			else mEnemyScript.mPounce = false;
		}
		if(mPounceProb > 0 && mEnemyScript.mTarget && !mEnemyScript.mKnockback)
		{
			mPounceChargeTimer += mPounceChargeRate * Time.deltaTime;
			if(mPounceChargeTimer > mPounceChargeMax && Vector3.Distance(transform.position,mEnemyScript.mTarget.transform.position) > 5.0f)
			{
				Vector3 tempVec = mEnemyScript.mTarget.transform.position;
				tempVec.y = tempVec.y + 5;
				tempVec = tempVec - transform.position;
				Vector3.Normalize(tempVec);
//				transform.rigidbody.AddForce(transform.up + (transform.forward * mPouncePower), ForceMode.Impulse);
				transform.rigidbody.AddForce(tempVec * 1.5f,ForceMode.Impulse);
				transform.rigidbody.AddForce(transform.forward * mPouncePower,ForceMode.Impulse);
				
				mPounceChargeTimer = 0.0f;
				mEnemyScript.mPounce = true;
				mPounceProb = Mathf.RoundToInt(Random.Range(0.0f,1.0f));
			}
		}
	}
}
