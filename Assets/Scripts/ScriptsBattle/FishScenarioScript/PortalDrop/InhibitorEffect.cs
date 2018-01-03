using UnityEngine;
using System.Collections;

public class InhibitorEffect : MonoBehaviour 
{
	public GameObject mTarget;
	
	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(mTarget.transform.position);
	}
}
