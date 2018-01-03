using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCheckerScript : MonoBehaviour 
{
	public GameObject mMasterCopy;
	public List<GameObject> mEnemyGruntList;
	public List<GameObject> mEnemyPLList;
	public List<GameObject> mMorphingGruntList;
	public int mMaxLeaderCount = 1, mMaxGruntCount = 6;
	public float mCheckerTimer, mCheckerTimerMax;
	public bool mSlotAvailable,mRespawnerAvailable;
	
	public bool IrvHack = false;
	
	// Use this for initialization
	void Start () 
	{
		mSlotAvailable = false;
		mCheckerTimer = 0.0f;
		mCheckerTimerMax = 2.0f;
		GameObject[] temparray = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in temparray)
		{
			mEnemyGruntList.Add(enemy);	
		}
		temparray = GameObject.FindGameObjectsWithTag("Enemy2");
		foreach(GameObject enemyLead in temparray)
		{
			mEnemyPLList.Add(enemyLead);
		}
		MorphChecker();
	}
	
	// Update is called once per frame
	void Update () 
	{
		MorphChecker();
		if(mEnemyGruntList.Count < mMaxGruntCount)
		{
			GameObject temp = (GameObject)Instantiate(mMasterCopy,mMasterCopy.transform.position,Quaternion.identity);
			temp.SetActive(true);
			mEnemyGruntList.Add(temp);
		}
	}
	
	public void OnGruntDeath(GameObject grunt)
	{
		for(int i = 0; i < mMorphingGruntList.Count; i++)
		{
			if(mMorphingGruntList[i] == grunt)
			{
				mMorphingGruntList.Remove(grunt);
			}
		}
		mEnemyGruntList.Remove(grunt);
	}
	
	public void OnPLDeath(GameObject toonLead)
	{
		mEnemyPLList.Remove(toonLead);
	}
	
	void MorphChecker()
	{
		if(mMorphingGruntList.Count + mEnemyPLList.Count < mMaxLeaderCount)
		{
			mSlotAvailable = true;
			//Debug.Log("Slot IS available!");
		}
		if(mSlotAvailable && mEnemyGruntList.Count > 0)
		{
			GameObject tempGrunt = mEnemyGruntList[Random.Range(0,mEnemyGruntList.Count)];				
			tempGrunt.AddComponent<EnemyMorpherScript>();
			tempGrunt.GetComponent<EnemyMorpherScript>().SetMorphType(1);
			tempGrunt.GetComponent<EnemyMorpherScript>().mManagerObject = this.gameObject;
			mMorphingGruntList.Add(tempGrunt); 
			//Debug.Log("PlatoonLead added!");
			
			if(mMorphingGruntList.Count + mEnemyPLList.Count >= mMaxLeaderCount)
			{
				mSlotAvailable = false;
				//Debug.Log("Slot is NOT available!");
			}
		}
	}
	
	void OnGUI()
	{
		if(!mRespawnerAvailable && !IrvHack)
		{
			GUI.Box(new Rect(Screen.width/2-50,Screen.height/2-20,100,40),"You Win!");
		}
	}
}
