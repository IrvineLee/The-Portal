using UnityEngine;
using System.Collections;

public class ScenarioDescendLightFlashScript : MonoBehaviour 
{
	Light mLightComponent;
	public float mLightFlashIntensity = 8.0f;
	public float mLerpOverSecond, mLerpCounter;
	// Use this for initialization
	void Start () 
	{
		mLerpCounter = 0.0f;
		
		mLightComponent = GetComponent<Light>();
		mLightComponent.intensity = mLightFlashIntensity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		mLightComponent.intensity = Mathf.Lerp(mLightFlashIntensity, 0.0f, mLerpCounter);
		
		if(mLerpCounter < 1.0f)
		{
			mLerpCounter += (Time.deltaTime / mLerpOverSecond) * 2.0f;
		}
		else
		{
			mLerpCounter = 0.0f;
			gameObject.SetActive(false);
		}
	}
}
