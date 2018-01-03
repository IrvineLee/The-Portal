using UnityEngine;
using System.Collections;

public class SimpleController : MonoBehaviour 
{
	/*public float StopRadius = 1.0f;
	public bool IsDrawGizmo = true;*/
	
	Vector3 mMoveDirection;
	float moveSpeed = 2.0f;
	
	CharacterController charController;
	SceneManager mSceneManager;
	
	void Start()
	{
		charController = GetComponent<CharacterController>();
		mSceneManager = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>();
	}
	
	void Update()
	{
		mMoveDirection = Vector3.zero;
		if (Input.GetKey(KeyCode.W)) mMoveDirection += transform.TransformDirection(Vector3.forward);
		if (Input.GetKey(KeyCode.S)) mMoveDirection += transform.TransformDirection(Vector3.back);
		if (Input.GetKey(KeyCode.A)) mMoveDirection += transform.TransformDirection(Vector3.left);
		if (Input.GetKey(KeyCode.D)) mMoveDirection += transform.TransformDirection(Vector3.right);

        mMoveDirection *= moveSpeed;
        charController.Move(mMoveDirection * Time.deltaTime);
		
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		float mRayCastDistance = 2.5f;
		
		if(Physics.Raycast (ray, out hit, mRayCastDistance))
		{
			InfoExchange.IsWinBattle = hit.collider.GetComponent<WinOrLose>().IsWin;
			// End battle. Return to world map.
			if(!AutoFade.Fading) mSceneManager.RestorePreviousScene ();
		}
		
		Debug.DrawRay(transform.position, transform.forward * mRayCastDistance, Color.red);
	}
	
	/*public Vector3 getDirection(Vector3 Target, out bool nextNode)
	{
		Vector3 dir = Target - transform.position;
		float radiusSqr = StopRadius * StopRadius;
		float distSqr = dir.sqrMagnitude;
		if(distSqr >= radiusSqr)
		{
			nextNode = false;
			return dir;
		}
		nextNode = true;
		return Vector3.zero;
	}
	
	void OnDrawGizmos()
	{
		if(IsDrawGizmo)
		{
			Gizmos.color = Color.red;
			Gizmos.matrix = transform.localToWorldMatrix;
			
			int segments = 32;
			float segmentAngle = (2.0f * Mathf.PI) / (float)segments;
			Vector3 prev = Vector3.right * StopRadius;
			
			for(int i = 0; i <= segments; i++)
			{
				float angle = segmentAngle * i;
				Vector3 next = new Vector3(Mathf.Cos (angle), 0.0f, Mathf.Sin(angle)) * StopRadius;
				Gizmos.DrawLine(prev, next);
				prev = next;
			}
		}
	}*/
}
