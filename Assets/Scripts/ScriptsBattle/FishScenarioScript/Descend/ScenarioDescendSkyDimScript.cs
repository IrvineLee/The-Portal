using UnityEngine;
using System.Collections;

public class ScenarioDescendSkyDimScript : MonoBehaviour 
{
	public Color mSkyColor;
	
	Light mLightComponent;
	public float mLightRecoverIntensity = 0.15f, mLightFlashIntensity = 2.0f;
	public float mLerpOverSecond, mLerpCounter;
	// Use this for initialization
	void Start () 
	{
		mLerpCounter = 0.0f;
		
		mSkyColor = Color.white;
		Camera.main.backgroundColor = mSkyColor;
		
		mLightComponent = GameObject.Find("Directional light").GetComponent<Light>();
		mLightComponent.intensity = mLightFlashIntensity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Camera.main.backgroundColor = Color.Lerp(mSkyColor, Color.black, mLerpCounter);
		mLightComponent.intensity = Mathf.Lerp(mLightFlashIntensity, mLightRecoverIntensity, mLerpCounter);
		
		if(mLerpCounter < 1.0f)
		{
			mLerpCounter += (Time.deltaTime / mLerpOverSecond) * 2.0f;
		}
		else
			this.enabled = false;
	}
}
