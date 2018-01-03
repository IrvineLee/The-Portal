using UnityEngine;
using System.Collections;

public class TestScrip : MonoBehaviour {
	public GameObject mGameObject;
	public float mPower;
	// Use this for initialization
	void Start () 
	{
		mPower = -Physics.gravity.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	mGameObject.rigidbody.AddForce(Vector3.up * (rigidbody.mass * mPower),ForceMode.Force);
	}
}
