using UnityEngine;
using System.Collections;

public class AdditionalGravityScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		rigidbody.AddForce(-Vector3.up * 1.0f * Time.deltaTime);
	}
}
