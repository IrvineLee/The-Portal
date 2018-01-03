using UnityEngine;
using System.Collections;

public class Scenario2Script : MonoBehaviour 
{
	// Enemy target.
	public GameObject Floor2Enemy;
	public GameObject Floor2SleepEnemy;
	public GameObject TargetGO;
	
	// Roof that spawns obj.
	public GameObject RoofGO;
	
	// Teleporter to disable.
	public GameObject TeleporterGO;
	
	// Activate emergency light when running starts.
	public GameObject GlowGO;
	
	// Change camera bg color to black.
	public Camera mCamera;
    public Color ToColor = Color.black;
    public float Duration = 3.0F;
	
	// Change light intensity
	public Light mLight;
	public float Intensity = 0.3F;
	
	Color mDefaultCamColor;
	float mDefaultLightIntensity;
	float mLerpTimer;
	bool mIsChangeSkyColor = false;
	bool mIsTriggered = false;
	
	GameObject mPlayerGO;
	GameObject[] mPLGOArray = new GameObject[5];
	
	ObjectDropScript mObjectDropScript;
	UIScript mUIScript;
	
	void Start()
	{
		mPlayerGO = GameObject.FindGameObjectWithTag ("Player").gameObject;
		mPLGOArray = GameObject.FindGameObjectsWithTag ("Enemy2");
		
		mDefaultLightIntensity = mLight.intensity;
		mDefaultCamColor = mCamera.backgroundColor;
	}
	
	void Update() 
	{
		if(!mIsChangeSkyColor) return;
		
		mLerpTimer += Time.deltaTime;
		float t = mLerpTimer / Duration;
        mCamera.backgroundColor = Color.Lerp(mDefaultCamColor, ToColor, t);
		mLight.intensity = Mathf.Lerp(mDefaultLightIntensity, Intensity, t);
		
		if(t >= 1.0f) mIsChangeSkyColor = false;
    }
	
   void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag != "Player" || mIsTriggered) return;
		
		mPlayerGO.GetComponent<AnimScript>().SetToCollectAnim(true);
		
		mUIScript = mPlayerGO.GetComponent<UIScript>();
		mUIScript.ShowInteraction("Take the key.");
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag != "Player" || !Input.GetMouseButtonUp (0) || mIsTriggered) return;
		
		transform.GetComponent<Timer>().ActivateTimer();
		GetComponent<TriggerFall>().Activate(false);
		RoofGO.GetComponent<ObjectDropScript>().enabled = true;
		TeleporterGO.GetComponent<Teleporter>().enabled = false;
		
		GlowGO.SetActive (true);
//		foreach(Transform child in Floor2Enemy)
//		{ child.GetComponent<EnemyScript>().mTarget = TargetGO2; }
		
//		foreach(EnemyScript grunts in EnemyScript.EnemyList)
//		{ grunts.mTarget = TargetGO; }
		
		foreach(Transform child in Floor2Enemy.transform)
		{ child.GetComponent<EnemyScript>().mTarget = TargetGO; }
		
		foreach(Transform child in Floor2SleepEnemy.transform)
		{ 
			child.GetComponent<EnemySleep>().enabled = true; 
			child.GetComponent<EnemyScript>().mTarget = TargetGO;
		}
		
		foreach (GameObject pl in mPLGOArray)
		{ pl.GetComponent<SquadLeaderScript>().HoldPositionWithGrunts(); }
		
		mIsChangeSkyColor = true;
		mIsTriggered = true;
		mUIScript.DisableInteraction();
		
		mPlayerGO.GetComponent<PlayerScript>().LockMovement (true);
		mPlayerGO.GetComponent<AnimScript>().LockAnim (true);
		mPlayerGO.GetComponent<CameraBattleScript>().enabled = false;
		GetComponent<ShakeCamera>().Activate (3.0f);
		GetComponent<TriggerEnabler>().SetActive (true);
		GetComponent<AudioScript>().enabled = true;
		GetComponent<AudioScript>().PlayBGM();
		
		EnableGameObj[] eneGO = GetComponents<EnableGameObj>();
		foreach(EnableGameObj enable in eneGO)
		{
			enable.ToggleGO(false);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag != "Player") return;
		
		mPlayerGO.GetComponent<AnimScript>().SetToCollectAnim(false);
		mUIScript.DisableInteraction();
	}
	
	public void EnableCam()
	{
		mPlayerGO.GetComponent<CameraBattleScript>().enabled = true;
	}
}
