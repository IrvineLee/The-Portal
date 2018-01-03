using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour 
{
	public float incDecY = 0.5f;
	public float incDecZ = 3.0f;
	public float incDecEular = 5.0f;
	public float lerpSpeed = 5.0f;
	public float zoomSpeed = 1.5f;
	public float zoomRatio = 0.8f;
	public float rotateSpd = 25.0f;
	
	public enum CameraRotation
	{
		NORMAL = 0,
		REVERSE
	};
	public CameraRotation mCameraRotation = CameraRotation.NORMAL;
	
	[HideInInspector]
	public bool isChangingCam = false;
	[HideInInspector]
	public bool isMovingPlayer = false;
	
	// Set camera properties. (For changing cam Mode)
	Quaternion mEndRotation;
	Vector3 mEndPosition = Vector3.zero;
	float mStartTime = 0.0f;
	
	// Set key for camera rotation.
	KeyCode mLeft;
	KeyCode mRight;
	bool mIsEnableRotation = true;
	
	// Set player properties. (For moving cam)
	Vector3 mSavedPlayerPos = Vector3.zero;
	Vector3 mEndPlayerPosition = Vector3.zero;
	float mStartTimeMovePlayer = 0.0f;
	
	// Zoom in areas.
	Vector3 mSavedCamPos = Vector3.zero;
	bool mIsZoomIn = false;
	
	GameObject mPlayer;
	GameObject mPlayerDummy;
	
	PlayerController mPlayerController;
	FOWRevealer mFOWRevealer;
	
	void Start () 
	{
		mPlayer = GameObject.FindGameObjectWithTag ("Player");
		mPlayerController = mPlayer.GetComponent<PlayerController>();
		mFOWRevealer = mPlayer.GetComponent<FOWRevealer>();
		
		mPlayerDummy = Resources.Load ("Dummy") as GameObject;
		
		transform.LookAt(mPlayer.transform);
		SetCameraRotation();
	}

	void Update () 
	{
		if(mIsEnableRotation) HandleRotation();
		
		if(isChangingCam)
		{
			if (mPlayerController.playerMode == PlayerController.Mode.UNLOCK) SetProperties(incDecY, incDecZ, incDecEular);
			else if (mPlayerController.playerMode == PlayerController.Mode.EXPLORE) SetProperties(-incDecY, -incDecZ, -incDecEular);
		}
		if(isMovingPlayer) HandleMovePlayerPos();
		if(mIsZoomIn) HandleZoomInArea();
	}
	
	public bool RotationActive
	{
		get{ return this.mIsEnableRotation; }
		set{ this.mIsEnableRotation = value; }
	}
	
	public void ChangeCamMode()
	{
		isChangingCam = true;
		
		if (mPlayerController.playerMode == PlayerController.Mode.UNLOCK)
		{ 
			// Save player position.
			mSavedPlayerPos = mPlayer.transform.position; 
			mPlayer.GetComponent<MeshRenderer>().enabled = false;
			Instantiate (mPlayerDummy, mSavedPlayerPos, mPlayer.transform.rotation);
		}
		else if (mPlayerController.playerMode == PlayerController.Mode.EXPLORE)
		{ 
			// Restore player position.
			MovePlayerPos(mSavedPlayerPos);
		}
	}
	
	public void MovePlayerPos(Vector3 position)
	{
		isMovingPlayer = true;
		mFOWRevealer.isEnabled = false;
		mEndPlayerPosition = position;
		mEndPlayerPosition.y = mSavedPlayerPos.y;
	}
	
	public void ZoomInArea(Vector3 position)
	{
		mIsZoomIn = true;
		mSavedCamPos = transform.position;

		float ratioOffset = 1.0f - zoomRatio;
		mEndPosition = (transform.position * ratioOffset + position * zoomRatio);
	}
	
	public void RestoreZoomCam()
	{
		// Return if is still default value.
		if(mSavedCamPos == Vector3.zero) return;
		
		transform.position = mSavedCamPos;
		ResetChangeCamVal();
		mSavedCamPos = Vector3.zero;
		mIsZoomIn = false;
	}
	
	void HandleRotation()
	{
		if (Input.GetKey(mLeft)) 
		{
			mPlayer.transform.Rotate(Vector3.up, rotateSpd * Time.deltaTime);
			mPlayerController.mTHoverCursor.Rotate(-Vector3.up, rotateSpd * Time.deltaTime);
		}
		else if(Input.GetKey(mRight)) 
		{
			mPlayer.transform.Rotate(-Vector3.up, rotateSpd * Time.deltaTime); 
			mPlayerController.mTHoverCursor.Rotate(Vector3.up, rotateSpd * Time.deltaTime);
		}
	}
	
	void SetProperties(float incDecY, float incDecZ, float incDecEular)
	{
		if (mStartTime == 0.0f)
		{
			mStartTime = Time.time;
			
			Vector3 startPos = transform.localPosition;
			mEndPosition = startPos + new Vector3(0.0f, incDecY, incDecZ);

			Vector3 eularAngle = transform.rotation.eulerAngles;
			eularAngle.x += incDecEular;
			mEndRotation = Quaternion.Euler(eularAngle);
		}
		
		float t = lerpSpeed * (Time.time - mStartTime);
		transform.localPosition = Vector3.Lerp(transform.localPosition, mEndPosition, t);
		transform.rotation = Quaternion.Lerp(transform.rotation, mEndRotation, t);
		
		if(transform.localPosition == mEndPosition && Mathf.FloorToInt(transform.rotation.eulerAngles.x) == Mathf.FloorToInt(mEndRotation.eulerAngles.x)) 
		{ ResetChangeCamVal(); }
	}
	
	void HandleMovePlayerPos()
	{
		if (mStartTimeMovePlayer == 0.0f) mStartTimeMovePlayer = Time.time;

		float currTime = Time.time;
		mPlayer.transform.position = Vector3.Lerp(mPlayer.transform.position, mEndPlayerPosition, lerpSpeed * (currTime - mStartTimeMovePlayer));

		if(mPlayer.transform.position == mEndPlayerPosition) 
		{ 
			ResetMoveCamVal(); 
			
			if(mPlayerController.playerMode == PlayerController.Mode.EXPLORE) 
			{
				mFOWRevealer.isEnabled = true;
				mPlayer.GetComponent<MeshRenderer>().enabled = true;
				DestroyObject(GameObject.FindGameObjectWithTag("Dummy").gameObject); 
			}
		}
	}
	
	void HandleZoomInArea()
	{
		if (mStartTime == 0.0f) mStartTime = Time.time;
		
		float currTime = Time.time;
		transform.position = Vector3.Lerp(transform.position, mEndPosition, zoomSpeed * (currTime - mStartTime));
		
		if(transform.position == mEndPosition) 
		{ ResetChangeCamVal(); }
	}
	
	void SetCameraRotation()
	{
		if(mCameraRotation == CameraRotation.NORMAL) { mLeft = KeyCode.E; mRight = KeyCode.Q; }
		else if(mCameraRotation == CameraRotation.REVERSE) { mLeft = KeyCode.Q; mRight = KeyCode.E; }
	}
	
	void ResetChangeCamVal()
	{
		mEndRotation = Quaternion.identity;
		mEndPosition = Vector3.zero;
		mStartTime = 0.0f;
		isChangingCam = false;
	}
	
	void ResetMoveCamVal()
	{
		mEndPlayerPosition = Vector3.zero;
		mStartTimeMovePlayer = 0.0f;
		isMovingPlayer = false;
	}
}
