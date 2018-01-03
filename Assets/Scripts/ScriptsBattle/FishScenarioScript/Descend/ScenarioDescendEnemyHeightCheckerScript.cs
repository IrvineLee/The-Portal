using UnityEngine;
using System.Collections;

public class ScenarioDescendEnemyHeightCheckerScript : MonoBehaviour 
{
	public float mHeightValue, mMaxAllowedDistance;
	// Use this for initialization
	void Start () 
	{
		mMaxAllowedDistance = transform.position.y - mHeightValue;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.position.y <= mMaxAllowedDistance)
		{
			Destroy(this.gameObject);
		}
	}
}
