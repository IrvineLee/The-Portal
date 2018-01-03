using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectiveScript : MonoBehaviour 
{
	public List<GameObject> mGameObjective;
	bool mObjectiveComplete;
	public int mDeathCount, mInitialEnemyCount;
	// Use this for initialization
	void Start () 
	{
		mObjectiveComplete = false;
		mDeathCount = 0;
		mInitialEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(mDeathCount == mInitialEnemyCount)
		{
			mObjectiveComplete = true;
		}
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect (10,10,150,100),mObjectiveComplete.ToString()))
		{
			if(mObjectiveComplete)
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
}
