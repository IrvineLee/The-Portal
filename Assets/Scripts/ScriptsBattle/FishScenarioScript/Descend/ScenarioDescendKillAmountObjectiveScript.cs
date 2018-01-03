using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenarioDescendKillAmountObjectiveScript : MonoBehaviour 
{
	ScenarioDescendBarrierScript mBarrierScript;
	public GameObject mEnemyBlueprint;
	public List<GameObject> mEnemies;
	public int mSpawnCount, mMaxSpawn = 10;
	public float mSizeRange = 5.0f;
	// Use this for initialization
	void Start () 
	{
		mBarrierScript = transform.parent.GetComponentInChildren<ScenarioDescendBarrierScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mBarrierScript.mObjectiveTrigger)
		{
			if(mSpawnCount < mMaxSpawn)
			{
				float rndWidth = Random.Range(transform.position.x - mSizeRange, transform.position.x + mSizeRange);
				float rndLength = Random.Range(transform.position.z - mSizeRange,transform.position.z + mSizeRange);
				float rndHeight = Random.Range(transform.position.y + mSizeRange * 0.25f, transform.position.y + mSizeRange * 0.5f);
				Vector3 tempVec = new Vector3(rndWidth,rndHeight,rndLength);
				GameObject tempObj = (GameObject)Instantiate(mEnemyBlueprint,tempVec,Quaternion.identity);
				tempObj.SetActive(true);
				mEnemies.Add(tempObj);
				mSpawnCount++;
			}
			if(mEnemies.Count <= 0)
			{
				Destroy(this.gameObject);
			}
		}
		ListChecker();
	}
	
	void ListChecker()
	{
		for(int i = 0; i < mEnemies.Count; i++)
		{
			if(!mEnemies[i])
			{
				mEnemies.Remove(mEnemies[i]);
			}
		}
	}
}
