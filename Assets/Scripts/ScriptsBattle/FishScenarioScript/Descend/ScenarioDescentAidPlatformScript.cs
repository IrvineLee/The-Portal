using UnityEngine;
using System.Collections;

public class ScenarioDescentAidPlatformScript : MonoBehaviour 
{
	public bool mIsUp;
	public Vector3 mMoveDestination, mMoveSource;
	public float mTravelDistance, mTravelSpeed, mTravelCounter = 0.0f;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	void OnColliderEnter(Collider collider)
	{}
}
