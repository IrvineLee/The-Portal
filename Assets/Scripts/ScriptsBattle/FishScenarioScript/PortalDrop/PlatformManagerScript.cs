using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformManagerScript : MonoBehaviour 
{
	public List<GameObject> mScriptedPlatforms;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0; i < mScriptedPlatforms.Count; i++)
		{
			mScriptedPlatforms[i].GetComponent<MovingPlatformScript>().enabled = true;
		}
		this.enabled = false;
	}
}
