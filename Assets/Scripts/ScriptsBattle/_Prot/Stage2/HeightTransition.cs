using UnityEngine;
using System.Collections;

public class HeightTransition : Elevator 
{
	GameObject mInstantiatedParticle;
	float mYPos, mForce;
	
	void Update () 
	{
		Vector3 pos = transform.position;
		pos.y += mForce;
		transform.position = pos;
		
		if((mState == Elevator.State.MOVE_UP && pos.y >= mYPos) 
			|| (mState == Elevator.State.MOVE_DOWN && pos.y <= mYPos))
		{
			ResetToDefault(gameObject);
			Destroy(mInstantiatedParticle);
			enabled = false;
		}
	}

	public void SetVariables(State state, float yPos, float force, GameObject targetUponArrival, GameObject particle)
	{
		mState = state;
		mYPos = yPos;
		TargetUponArrivalGO = targetUponArrival;
		ParticleGO = particle;
		
		if(mState == Elevator.State.MOVE_UP) mForce = force;
		else if(mState == Elevator.State.MOVE_DOWN) mForce = -force;
		
		mInstantiatedParticle = (GameObject) Instantiate (ParticleGO, transform.position, Quaternion.identity);
		mInstantiatedParticle.transform.parent = transform;
	}
}
