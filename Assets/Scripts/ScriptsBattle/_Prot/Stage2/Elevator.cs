using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour 
{
	public GameObject ParticleGO;
	public GameObject DestinationGO;
	public float Force = 0.1f;
	public enum State
	{
		MOVE_UP = 0,
		MOVE_DOWN
	};
	public State mState = State.MOVE_UP;
	public GameObject TargetUponArrivalGO;
	public bool IsAppliedToPlayer = true;
	
	bool mIsTriggered = false;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag != "Player" && other.gameObject.tag != "Enemy") return;
		
		// Reached destination.
		if(other.GetComponent<HeightTransition>() != null)
		{
			Destroy(other.GetComponent<HeightTransition>());
			return;
		}
		
		if(other.tag == "Player") 
		{
			if(!IsAppliedToPlayer) return;
			other.GetComponent<PlayerScript>().SetGravity (0);
			TogglePlayerActive(other.gameObject, false);
		}
		else if(other.tag == "Enemy")
		{
			Collider collider = other.GetComponent<Collider>();
			collider.enabled = false;
			
			Rigidbody rigid = other.GetComponent<Rigidbody>();
			rigid.mass = 0.1f;
			rigid.useGravity = false;
			Vector3 vel = rigid.velocity;
			vel *= 0.2f;
			rigid.velocity = vel;
			
			other.GetComponent<MeshRenderer>().enabled = false;
			
			EnemyScript grunt = other.GetComponent<EnemyScript>();
			grunt.mTarget = null;
			grunt.mFoundTarget = false;
			grunt.enabled = false;
		}
		
		float y = DestinationGO.transform.position.y;
		other.gameObject.AddComponent<HeightTransition>();
		if(TargetUponArrivalGO == null)
		{
			other.gameObject.GetComponent<HeightTransition>().SetVariables (mState, y, Force, null, ParticleGO);
		}
		else
		{
			other.gameObject.GetComponent<HeightTransition>().SetVariables (mState, y, Force, TargetUponArrivalGO, ParticleGO);
		}
	}
	
	protected void ResetToDefault(GameObject go)
	{
		Collider collider = go.GetComponent<Collider>();
		collider.enabled = true;
		
		if(go.tag == "Player") 
		{
			go.GetComponent<PlayerScript>().SetGravity (3.0f);
			TogglePlayerActive(go, true);
		}
		else if(go.tag == "Enemy")
		{
			Rigidbody rigid = go.GetComponent<Rigidbody>();
			rigid.mass = 1.0f;
			rigid.useGravity = true;
			
			go.GetComponent<MeshRenderer>().enabled = true;
			
			EnemyScript grunt = go.GetComponent<EnemyScript>();
			grunt.mTarget = TargetUponArrivalGO;
			grunt.enabled = true;
		}
	}
	
	void TogglePlayerActive(GameObject player, bool toggle)
	{
		player.GetComponent<PlayerScript>().LockMovement (!toggle);
		player.GetComponent<CharacterController>().enabled = toggle;
		player.transform.FindChild ("Blade_Warrior_Base_All").gameObject.SetActive (toggle);
	}
}
