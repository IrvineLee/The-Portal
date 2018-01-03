using UnityEngine;
using System.Collections;

public class InitialStartupAfterCamAnim : MonoBehaviour 
{
	PlayerScript mPlayerScript;
	CameraBattleScript mCameraBattleScript;
	HealthUI mHealthUI;
	
	void Start () 
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		mCameraBattleScript = player.GetComponent<CameraBattleScript>();
		mPlayerScript = player.GetComponent<PlayerScript>();
		mHealthUI = player.GetComponent<HealthUI>();
		
		mPlayerScript.LockMovement (true);
	}
	
	void Update () 
	{
		if (!animation["CamRunThrough"].enabled)
		{
			mPlayerScript.LockMovement (false);
			mCameraBattleScript.enabled = true;
			mHealthUI.enabled = true;
			enabled = false;
		}
		else if (animation["CamRunThrough"].enabled)
		{
			if(Input.GetKey (KeyCode.Escape)) animation["CamRunThrough"].enabled = false;
		}
	}
}
