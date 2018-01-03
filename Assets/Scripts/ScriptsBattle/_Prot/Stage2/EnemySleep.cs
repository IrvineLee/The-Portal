using UnityEngine;
using System.Collections;

public class EnemySleep : MonoBehaviour 
{
	public float Duration;
	
	float mTimer;
	
	void Start()
	{
		GetComponent<EnemyScript>().enabled = false;
	}
	
	void Update () 
	{
		mTimer += Time.deltaTime;
		if(mTimer >= Duration) GetComponent<EnemyScript>().enabled = true;
	}
}
