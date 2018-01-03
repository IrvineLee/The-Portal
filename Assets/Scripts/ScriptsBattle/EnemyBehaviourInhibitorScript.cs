using UnityEngine;
using System.Collections;

public class EnemyBehaviourInhibitorScript : MonoBehaviour 
{
	EnemyScript mEnemyScript;
	EnemyPounceScript mEnemyPounceScript;
	public float mDetectionRadiusHack = 0.1f;
	public int mPounceProbHack = 0;
	// Use this for initialization
	void Start () 
	{
		mEnemyScript = GetComponent<EnemyScript>();
		mEnemyPounceScript = GetComponent<EnemyPounceScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		mEnemyScript.mDetectionRadius = mDetectionRadiusHack;
		mEnemyPounceScript.mPounceProb = mPounceProbHack;
	}
}
