using UnityEngine;
using System.Collections;

public class ScenarioDescendBarrierDetectorScript : MonoBehaviour 
{
	public ScenarioDescendBarrierScript mParentBarrierScript;
	// Use this for initialization
	void Start () 
	{
		mParentBarrierScript = transform.parent.GetComponent<ScenarioDescendBarrierScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnCollisionEnter(Collision obj)
	{
		Debug.Log(this.transform.parent.parent.name + " detected " + obj.collider.name);
		if(obj.collider.tag == "Player" || obj.collider.tag == "Weapon")
		{
			mParentBarrierScript.SetBarrierStatus(true);
			Debug.Log(obj.collider.name + " Exit!");
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		Debug.Log(this.transform.parent.parent.name + " detected " + collider.name);
		if(collider.tag == "Player" || collider.tag == "Weapon")
		{
			mParentBarrierScript.SetBarrierStatus(true);
			Debug.Log(collider.name + " Exit!");
		}
	}
}
