using UnityEngine;
using System.Collections;

public class PickupFeedbackScript : MonoBehaviour 
{
	public Transform mKeyMesh, mParticleEmitter;
	public float mTimer, mAnimSpeed;
	bool mProgress;
	// Use this for initialization
	void Start () 
	{
		mProgress = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mProgress)
		{
			mTimer -= Time.deltaTime;
			if(mTimer < 0.0f)
			{
				mProgress = true;
				SetupFeedback();
			}
		}
		if(mProgress)
		{
			mTimer -= Time.deltaTime;
			if(mTimer < 0.0f)
			{
				gameObject.SetActive(false);
			}
		}
	}
	
	void SetupFeedback()
	{
		mKeyMesh = transform.Find("KeyMesh");
		mKeyMesh.gameObject.SetActive(false);
		mParticleEmitter = transform.Find("ParticleEmitter");
		mParticleEmitter.gameObject.SetActive(true);
		mTimer = mParticleEmitter.particleSystem.duration * 1.5f;
	}
}
