using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour 
{
	// Fade time.
	public float fadeOutTime = 1.0f;
	public float fadeInTime = 1.0f;
	
	// Battle scene.
	public string worldMapScene;
	public string battleScene;
	
	GameObject mCurrSceneGO;
	string mDisabledSceneTag;
	string mLoadedScene;
	bool mIsDisableScene = false;
	bool mIsRestoreScene = false;
	
	GameObject playerGO;
	GameObject cameraGO;
	
	PlayerController mPlayerController;
	CameraScript mCameraScript;
	
	void Start()
	{
		DontDestroyOnLoad(transform.gameObject);
		
		playerGO = GameObject.FindGameObjectWithTag ("Player");
		if(playerGO != null) mPlayerController = playerGO.GetComponent<PlayerController>();
	}
	
	void Update()
	{
		if(!mIsDisableScene && !mIsRestoreScene) return;
		
		if(AutoFade.IsFadeOutSolid)
		{
			if(mIsDisableScene) 
			{
				mCameraScript.RestoreZoomCam();
				DisableScene(mDisabledSceneTag);
			}
			else if(mIsRestoreScene) 
			{
				RestoreScene();
				mPlayerController.isUpdate = true;
			}
		}
	}
	
	public void ChangeScene(string disableSceneTag, string loadScene)
	{
		if(playerGO == null || cameraGO == null) Cache();

		mIsDisableScene = true;
		AutoFade.LoadLevel(loadScene, fadeOutTime, fadeInTime, Color.black, true);
		mDisabledSceneTag = disableSceneTag;
		mLoadedScene = loadScene;
	}
	
	public void RestorePreviousScene()
	{
		mIsRestoreScene = true;
		AutoFade.LoadLevel("", fadeOutTime, fadeInTime, Color.black);
	}
	
	// Used for moving to next floor.
	public void FadeScreen()
	{
		AutoFade.LoadLevel("", fadeOutTime, fadeInTime, Color.black);
	}
	
	void Cache()
	{
		mPlayerController = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>();
		cameraGO = GameObject.FindGameObjectWithTag ("MainCamera");
		mCameraScript = cameraGO.GetComponent<CameraScript>();
		Debug.Log ("CACHED");
	}
	
	void DisableScene(string disableSceneTag)
	{
		mCurrSceneGO = GameObject.FindGameObjectWithTag (disableSceneTag).gameObject;
		
		mCurrSceneGO.SetActive(false);
		mIsDisableScene = false; 
	}
	
	void RestoreScene()
	{
		GameObject battleScene = GameObject.FindGameObjectWithTag (mLoadedScene).gameObject;
		DestroyObject (battleScene);
		mCurrSceneGO.SetActive(true);
		mIsRestoreScene = false;
	}
}
