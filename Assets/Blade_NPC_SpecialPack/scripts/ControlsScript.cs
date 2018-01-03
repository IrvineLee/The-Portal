using UnityEngine;
using System.Collections.Generic;

public class ControlsScript : MonoBehaviour 
{
	public GameObject playerGO;
	
	List<char> keyPressedList = new List<char>();
	
	Quaternion mEndRotation;
	float mStartTime;
	float mLerpSpd = 6.0f;
	bool mIsReset = false;
	
	void Start () 
	{
		playerGO.animation.wrapMode = WrapMode.Loop;
	}
	
	void Update () 
	{
		HandleInput();
		if(keyPressedList.Count == 2) Handle2Input();
		//if (!Input.anyKey) ResetToDefault();
	}
	
	void HandleInput()
	{
		if(Input.GetKey(KeyCode.W)) 
		{
			if(Input.GetKeyDown(KeyCode.W)) 
			{
				keyPressedList.Add ('W');
				playerGO.animation["BW_L_Run00"].speed = 1.0f;
			}
			if(keyPressedList.Count < 2)
			{
				MovePlayer(playerGO, 'z', 0.05f, "BW_Run00");
				if(playerGO.transform.eulerAngles.y != 0.0f) 
				{
					if(mIsReset) 
					{
						mStartTime = 0.0f;
						mIsReset = false;
					}
					ChangeAngle(playerGO, 0.0f);
				}
			}
		}
		else keyPressedList.Remove('W');

		if(Input.GetKey(KeyCode.A)) 
		{
			if(Input.GetKeyDown(KeyCode.A)) 
			{
				keyPressedList.Add ('A');
				playerGO.animation["BW_L_Run00"].speed = 1.0f;
			}
			if(keyPressedList.Count < 2)
			{
				MovePlayer(playerGO, 'x', -0.05f, "BW_L_Run00");
				if(playerGO.transform.eulerAngles.y != -45.0f) 
				{
					if(mIsReset) 
					{
						mStartTime = 0.0f;
						mIsReset = false;
					}
					ChangeAngle(playerGO, -45.0f);
				}
			}
		}
		else keyPressedList.Remove('A');

		if(Input.GetKey(KeyCode.S)) 
		{
			if(Input.GetKeyDown(KeyCode.S)) 
			{
				keyPressedList.Add ('S');
				playerGO.animation["BW_B_Run00"].speed = 1.0f;
			}
			if(keyPressedList.Count < 2)
			{
				MovePlayer(playerGO, 'z', -0.05f, "BW_B_Run00");
				if(playerGO.transform.eulerAngles.y != 0.0f) 
				{
					if(mIsReset) 
					{
						mStartTime = 0.0f;
						mIsReset = false;
					}
					ChangeAngle(playerGO, 0.0f);
				}
			}
		}
		else keyPressedList.Remove('S');

		if(Input.GetKey(KeyCode.D)) 
		{
			if(Input.GetKeyDown(KeyCode.D)) 
			{
				keyPressedList.Add ('D');
				playerGO.animation["BW_R_Run00"].speed = 1.0f;
			}
			if(keyPressedList.Count < 2)
			{
				MovePlayer(playerGO, 'x', 0.05f, "BW_R_Run00");
				if(playerGO.transform.eulerAngles.y != 45.0f) 
				{
					if(mIsReset) 
					{
						mStartTime = 0.0f;
						mIsReset = false;
					}
					ChangeAngle(playerGO, 45.0f);
				}
			}
		}
		else keyPressedList.Remove('D');
		
		if(Input.GetKeyDown(KeyCode.Space)) 
		{
			playerGO.animation.CrossFade("BW_Jump00");
		}
	}
	
	void Handle2Input()
	{
		ChangeAngle(playerGO, 0.0f);
		if(IsKeyPresses('W', 'A'))
		{
			MovePlayer(playerGO, -0.05f, 0.05f, "BW_L_Run00");
		}
		else if(IsKeyPresses('W', 'D'))
		{
			MovePlayer(playerGO, 0.05f, 0.05f, "BW_R_Run00");
		}
		else if(IsKeyPresses('S', 'A'))
		{
			playerGO.animation["BW_R_Run00"].speed = -1.0f;
			MovePlayer(playerGO, -0.05f, -0.05f, "BW_R_Run00");
		}
		else if(IsKeyPresses('S', 'D'))
		{
			playerGO.animation["BW_L_Run00"].speed = -1.0f;
			MovePlayer(playerGO, 0.05f, -0.05f, "BW_L_Run00");
		}
		else if(IsKeyPresses('W', 'S') || IsKeyPresses('A', 'D'))
		{
			ResetToDefault();
		}
	}
	
	void MovePlayer(GameObject go, char axis, float amount, string animName)
	{
		Vector3 pos;
		
		pos = go.transform.position;
		
		//if(axis == 'x') pos.x += amount;
		//else if(axis == 'z') pos.z += amount;
		
		go.transform.position = pos;
		go.animation.CrossFade(animName);
	}
	
	void MovePlayer(GameObject go, float x, float z, string animName)
	{
		Vector3 pos;
		
		pos = go.transform.position;
		pos.x += x;
		pos.z += z;
		
		go.transform.position = pos;
		go.animation.CrossFade(animName);
	}
	
	void ChangeAngle(GameObject go, float angle)
	{
		if(mStartTime == 0.0f) 
		{
			mStartTime = Time.time;
			
			Vector3 eularAngle = transform.rotation.eulerAngles;
			eularAngle.x = 0.0f;
			eularAngle.y = angle;
			mEndRotation = Quaternion.Euler(eularAngle);
		}
		
		float t = (Time.time - mStartTime) * mLerpSpd;
		go.transform.rotation = Quaternion.Lerp(go.transform.rotation, mEndRotation, t);
		
		if(Mathf.FloorToInt(go.transform.rotation.eulerAngles.y) == Mathf.FloorToInt(mEndRotation.eulerAngles.y)) 
		{ mIsReset = true;  }
	}
	
	bool IsKeyPresses(char first, char second)
	{
		if((keyPressedList[0] == first || keyPressedList[0] == second) 
			&& (keyPressedList[1] == first || keyPressedList[1] == second))
		{ return true; }
		else return false;
	}
	
	void ResetToDefault()
	{
		if(playerGO.transform.eulerAngles.y != 0) 
		{
			if(mIsReset) 
			{
				mStartTime = 0.0f;
				mIsReset = false;
			}
			ChangeAngle(playerGO, 0.0f);
		}
		playerGO.animation.CrossFade("BW_Idle");
		
	}
}
