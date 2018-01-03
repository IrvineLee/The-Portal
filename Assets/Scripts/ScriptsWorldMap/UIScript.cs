using UnityEngine;
using System.Collections.Generic;

public class UIScript : MonoBehaviour 
{
	public Texture2D TDialogName;
	public Texture2D TDialogArea;
	
	public enum UIType
	{
		NONE = 0,
		SHOP
	};
	UIType mUIType = UIType.NONE;
	
	enum MsgType
	{
		NONE = 0,
		DIALOG,
		TILE_MSG
	};
	MsgType mMsgType = MsgType.NONE;
	
	enum ButtonState
	{
		SLIDE_IN = 0,
		SLIDE_OUT
	};
	ButtonState mMsgState = ButtonState.SLIDE_IN;
	ButtonState mUIState = ButtonState.SLIDE_IN;
	
	// Handle Msg.
	public float mMsgSpd = 1.0f;
	float mMsgXPos = Screen.width, mIntendedWidth;
	string mInteractionMsg = "";
	bool mIsLabelShowing = false;
	
	// Handle Dialogs.
	float mX_Offset, mText_X_Offset;
	GUIStyle dialogNameStyle, dialogTextStyle;
	string mName = "";
	string mDialogMsg = "";
	string currSentence = "";
	bool mIsSetStyle = true;
	
	// Handle msg box appearing.
	Color mDefaultColor;
	float mTextureAlpha = 0.0f, mAppearSpeed = 3.0f, mHeightRatio, mHeightOffset = -1;
	bool mIsSetAlpha = true, mIsAnimateMsgBox = true;
	
	// Handle Replies.
	List<Response> mAnswerList = new List<Response>();
	int mAnswerIndex;
	bool mIsReply = false;
	bool mIsAnswered = false;
	
	// Handle ShopUI
	float mUI_XPos, mBtmButtonXPos, mButtonWidth, mShopButtonOffset;
	bool mIsShowButton = true;
	
	PlayerController mPlayerController;
	
	void Start()
	{
		mPlayerController = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>();
		
		// Msg.
		mIntendedWidth = 0.8f * Screen.width;
		
		// Dialogs.
		mX_Offset = 0.01f * Screen.width;
		mText_X_Offset = mX_Offset * 4;
		
		// ShopUI.
		mBtmButtonXPos = 0.300f * Screen.width;
		mButtonWidth = 0.2f * Screen.width;
		mShopButtonOffset = -mBtmButtonXPos + (-mButtonWidth);
		mUI_XPos = mShopButtonOffset;
		
		mDefaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}
	
	public bool IsLabelShowing
	{
		get { return mIsLabelShowing; }
	}
	
	public bool IsAwaitingReply
	{
		get { return mIsReply; }
	}
	
	public bool IsAnswered
	{
		get { return mIsAnswered; }
		set { mIsAnswered = value; }
	}
	
	public int GetAnswerIndex
	{
		get { return mAnswerIndex; }
	}
	
	public UIType GetUIType
	{
		get { return mUIType; }
	}
	
	public void ShowUI(UIType type)
	{
		mUIType = type;
		mUIState = ButtonState.SLIDE_IN;
	}
	
	public void UIButtonVisible(bool active)
	{
		mIsShowButton = active;
		/*if(type == mUIType)
		{
			if(active) mUIType = UIType.NONE;
			else if(!active) mUIType = type;
		}*/
	}
	
	public void DisableUI()
	{
		mUIState = ButtonState.SLIDE_OUT;
	}
	
	public void ShowDialog(string msg)
	{
		currSentence = "";
		mDialogMsg = msg;
		mMsgType = MsgType.DIALOG;
		ResetMsgState();
	}
	
	public void ShowDialog(Dialog talk)
	{
		if(mName == talk.name) mIsAnimateMsgBox = false;
		else mIsAnimateMsgBox = true;
		
		currSentence = "";
		mName = talk.name;
		mDialogMsg = talk.dialog;
		mMsgType = MsgType.DIALOG;
		ResetMsgState();
	}
	
	public string NextDialog(Dialog talk)
	{
		if(talk == null) return null;
		
		if(mName == talk.name) mIsAnimateMsgBox = false;
		else mIsAnimateMsgBox = true;
		mName = talk.name;
		currSentence = "";
		mDialogMsg = talk.dialog;
		return mDialogMsg;
	}
	
