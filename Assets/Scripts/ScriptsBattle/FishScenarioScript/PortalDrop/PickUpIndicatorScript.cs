using UnityEngine;
using System.Collections;

public class PickUpIndicatorScript : MonoBehaviour 
{
	public Transform mPlayer, mIndicator;
	public PickUpGlowScript mGlowComponent;
	bool mPlayerIsClose;
	public float mProximityLimit;
	// Use this for initialization
	void Start () 
	{
		mGlowComponent = mIndicator.GetComponent<PickUpGlowScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(ProximityCheck())
		{
			if(!mIndicator.gameObject.activeSelf)
			{
				mIndicator.gameObject.SetActive(true);
				mGlowComponent.InitializeScript();
			}
			else return;
		}
		else
		{
			if(mIndicator.gameObject.activeSelf && !mGlowComponent.mPlayerLeftProximity)
				mGlowComponent.mPlayerLeftProximity = true;
			else return;
		}
	}
	bool ProximityCheck()
	{
		Vector3 dist = transform.position - mPlayer.transform.position;
		float tempf = dist.sqrMagnitude;
		if (tempf < mProximityLimit * mProximityLimit)return true;
		else return false;
	}
}
