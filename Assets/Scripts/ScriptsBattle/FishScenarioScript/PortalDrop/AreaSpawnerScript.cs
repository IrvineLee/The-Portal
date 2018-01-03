using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaSpawnerScript : MonoBehaviour 
{
	public GameObject mEnemyMasterCopy;
	public float mTimer, mTimerDuration;
	public List<GameObject> mEnemyList;
	public int mEnemyLimit;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		mTimer += Time.deltaTime;
		
		if(mTimer > mTimerDuration)
		{
			SpawnEnemy();
		}
	}
	
	void SpawnEnemy()
	{
		ListUpdate();
		while(mEnemyList.Count < mEnemyLimit)
		{
			float tempx = transform.collider.bounds.size.x * 0.4f;
			float tempz = transform.collider.bounds.size.z * 0.4f;
			Vector3 tempVec = new Vector3(
				Random.Range(transform.position.x - tempx, transform.position.x + tempx),//x
					transform.position.y + 3.0f,//y
						Random.Range(transform.position.z - tempz, transform.position.z + tempz));//z + end
			
			GameObject tempObj = (GameObject)Instantiate(mEnemyMasterCopy, tempVec, Quaternion.identity);
			tempObj.SetActive(true);
			mEnemyList.Add(tempObj);
		}
	}
	
	void ListUpdate()
	{
		for(int i = 0; i < mEnemyList.Count; i++)
		{
			if(!mEnemyList[i].gameObject)
				mEnemyList.Remove(mEnemyList[i]);
		}
	}
}
