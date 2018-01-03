using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnerSelectorScript : MonoBehaviour 
{
	EnemyCheckerScript mEnemyCheckerScript;
	public List<GameObject> mRespawners;
	public GameObject mActiveRespawner;
	// Use this for initialization
	void Start () 
	{
		mEnemyCheckerScript = GameObject.Find("ManagerObject").GetComponent<EnemyCheckerScript>();
		int i = Random.Range(0,mRespawners.Count);
		mRespawners[i].SetActive(true);
		mEnemyCheckerScript.mRespawnerAvailable = true;
		mActiveRespawner = mRespawners[i];
	}
	
	// Update is called once per frame
	void Update () 
	{	
		if(!mActiveRespawner)
		{
			mEnemyCheckerScript.mRespawnerAvailable = false;
		}
	}
}
