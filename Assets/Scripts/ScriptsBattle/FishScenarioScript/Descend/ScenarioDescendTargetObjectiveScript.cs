using UnityEngine;
using System.Collections;

public class ScenarioDescendTargetObjectiveScript : MonoBehaviour 
{
	ScenarioDescendBarrierScript mBarrierScript;
	bool mOneShotTrigger = false;
	public GameObject mTarget;
	// Use this for initialization
	void Start () 
	{
		mBarrierScript = transform.parent.GetComponentInChildren<ScenarioDescendBarrierScript>();
		if(mTarget.activeSelf)
		{
			mTarget.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mOneShotTrigger && mBarrierScript.mObjectiveTrigger)
		{
			mTarget.SetActive(true);
			mOneShotTrigger = true;
		}
		if(!mTarget)
		{
			Destroy(this.gameObject);
		}
	}
}
