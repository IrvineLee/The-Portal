using UnityEngine;
using System.Collections;

public class SceneManagerScript : MonoBehaviour
{
	public GameObject mPhase1Objective, mPhase2Objective;
	public GameObject mPhase1Effect, mPhase2Effect;
	public bool mGameWin = false, mGameLose = false, mObjectiveAchieved = false;
	
	public enum ScenePhase
	{
		SP_Phase1 = 1,
		SP_Phase2,
		SP_Phase3
	};
	
	public ScenePhase mCurrentPhase;
	
	// Use this for initialization
	void Start () 
	{
		mCurrentPhase = ScenePhase.SP_Phase1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mCurrentPhase == ScenePhase.SP_Phase1)
		{
			if(!mPhase1Objective.activeSelf)
			{
//				mSceneEffect.SetStage((int)ScenePhase.SP_Phase1);
				mPhase1Effect.SetActive(true);
				mCurrentPhase++;
			}
		}
		if(mCurrentPhase == ScenePhase.SP_Phase2)
		{
			if(!mPhase2Objective.activeSelf)
			{
//				mSceneEffect.SetStage((int)ScenePhase.SP_Phase1);
				mPhase2Effect.SetActive(true);
				mCurrentPhase++;
			}
		}
		if(mObjectiveAchieved && !mGameLose)
		{
			mGameWin = true;
		}
	}
	
	void OnGUI()
	{
		if(mGameWin)
			GUI.Label(new Rect((Screen.width * 0.5f) - 50.0f,(Screen.height * 0.5f) - 10.0f, 100.0f, 20.0f),"YOU WIN!!");
		if(mGameLose && !mObjectiveAchieved)
			GUI.Label(new Rect((Screen.width * 0.5f) - 50.0f,(Screen.height * 0.5f) - 10.0f, 100.0f, 20.0f),"GAME OVER!!");
	}
}
