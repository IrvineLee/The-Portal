using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour 
{
	public static bool isLoad = false;
	public float stayTime = 1.0f;
	public float fadeSpd = 0.35f;
	
	GameObject mCurrFloor;
	
	// Handle 'Saved' label popping after saving.
	Rect mSaveRect;
	GUIStyle myStyle;
	Color mColor;
	float mStayTimer;
	bool mIsSetStyle = true, mIsShowSave = false;
	
	PlayerController mPlayerController;
	
	void Awake () 
	{
		DontDestroyOnLoad (transform.gameObject);
		
		mCurrFloor = GameObject.FindGameObjectWithTag ("Floor1");
		mPlayerController = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>();
		
		mColor = Color.white;
		float width = 0.2f * Screen.width;
		float height = 0.075f * Screen.height;
		mSaveRect = new Rect(Screen.width/2 - width/2, 0.2f * Screen.height, width, height);
		
		if(isLoad) LoadGame();
	}
	
	void OnGUI()
	{
		if(!mIsShowSave) return;
		
		if(mIsSetStyle)
		{
			myStyle = new GUIStyle(GUI.skin.label);
			myStyle.fontSize = 20;
			myStyle.alignment = TextAnchor.MiddleCenter;
			mIsSetStyle = false;
		}
		
		mStayTimer += Time.deltaTime;
		if(mStayTimer > stayTime)
		{
			mColor.a -= fadeSpd * Time.deltaTime;
			GUI.color = mColor;
			
			if(mColor.a <= 0) 
			{
				mColor.a = 1.0f;
				mStayTimer = 0.0f;
				mIsSetStyle = true;
				mIsShowSave = false;
			}
		}
		GUI.Label(mSaveRect, "Saved!", myStyle);
	}
	
	public void SaveGame()
	{
		PlayerPrefs.SetInt ("SaveSlot1", 0);
		mPlayerController.Save();
		
		mCurrFloor = mPlayerController.CurrFloorGO;
		foreach(Transform child in mCurrFloor.transform)
		{ if(child.name == "Hex") child.GetComponent<HexScript>().Save(); }
		
		mIsShowSave = true;
		Debug.Log ("Saved!!");
	}
	
	public void LoadGame()
	{
		mPlayerController.Load();
		
		mCurrFloor = mPlayerController.CurrFloorGO;
		foreach(Transform child in mCurrFloor.transform)
		{ if(child.name == "Hex") child.GetComponent<HexScript>().Load(); }
		isLoad = false;
		Debug.Log ("Loaded!!");
	}
	
	public static string UniqueName(GameObject go, string variableName)
	{
		string nameGO = go.name;
		float x = go.transform.position.x;
		float y = go.transform.position.y;
		float z = go.transform.position.z;
		
		string uniqueName = nameGO + "-" + variableName + "-" + x + "-" + y + "-" + z;
		return uniqueName;
	}
}
