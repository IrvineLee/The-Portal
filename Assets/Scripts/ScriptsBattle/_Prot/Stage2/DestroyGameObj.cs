using UnityEngine;
using System.Collections;

public class DestroyGameObj : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) 
	{
		DestroyObject(other.gameObject);
	}
}
