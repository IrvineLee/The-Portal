using UnityEngine;
using System.Collections;

public class PlayerParentChecking : MonoBehaviour 
{
	public GameObject mParent;
	public float mSphereCastDistGround = 0.5f, mRayDist = 1.0f;
	CharacterController mCharController;
	// Use this for initialization
	void Start ()
	{
		mCharController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.DrawRay(transform.position + new Vector3(0.0f,0.5f,0.0f), -transform.up);
		RaycastHit hitInfo;
		if(Physics.Raycast(transform.position + new Vector3(0.0f,0.5f,0.0f), -transform.up, out hitInfo, mRayDist, ~(LayerMask.NameToLayer("Enemy"))))
		{
			if(!transform.parent || transform.parent != hitInfo.collider)
			{
				transform.parent = hitInfo.transform;
			}
		}
		else
		{
			transform.parent = null;
		}
	}
}
