using UnityEngine;
using System.Collections;

public class PortalChecking : MonoBehaviour 
{
	SceneManagerScript mSceneManager;
	bool mGame;
	public LayerMask mPlayerLayer;
	// Use this for initialization
	void Start () 
	{
		mPlayerLayer = (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("IgnorePhysics"));
		mSceneManager = GameObject.Find("SceneManager").GetComponent<SceneManagerScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Player")
		{
			mSceneManager.mObjectiveAchieved = true;
		}
	}
}
