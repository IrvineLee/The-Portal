using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenarioDescendBarrierScript : MonoBehaviour 
{
	public bool mBarrierDetectionActive = false, mSpecial = false, mObjectiveTrigger = false;
	public GameObject mTargetObjective, mLightEffectObject;
	public LayerMask mBarrier, mBarrierMarker;
	int mChildCount , mKillCount;
	public List<GameObject> mBarrierMarkerList, mBarrierWallList;
	
	public enum ObjectiveType
	{
		OT_KillAmount,
		OT_KillTarget
	};
	public ObjectiveType mObjectiveType;
	
	// Use this for initialization
	void Start () 
	{
		mBarrier = LayerMask.NameToLayer("Barrier");
		mBarrierMarker = LayerMask.NameToLayer("BarrierMarker");
		
		mChildCount = transform.childCount;
		for(int i = 0; i < mChildCount; i++)
		{
			GameObject tempObj = transform.GetChild(i).gameObject;
			if(tempObj.layer == mBarrierMarker)
			{
				mBarrierMarkerList.Add(tempObj);
			}
			if(tempObj.layer == mBarrier)
			{
				mBarrierWallList.Add(tempObj);
			}
		}
		mBarrierDetectionActive = true;
		mObjectiveTrigger = false;
		SetBarrierStatus(false);
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if(!mTargetObjective && mBarrierDetectionActive)
		{
			mLightEffectObject.SetActive(true);
			mBarrierDetectionActive = false;
			SetBarrierStatus(false);
		}
		if(mSpecial && mBarrierDetectionActive)
		{
			SetBarrierStatus(true);
			mSpecial = false;
		}
	}
	
	public void SetBarrierStatus(bool powerSwitch)
	{
		for(int i = 0; i < mBarrierWallList.Count; i++)
		{
			GameObject tempObj = mBarrierWallList[i];
			if(powerSwitch && (mBarrierDetectionActive || mSpecial))
			{
				tempObj.collider.isTrigger = false;
				mObjectiveTrigger = true;
			}
			else
			{
				tempObj.collider.isTrigger = true;
				mObjectiveTrigger = false;
			}
		}
		
		
		for(int i = 0; i < mBarrierMarkerList.Count; i++)
		{
			GameObject tempObj = mBarrierMarkerList[i];
			if(powerSwitch && (mBarrierDetectionActive || mSpecial))
			{
				tempObj.SetActive(true);
			}
			else
			{
				tempObj.SetActive(false);
			}
		}
	}
}
