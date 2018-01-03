using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class StartMenuScript : MonoBehaviour 
{
	// Fade time.
	public float FadeOutTime = 1.0f;
	public float FadeInTime = 1.0f;
	
	public float posXRatio, posYRatio, widthRatio, heightRatio, offsetRatio;
	public bool isXCenter = false, isYCenter = false;
	public string[] buttonStringArray = new string[] {"New Game", "Load", "Exit"};
	
	public enum InputMode
	{
		MOUSE = 0,
		KEYBOARD
	};
	public InputMode mInputMode = InputMode.MOUSE;
	
	int mSelGridInt = 0, mMaxButton;
	float mPosX, mPosY, mWidth = 0.2f, mHeight = 0.075f, mOffsetHeight;
	
	SceneManager mSceneManager;
	
	void Start () 
	{
		ButtonPositioning();
		mMaxButton = buttonStringArray.Length;
		mSceneManager = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager>();
	}
	
	void Update () 
	{
		if(!Application.isPlaying) ButtonPositioning();
		
		if(mInputMode == InputMode.KEYBOARD)
		{
			// Get keyboard input and increase or decrease our grid integer
			if(Input.GetKeyDown(KeyCode.W))
			{
				if(mSelGridInt <= 0) mSelGridInt = mMaxButton - 1;
				else if(mSelGridInt > 0) mSelGridInt--;
			}
			else if(Input.GetKeyDown(KeyCode.S))
			{
				if(mSelGridInt < (mMaxButton-1)) mSelGridInt++;
				else if(mSelGridInt > (mMaxButton-1)) mSelGridInt = 0;
			}
		}
	}
	
	void OnGUI()
	{
		if(mInputMode == InputMode.MOUSE)
		{
			if(GUI.Button(new Rect(mPosX, mPosY, mWidth, mHeight), buttonStringArray[0]))
			{ NewGame(); }
			else if(GUI.Button(new Rect(mPosX, mPosY + mOffsetHeight, mWidth, mHeight), buttonStringArray[1]))
			{ LoadGame(); }
			else if(GUI.Button(new Rect(mPosX, (mPosY + mOffsetHeight * 2), mWidth, mHeight), buttonStringArray[2]))
			{ QuitGame(); }
		}
		else if(mInputMode == InputMode.KEYBOARD)
		{
			GUIStyle myStyle = new GUIStyle (GUI.skin.button);
			myStyle.margin = new RectOffset(0,0,0,13);
        	mSelGridInt = GUI.SelectionGrid(new Rect(mPosX, mPosY, mWidth, mHeight + mOffsetHeight * 2), mSelGridInt, buttonStringArray, 1, myStyle);
			
			if(Input.GetMouseButtonDown(0))
			{
				switch(mSelGridInt)
				{
					case 0: NewGame();
					break;
					case 1: LoadGame();
					break;
					case 2: QuitGame();
					break;
				}
			}
		}
	}
	
	void NewGame()
	{
		AutoFade.LoadLevel (mSceneManager.worldMapScene, FadeOutTime, FadeInTime, Color.black);
	}
	
	void LoadGame()
	{
		if(PlayerPrefs.HasKey("SaveSlot1"))
		{
			// Save Manager will load the game upon entering the world map scene.
			SaveManager.isLoad = true; 
			AutoFade.LoadLevel (mSceneManager.worldMapScene, FadeOutTime, FadeInTime, Color.black);
			Debug.Log ("Loading..");
		}
		else Debug.Log ("No save file.");	
	}
	
	void QuitGame()
	{
		Application.Quit();
		Debug.Log ("QUIT");
	}
	
	void ButtonPositioning()
	{
		mWidth = widthRatio * Screen.width;
		mHeight = heightRatio * Screen.height;
		
		if(isXCenter) 
		{
			mPosX = Screen.width/2 - mWidth/2;
			posXRatio = mPosX / Screen.width;
		}
		else if(!isXCenter) mPosX = posXRatio * Screen.height;
		
		if(isYCenter) 
		{
			mPosY = Screen.height/2 - mHeight/2;
			posYRatio = mPosY / Screen.height;
		}
		else if(!isYCenter) mPosY = posYRatio * Screen.height;
		
		mOffsetHeight = offsetRatio * Screen.height;
	}
	
	public void ClearSave()
	{
		PlayerPrefs.DeleteAll();
		Debug.Log ("All save files have been deleted.");
	}
}
