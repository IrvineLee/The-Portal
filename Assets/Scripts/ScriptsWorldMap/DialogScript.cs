using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Dialog
{
	public string name;
	public string dialog;
	
	public Dialog(string name, string dialog)
	{
		this.name = name;
		this.dialog = dialog;
	}
};

[System.Serializable]
public class ReplyChoice
{
	public string name;
	public string question;
	public List<Response> answerList;
	
	public ReplyChoice(string name, string question, List<Response> answerList)
	{
		this.name = name;
		this.question = question;
		this.answerList = answerList;
	}
};

[System.Serializable]
public class Response
{
	public string reply;
	public List<string> responseList;
	
	public enum AnswerEffect
	{
		NONE = 0,
		TILE,
		BATTLE,
		ITEM
	};
	public AnswerEffect effect = AnswerEffect.TILE;
	
	public Response(string reply, List<string> responseList, AnswerEffect effect)
	{
		this.reply = reply;
		this.responseList = responseList;
		this.effect = effect;
	}
};

[ExecuteInEditMode]
public class DialogScript : MonoBehaviour 
{
	[SerializeField] public List<TalkType> fullTalkList = new List<TalkType>();
	[SerializeField] public List<Dialog> dialogList = new List<Dialog>();
	[SerializeField] public List<ReplyChoice> interactionList = new List<ReplyChoice>();
	
	public enum TalkScene
	{
		NORMAL = 0,
		SHOP
	};
	public TalkScene mTalkScene = TalkScene.NORMAL;
	
	public enum TalkType
	{
		DIALOG = 0,
		INTERACTION
	};
	
	int mCurrIndex = 0, mCurrStringIndex = -1, mCurrInteractionIndex = -1, mResIndex = -1;
	int mAnswerIndex = -1, mResponseCount = -1;
	bool mIsDialogFirst = false;
	
	void Start()
	{
		if(fullTalkList.Count != 0)
		{
			if(fullTalkList[0] == TalkType.DIALOG) mIsDialogFirst = true;
			else mIsDialogFirst = false;
		}
	}
	
	public Dialog GetDialog
	{
		get 
		{
			if(fullTalkList[mCurrIndex] == TalkType.DIALOG)
			{ 
				mCurrIndex += 1;
				mCurrStringIndex += 1;
				return dialogList[mCurrStringIndex]; 
			}
			return null;
		}
	}
	
	public ReplyChoice GetInteraction
	{
		get 
		{
			if(fullTalkList[mCurrIndex] == TalkType.INTERACTION)
			{  
				mCurrIndex += 1;
				mCurrInteractionIndex += 1;
				return interactionList[mCurrInteractionIndex]; 
			}
			return null;
		}
	}
	
	public void RegisterAnsweredIndex(int index)
	{
		mAnswerIndex = index;
		mResponseCount = interactionList[mCurrInteractionIndex].answerList[index].responseList.Count;
	}
	
	public string GetResponse()
	{
		if(IsLastResponse) return null;

		mResIndex += 1;
		return interactionList[mCurrInteractionIndex].answerList[mAnswerIndex].responseList[mResIndex];
	}
	
	public Response.AnswerEffect GetAnswerEffect
	{
		get 
		{
			return interactionList[mCurrInteractionIndex].answerList[mAnswerIndex].effect;
		}
	}
	
	public bool IsDialogFirst
	{
		get { return mIsDialogFirst; }
	}
	
	public bool IsTalkEnd
	{
		get
		{
			if(mCurrStringIndex >= dialogList.Count - 1 && mCurrInteractionIndex >= interactionList.Count - 1)
			{ 
				return true; 
			}
			return false;
		}
	}
	
	public bool IsLastResponse
	{
		get 
		{	
			// Return false if haven't register answered index.
			if(mAnswerIndex == -1 && mResponseCount == -1) return false;
			else if(mResIndex >= mResponseCount - 1) return true;
			return false;
		}
	}
	
	public void ResetIndexCount()
	{
		mCurrIndex = 0;
		mCurrStringIndex = -1;
		mCurrInteractionIndex = -1;
		mResIndex = -1;
		mAnswerIndex = -1;
		mResponseCount = -1;
	}
	
	public void AddString()
	{
		Dialog newDialog = new Dialog("", "");
		dialogList.Add (newDialog);
		fullTalkList.Add(TalkType.DIALOG);
	}
	
	public void DeleteString(int fullTakIndex, int dialogIndex)
	{
		dialogList.RemoveAt (dialogIndex);
		fullTalkList.RemoveAt (fullTakIndex);
	}
	
	public void AddInteraction()
	{
		List<string> responseList = new List<string>();
		responseList.Add ("Response..");
		List<string> responseList2 = new List<string>();
		responseList2.Add ("Response..");
		
		Response newResponse = new Response("Yes", responseList, Response.AnswerEffect.NONE);
		Response newResponse2 = new Response("No", responseList2, Response.AnswerEffect.NONE);
		
		List<Response> newResponseList = new List<Response>();
		newResponseList.Add(newResponse);
		newResponseList.Add(newResponse2);
		
		ReplyChoice newReply = new ReplyChoice("", "Question", newResponseList);
		interactionList.Add(newReply);
		fullTalkList.Add(TalkType.INTERACTION);
	}
	
	public void DeleteInteraction(int fullTalkIndex, int interactIndex)
	{
		interactionList.RemoveAt(interactIndex);
		fullTalkList.RemoveAt (fullTalkIndex);
	}
	
	public void AddReply(int InteractionIndex)
	{
		List<string> responseList = new List<string>();
		responseList.Add ("Response..");
		
		Response newResponse = new Response("", responseList, Response.AnswerEffect.NONE);
		interactionList[InteractionIndex].answerList.Add(newResponse);
	}
	
	public bool RemoveReply(int fullTalkIndex, int InteractionIndex)
	{
		int count = interactionList[InteractionIndex].answerList.Count;
		
		if(count - 1 == 0) 
		{
			DeleteInteraction(fullTalkIndex, InteractionIndex);
			return true;
		}
		else 
		{
			interactionList[InteractionIndex].answerList.RemoveAt(count - 1);
			return false;
		}
	}
	
	public void ExtendResponse(int InteractionIndex, int answerIndex)
	{
		interactionList[InteractionIndex].answerList[answerIndex].responseList.Add("Extended");
	}
	
	public void DeleteResponse(int InteractionIndex, int answerIndex)
	{
		int count = interactionList[InteractionIndex].answerList[answerIndex].responseList.Count;
		interactionList[InteractionIndex].answerList[answerIndex].responseList.RemoveAt(count - 1);
	}
	
	public void Reset()
	{
		fullTalkList.Clear ();
		dialogList.Clear ();
		interactionList.Clear ();
	}
}
