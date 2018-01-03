using UnityEngine;
using System.Collections;

public class EneGenDest : MonoBehaviour 
{
	ObjectiveScript mObjectiveScript;
	
	void Start()
	{
		mObjectiveScript = GameObject.FindGameObjectWithTag ("Trigger1").GetComponent<ObjectiveScript>();
	}
	
	void OnDestroy()
	{
		mObjectiveScript.IsComplete ();
	}
}
