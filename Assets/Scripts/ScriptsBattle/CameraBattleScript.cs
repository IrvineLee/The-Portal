using UnityEngine;
using System.Collections;

public class CameraBattleScript : MonoBehaviour 
{
	public GameObject mPlayerHead,mCamera;
	
	public float mXAxis, mYAxis, mMinVerticalY = -80.0f, mMaxVerticalY = 80.0f, mSensitivity = 10.0f, mMinSensitivity = 1.0f, mMaxSensitivity = 20.0f;//camera rotation
	public float mDistance = 8.0f, mHeight = 2.0f, mZoomIncrement = 10.0f, mMinZoom = 5.0f, mMaxZoom = 15.0f;// camera zoom
	
	public bool mTrackHead;//head tracking
	public float mMaxTurnAngle;
	
	public LayerMask mObstacleLayer;
	// Use this for initialization
	void Start () 
	{
		mCamera = Camera.main.gameObject;
		Vector3 angles = transform.eulerAngles;
		mXAxis = angles.x;
		mYAxis = angles.y;
		
		mTrackHead = false;
		
		mObstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
        mXAxis += Input.GetAxis("Mouse X") * mSensitivity;
        mYAxis -= Input.GetAxis("Mouse Y") * mSensitivity;
		mYAxis = ClampAngle(mYAxis,mMinVerticalY,mMaxVerticalY);
		
		mDistance += -Input.GetAxis("Mouse ScrollWheel") * mZoomIncrement;
		mDistance = Mathf.Clamp(mDistance,mMinZoom,mMaxZoom);
		
		NoHeadTracking();
	}
	
	static float ClampAngle (float clampValue, float min ,float max)
	{
		if (clampValue < -360.0f)
			clampValue += 360.0f;
		if (clampValue > 360.0f)
			clampValue -= 360.0f;
		return Mathf.Clamp (clampValue, min, max);
		
	}
		
		
	void BodyTrackHead(Transform head)/////////////constant headtracking
	{
		Quaternion headRotation = Quaternion.Euler(mYAxis,mXAxis,0);
        Vector3 position = headRotation * new Vector3(0.0f, 0.0f, -mDistance) + transform.position + (transform.up * mHeight);
        mCamera.transform.rotation = headRotation;
        mCamera.transform.position = Vector3.Lerp(mCamera.transform.position, position, 1.0f);
		
		mPlayerHead.transform.rotation = mCamera.transform.rotation;
		
		Vector3 headVec = mCamera.transform.forward;
		headVec.y = 0.0f;
		Quaternion tempRot = Quaternion.LookRotation(headVec);
		transform.rotation = /*Quaternion.Slerp(transform.rotation,*/tempRot/*,5.0f * Time.deltaTime)*/;
	}
	
	void NoHeadTracking()/////////////no headtracking
	{
		Quaternion headRotation = Quaternion.Euler(mYAxis,mXAxis,0);
        Vector3 position = headRotation * new Vector3(0.0f, 0.0f, -ObstacleCheck()) + transform.position + (transform.up * mHeight);
        mCamera.transform.rotation = headRotation;
        mCamera.transform.position = Vector3.Lerp(mCamera.transform.position, position, 1.0f);
		
		Vector3 headVec = mCamera.transform.forward;
		headVec.y = 0.0f;
		Quaternion tempRot = Quaternion.LookRotation(headVec);
		transform.rotation = /*Quaternion.Slerp(transform.rotation,*/tempRot/*,5.0f * Time.deltaTime)*/;
	}
	
	
	void BodyTrackHeadWithLimit(Transform head)///////////once head exceeds threshold, body will track head until aligned
	{
		Quaternion headRotation = Quaternion.Euler(mYAxis,mXAxis,0);
        Vector3 position = headRotation * new Vector3(0.0f, 0.0f, -mDistance) + transform.position + (transform.up * mHeight);
        mCamera.transform.rotation = headRotation;
        mCamera.transform.position = Vector3.Lerp(mCamera.transform.position, position, 1.0f);
		
		mPlayerHead.transform.rotation = mCamera.transform.rotation;
		
		Vector3 headVec = mCamera.transform.forward;
		headVec.y = 0.0f;
		float diff = Vector3.Angle(transform.rotation * Vector3.forward, headVec);
		
		if(!mTrackHead)
		{
			if(diff > mMaxTurnAngle)
			{
				mTrackHead = true;
			}
		}
		else
		{
			Quaternion tempRot = Quaternion.LookRotation(headVec);
			transform.rotation = Quaternion.Slerp(transform.rotation,tempRot,5.0f * Time.deltaTime);
			
			
			if(Mathf.RoundToInt(diff) == 0)
			{
				mTrackHead = false;
			}
		}
	}
	
	void HeadRotateUntilLimit(Transform head)///////////head will track as long as within constrain
	{
		Vector3 headVec = mCamera.transform.forward;
		headVec.y = 0.0f;
		float diff = Vector3.Angle(transform.rotation * Vector3.forward, headVec);
		Quaternion headRotation = Quaternion.Euler(mYAxis,mXAxis,0);
	    Vector3 position = headRotation * new Vector3(0.0f, 0.0f, -mDistance) + transform.position + (transform.up * mHeight);
	    mCamera.transform.rotation = headRotation;
	    mCamera.transform.position = Vector3.Lerp(mCamera.transform.position, position, 1.0f);
		
		if(diff < mMaxTurnAngle)
		{		
			mPlayerHead.transform.rotation = headRotation;
		}
		else
		{
			Quaternion tempHeadRotation = Quaternion.Euler(mYAxis,mPlayerHead.transform.rotation.eulerAngles.y,0);
			mPlayerHead.transform.rotation = tempHeadRotation;
		}
		if(mTrackHead)
		{
			Quaternion tempRot = Quaternion.LookRotation(headVec);
			transform.rotation = Quaternion.Slerp(transform.rotation,tempRot,5.0f * Time.deltaTime);
			if(Mathf.RoundToInt(diff) == 0)
			{
				mTrackHead = false;
			}
		}
	}
	
	float ObstacleCheck()
	{
		Vector3 tempVec = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		Ray cameraRay = new Ray(tempVec,-mCamera.transform.forward);
		Debug.DrawRay(tempVec,-mCamera.transform.forward * mDistance);
		RaycastHit hitinfo;
		if(Physics.Raycast(cameraRay,out hitinfo, mDistance, mObstacleLayer))
		{
			float tempDist = Vector3.Distance(transform.position, hitinfo.point);
			return tempDist;
		}
		else
		{
			return mDistance;
		}
	}
}
