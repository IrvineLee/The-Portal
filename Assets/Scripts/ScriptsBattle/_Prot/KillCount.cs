using UnityEngine;
using System.Collections;

public class KillCount : MonoBehaviour 
{
	public static bool mIsObjective = true;
	
	public static int KillNeeded;
	
	bool mIsEnabled = false;
	ObjectiveScript mObjectiveScript;
	
	void Start()
	{
		mObjectiveScript = transform.GetComponent<ObjectiveScript>();
		Debug.Log (mObjectiveScript.transform.name);
	}
	
	void Update()
	{
		if(!mIsEnabled) return;
		
		if(KillNeeded <= 0) 
		{
			mObjectiveScript.IsComplete ();
			Debug.Log ("AAA");
		}
	}
	
	public void SetActive(bool toggle)
	{
		this.enabled = toggle;
		mIsEnabled = toggle;
	}
	
	public void SetActive(bool toggle, int killCount)
	{
		this.enabled = toggle;
		mIsEnabled = toggle;
		KillNeeded = killCount;
	}
}