	public void ShowDialogInteraction(ReplyChoice reply)
	{
		if(mName == reply.name) mIsAnimateMsgBox = false;
		else mIsAnimateMsgBox = true;
		
		mIsReply = true;
		mDialogMsg = reply.question;
		mAnswerList = reply.answerList;
	}
	
	public void ShowFullText()
	{
		currSentence = mDialogMsg;
	}
	
	public void DisableDialog()
	{
		if(mIsLabelShowing) mMsgType = MsgType.TILE_MSG;
		else ResetMsgStateAndType();
		
		mName = "";
		mIsAnimateMsgBox = true;
	}
	
	public void ShowInteraction(string msg)
	{
		mInteractionMsg = msg;
		mMsgType = MsgType.TILE_MSG;
		mMsgState = ButtonState.SLIDE_IN;
	}
	
	public void DisableInteraction()
	{
		mMsgState = ButtonState.SLIDE_OUT;
	}
	
	void OnGUI () 
	{
		if(mMsgType == MsgType.DIALOG)
		{
			if(mIsSetStyle)
			{
				dialogNameStyle = new GUIStyle(GUI.skin.label);
				dialogNameStyle.fontStyle = FontStyle.Normal;
				dialogNameStyle.fontSize = 20;
				dialogNameStyle.alignment = TextAnchor.MiddleCenter;
				
				dialogTextStyle = new GUIStyle(GUI.skin.label);
				dialogTextStyle.fontStyle = FontStyle.Normal;
				dialogTextStyle.fontSize = 18;
				dialogTextStyle.wordWrap = true;
				dialogTextStyle.alignment = TextAnchor.UpperLeft;
				
				mIsSetStyle = false;
			}
			
			// Ensure it only register once for every dialog.
			if(!TypeWriterStyle.IsTexting && currSentence != mDialogMsg) 
			{
				TypeWriterStyle.Register(mDialogMsg);
				ResetUIMovementAndAlpha();
			}
			
			if(currSentence != mDialogMsg) currSentence = TypeWriterStyle.GetCurrWord;
			else if(currSentence == mDialogMsg) TypeWriterStyle.IsTexting = false; // Stops the coroutine.
			
			if(mIsAnimateMsgBox)
			{
				if(mIsSetAlpha) Alpha01();
				if(mHeightOffset != 0) mHeightOffset = HeightOffsetMovement(0.76f);
			}
			else if(!mIsAnimateMsgBox) mHeightOffset = 0.0f;
			
			// Display dialog texture.
			GUI.Label(new Rect(mX_Offset, 0.7f * Screen.height, 0.3f * Screen.width, 0.06f * Screen.height), TDialogName);
			GUI.DrawTexture(new Rect(mX_Offset, 0.76f * Screen.height + mHeightOffset, Screen.width - (mX_Offset * 2), 0.22f * Screen.height), TDialogArea);
			
			// Display name.
			GUI.Label(new Rect(mX_Offset, 0.7f * Screen.height, 0.3f * Screen.width, 0.06f * Screen.height), mName, dialogNameStyle);
			GUI.color = mDefaultColor;
			
			// Display dialog text.
			GUI.Label(new Rect(mText_X_Offset, 0.78f * Screen.height, Screen.width - (mText_X_Offset * 2), 0.18f * Screen.height), currSentence, dialogTextStyle);
			
			if(mIsReply && !TypeWriterStyle.IsTexting) 
			{
				for(int i = 0; i < mAnswerList.Count; i++)
				{
					if(GUI.Button(new Rect(mText_X_Offset + 0.5f, (0.82f * Screen.height) + (i * 0.05f * Screen.height), 0.2f * Screen.width, 0.05f * Screen.height), mAnswerList[i].reply))
					{ 
						mIsReply = false;
						mIsAnswered = true;
						mAnswerIndex = i;
					}
				}
			}
		}
		else if(mMsgType == MsgType.TILE_MSG)
		{
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;
			
			if(!mIsLabelShowing) mIsLabelShowing = true;
			
			if(mMsgState == ButtonState.SLIDE_IN)
			{
				if(mMsgXPos > mIntendedWidth) 
				{
					float temp = mMsgXPos - (Time.deltaTime * Screen.width) * mMsgSpd;
					
					if(temp > mIntendedWidth) mMsgXPos = temp;
					else if(temp < mIntendedWidth) mMsgXPos -= mMsgXPos - mIntendedWidth;
				}
				//else if(mMsgXPos < mIntendedWidth) mMsgXPos = mIntendedWidth;
			}
			else if(mMsgState == ButtonState.SLIDE_OUT)
			{
				if(mMsgXPos < Screen.width) mMsgXPos += (Time.deltaTime * Screen.width) * mMsgSpd;
				else if(mMsgXPos >= Screen.width) ResetMsgStateAndType();
			}
			GUI.skin.box.fontSize = 15;
			GUI.Box (new Rect(mMsgXPos, 0.85f * Screen.height, 0.22f * Screen.width, 0.075f * Screen.height), mInteractionMsg);
		}
		
		if(mUIType == UIType.NONE) return;
		else if(mUIType == UIType.SHOP)
		{
			if(mUIState == ButtonState.SLIDE_IN)
			{
				if(mUI_XPos < 0) 
				{
					float temp = mUI_XPos + (Time.deltaTime * Screen.width) * mMsgSpd;
					
					if(temp < 0) mUI_XPos = temp;
					else if(temp > 0) mUI_XPos += 0 - mUI_XPos;
				}
				//else if(mUI_XPos > 0) mUI_XPos = 0;
			}
			else if(mUIState == ButtonState.SLIDE_OUT)
			{
				if(mUI_XPos > mShopButtonOffset) mUI_XPos -= (Time.deltaTime * Screen.width) * mMsgSpd;
				else if(mUI_XPos < mShopButtonOffset) ResetUIState();
			}
			
			GUI.skin.box.fontSize = 20;
			GUI.Box(new Rect(0.35f * Screen.width, 0.08f * Screen.height, 0.3f * Screen.width, 0.075f * Screen.height), "SHOP");
			
			if(mIsShowButton)
			{
				GUI.skin.button.fontSize = 15;
				GUI.skin.button.alignment = TextAnchor.MiddleCenter;
				if(GUI.Button(new Rect((0.100f * Screen.width) + mUI_XPos, 0.25f * Screen.height, mButtonWidth, 0.075f * Screen.height), "TALK")) mPlayerController.Shop = PlayerController.ShopMode.TALK;
				else if(GUI.Button(new Rect((0.150f * Screen.width) + mUI_XPos, 0.35f * Screen.height, mButtonWidth, 0.075f * Screen.height), "SYNTHESIS")) mPlayerController.Shop = PlayerController.ShopMode.COMBINE;
				else if(GUI.Button(new Rect((0.200f * Screen.width) + mUI_XPos, 0.45f * Screen.height, mButtonWidth, 0.075f * Screen.height), "SOUL BLESS")) mPlayerController.Shop = PlayerController.ShopMode.BUFF;
				else if(GUI.Button(new Rect((0.250f * Screen.width) + mUI_XPos, 0.55f * Screen.height, mButtonWidth, 0.075f * Screen.height), "Minigame!")) mPlayerController.Shop = PlayerController.ShopMode.MINIGAME;
				else if(GUI.Button(new Rect(mBtmButtonXPos + mUI_XPos, 0.65f * Screen.height, mButtonWidth, 0.075f * Screen.height), "Exit")) mPlayerController.Shop = PlayerController.ShopMode.EXIT;
			}
		}
	}
	
