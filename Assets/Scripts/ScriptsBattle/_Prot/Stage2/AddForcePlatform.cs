using UnityEngine;
using System.Collections;

public class AddForcePlatform : MonoBehaviour 
{
	public float Force = 0.1f;
	public GameObject EnemyTargetGO;
	
	bool mIsAddForce = false;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag != "Player" && other.gameObject.tag != "Enemy") return;
		
		if(other.gameObject.tag == "Player") other.GetComponent<PlayerScript>().SetGravity (0);
		else 
		{
			Rigidbody rigid = other.GetComponent<Rigidbody>();
			rigid.mass = 0.1f;
			rigid.useGravity = false;
			Vector3 vel = rigid.velocity;
			vel *= 0.2f;
			rigid.velocity = vel;
			//rigid.constraints = RigidbodyConstraints.FreezePositionZ;
			
			EnemyScript grunt = other.GetComponent<EnemyScript>();
			grunt.mTarget = null;
			grunt.mFoundTarget = false;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		Vector3 pos = other.transform.position;
		pos.y += Force;
		other.transform.position = pos;
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag != "Player" && other.gameObject.tag != "Enemy") return;
		
		if(other.gameObject.tag == "Player") other.GetComponent<PlayerScript>().SetGravity (3.0f);
		else if(other.gameObject.tag == "Enemy")
		{
			Rigidbody rigid = other.GetComponent<Rigidbody>();
			rigid.mass = 1.0f;
			rigid.useGravity = true;
			EnemyScript grunt = other.GetComponent<EnemyScript>();
			grunt.mTarget = EnemyTargetGO;
		}
	}
}
