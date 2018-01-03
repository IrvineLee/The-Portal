using UnityEngine;
using System.Collections;

public class SceneStage1Script : MonoBehaviour 
{
	
	EdgeColorLerpScript mEdgeLerpScript;
	bool mChange = false, mGlowEdge = false;
	public GameObject mObject;
	public float mTimer, mTimerDelay = 3.0f, radius = 50.0f, power = 500.0f , mMaxFallLimit = -200.0f;
	public LayerMask mGroundLayer;
	Collider[] colliders;
	public ForceMode mForceMode;
	// Use this for initialization
	void Start () 
	{
		mEdgeLerpScript = GetComponent<EdgeColorLerpScript>();
		
		mTimer = mTimerDelay;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void FixedUpdate()
	{
		if(!mGlowEdge)
		{
			mEdgeLerpScript.enabled = true;
			mEdgeLerpScript.InitializeTimer(mTimer);
			mGlowEdge = true;
		}
		mTimer -= Time.deltaTime;
		if(mTimer <= 0.0f && !mChange)
		{
			Debug.Log("Boom");
			Vector3 explosionPos = transform.position;
			colliders = Physics.OverlapSphere(explosionPos, radius , mGroundLayer);
			foreach (Collider hit in colliders) 
			{
				if (hit.rigidbody)
				{
					Vector3 dir = hit.transform.position - transform.position;
					hit.rigidbody.AddForce(dir * power, mForceMode);
					//hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 0.0f, ForceMode.Impulse);
				}
			}
			mChange = true;
			mTimer = mTimerDelay;
		}
		else if(mTimer <= 0.0f && mChange)
		{
			foreach (Collider hit in colliders) 
			{
				if (hit.rigidbody)
					hit.rigidbody.isKinematic = true;
			}
			transform.parent.rigidbody.isKinematic = false;
			transform.parent.rigidbody.useGravity = true;
		}
		if(transform.position.y < mMaxFallLimit)
		{
			transform.parent.gameObject.SetActive(false);
		}
	}
}
