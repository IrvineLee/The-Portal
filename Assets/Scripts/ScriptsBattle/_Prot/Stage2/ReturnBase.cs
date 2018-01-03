using UnityEngine;
using System.Collections;

public class ReturnBase : MonoBehaviour 
{
	public enum Scene
	{
		NONE = 0,
		SCENARIO_COMPLETE,
		GAME_OVER
	};
	public Scene mScene = Scene.NONE;
	
	public float mWinMsgTime = 2.0f;
	
	float mWinMsgTimer;
	bool mIsShowMsg = false;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag != "Player") return;
		
		if(mScene == Scene.SCENARIO_COMPLETE) 
		{
			// For stage 2(Against the clock)
//			if(Timer.IsCountDown)
//			{
				Timer.IsCountDown = false;
				mIsShowMsg = true;
				mWinMsgTimer = mWinMsgTime;
				
				GetComponent<AudioScript>().enabled = true;
				GetComponent<AudioScript>().PlayBGM ();
//			}
		}
		else if(mScene == Scene.GAME_OVER) 
		{
			mIsShowMsg = true;
			mWinMsgTimer = mWinMsgTime;
		}
	}
	
	void Update()
	{
		if(!mIsShowMsg) return;
		
		mWinMsgTimer -= Time.deltaTime;
		if(mWinMsgTimer < 0.0f) 
		{
			mIsShowMsg = false;
			
			if(mScene == Scene.SCENARIO_COMPLETE) 
			{
				// Change level to base.
				AutoFade.LoadLevel ("WorldMap", 1.0f, 1.0f, Color.black);
			}
			else if(mScene == Scene.GAME_OVER) 
			{
				AutoFade.LoadLevel (Application.loadedLevelName, 1.0f, 1.0f, Color.black);
			}
		}
	}
	
	void OnGUI()
	{
		if(!mIsShowMsg) return;
		
		string str = "";
		float width = 0.5f;
		float height = 0.15f;
		
		GUI.skin.box.fontSize = 40;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		
		if(mScene == Scene.SCENARIO_COMPLETE) str = "Scenario Complete!!";
		else if(mScene == Scene.GAME_OVER) str = "Game Over..";
		
		GUI.Box (new Rect(Screen.width * (width - (width / 2.0f)), Screen.height * ((1 - height) / 2), Screen.width * width, Screen.height * height), str);
		GUI.skin.box.fontSize = 12;
	}
}
