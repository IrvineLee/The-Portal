using UnityEngine;
using System.Collections;

public class TriggerEnabler : MonoBehaviour 
{
	public GameObject TriggersGo;
	
	public void SetActive(bool toggle)
	{
		foreach(Transform child in TriggersGo.transform)
		{
			if(child.GetComponent<BoxCollider>() == null) continue;
			child.GetComponent<BoxCollider>().enabled = toggle;
		}
	}
}