	void Alpha01()
	{
		Color color = GUI.color;
		mTextureAlpha += 0.01f * mAppearSpeed;
		color.a = mTextureAlpha;
		GUI.color = color;
		
		if(color.a >= 1.0f) mIsSetAlpha = false;
	}
	
	float HeightOffsetMovement(float yRatio)
	{
		float offsetRatio = 1.0f - yRatio;
		if(Mathf.Approximately(offsetRatio, mHeightRatio)) return 0.0f;
		
		mHeightRatio += 0.01f * 1.5f;
		if(mHeightRatio > offsetRatio) mHeightRatio = offsetRatio;
		
		return ((offsetRatio - mHeightRatio) * Screen.height) * 0.5f;
	}
	
	void ResetMsgState()
	{
		mMsgXPos = Screen.width;
		mIsLabelShowing = false;
		mMsgState = ButtonState.SLIDE_IN;
		mIsSetStyle = true;
	}
	
	void ResetMsgStateAndType()
	{
		ResetMsgState();
		mMsgType = MsgType.NONE;
	}
	
	void ResetUIState()
	{
		mUI_XPos = mShopButtonOffset;
		mUIType = UIType.NONE;
		mUIState = ButtonState.SLIDE_IN;
	}
	
	void ResetUIMovementAndAlpha()
	{
		mHeightOffset = -1;
		mHeightRatio = 0.0f;
		mTextureAlpha = 0.0f;
		mIsSetAlpha = true;
	}
}
