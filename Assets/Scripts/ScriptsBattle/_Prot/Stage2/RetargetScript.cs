using UnityEngine;
using System.Collections;

public class RetargetScript : MonoBehaviour 
{
	public GameObject EnemyGO;
	public GameObject TargetGO;
	public GameObject[] mPLGOArray;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag != "Player") return;
		
		foreach(Transform child in EnemyGO.transform)
		{ child.GetComponent<EnemyScript>().mTarget = TargetGO; }
		
		foreach (GameObject pl in mPLGOArray)
		{ 
			if(pl == null) continue;
			pl.GetComponent<SquadLeaderScript>().HoldPositionWithGrunts(); 
		}
	}
	
//	public static void None()
//	{
//		for(int i = 0; i < gruntArray.Length; i++)
//		{
//			if(gruntArray[i] == null) continue;
//			
//			EnemyScript gruntScript = gruntArray[i].GetComponent<EnemyScript>();
//			gruntScript.mTarget = null;
//			gruntScript.mEnemyState = EnemyScript.EnemyState.ES_Idle;
//		}
//	}
}
