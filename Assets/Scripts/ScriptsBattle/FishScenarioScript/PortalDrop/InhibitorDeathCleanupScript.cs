using UnityEngine;
using System.Collections;

public class InhibitorDeathCleanupScript : MonoBehaviour 
{
	PortalDropScript mPortalDropScript;
	// Use this for initialization
	void Start () 
	{
		mPortalDropScript = GameObject.Find("Portal").GetComponent<PortalDropScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnDestroy()
	{
		Debug.Log("inhibitor destroyed");
		mPortalDropScript.mInhibitors.Remove(this.gameObject);
	}
}
