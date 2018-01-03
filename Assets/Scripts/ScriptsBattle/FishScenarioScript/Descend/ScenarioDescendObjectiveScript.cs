using UnityEngine;
using System.Collections;

public class ScenarioDescendObjectiveScript : MonoBehaviour 
{
	public bool mIsActive = false, mSpecial = false;
	public GameObject mTargetObjective, mTriggerObjective;
	public float mKillCount;
	
	public enum ObjectiveType
	{
		OT_KillAmount,
		OT_KillTarget
	};
	
	public ObjectiveType mObjectiveType;
	
	// Use this for initialization
	void Start () 
	{
		if(mIsActive)
			GetComponentInChildren<ScenarioDescendBarrierScript>().mBarrierDetectionActive = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mTargetObjective && mIsActive)
		{
			mIsActive = false;
		}
		if(mSpecial && mIsActive)
		{
			GetComponentInChildren<ScenarioDescendBarrierScript>().SetBarrierStatus(true);
			mSpecial = false;
		}
	}
}
