using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneStage2Script : MonoBehaviour 
{
	public GameObject mPortalObject;
	public List<AreaSpawnerScript> mSpawnerScriptList;
	public List<GameObject>mInhibitors;
	public bool mPlatformActivate = false;
	// Use this for initialization
	void Start () 
	{
		ActivateStageTwoObjects();
		mPlatformActivate = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void ActivateStageTwoObjects()
	{
		mPortalObject.rigidbody.useGravity = true;
		mPortalObject.GetComponent<PortalDropScript>().enabled = true;
		for(int i = 0; i < mInhibitors.Count; i++)
		{
			mInhibitors[i].SetActive(true);
		}
		for(int i = 0; i < mSpawnerScriptList.Count; i++)
		{
			mSpawnerScriptList[i].enabled = true;
		}
	}
}
