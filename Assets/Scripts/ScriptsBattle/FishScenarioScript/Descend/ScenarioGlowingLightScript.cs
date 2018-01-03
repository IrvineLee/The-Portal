using UnityEngine;
using System.Collections;

public class ScenarioGlowingLightScript : MonoBehaviour 
{
	public Light mLightComponent;
	public float mGlowIntensity, mLerpCounter, mLerpOverSecond;
	GameObject mPlayer;
	// Use this for initialization
	void Start () 
	{
		mPlayer = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(mPlayer.transform.position);
		mLightComponent.intensity = Mathf.Lerp(0, mGlowIntensity, mLerpCounter);
		
		if(mLerpCounter < 1.0f)
		{
			mLerpCounter += (Time.deltaTime / mLerpOverSecond);
		}
		else
		{
			this.enabled = false;
		}
	}
}
