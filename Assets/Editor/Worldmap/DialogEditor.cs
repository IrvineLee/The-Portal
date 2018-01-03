using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DialogScript))]
public class DialogEditor : Editor
{
	DialogScript mSelf;
	
	[SerializeField] bool foldoutStringArray = true;

	int dialogSize, interactSize, dialogCount = 0, interactCount = 0;
	Vector2[] scroll;
	bool isDeleteInteract = false;
	
	void OnEnable () 
	{
		mSelf = (DialogScript)target;
	}
	
	public override void OnInspectorGUI() 
	{
		foldoutStringArray = EditorGUILayout.Foldout(foldoutStringArray, "Dialog");
		if (foldoutStringArray)
		{
			if(dialogSize != mSelf.dialogList.Count || interactSize != mSelf.interactionList.Count)
			{
				dialogSize = mSelf.dialogList.Count;
				scroll = new Vector2[dialogSize];
				
				interactSize = mSelf.interactionList.Count;
			}
			
			mSelf.mTalkScene = (DialogScript.TalkScene) EditorGUILayout.EnumPopup ("Scene", mSelf.mTalkScene);
			
			EditorGUILayout.LabelField("Dialog Count", dialogSize.ToString ());
			EditorGUILayout.LabelField("Interact. Count", interactSize.ToString ());
			for(int i = 0; i < mSelf.fullTalkList.Count; i++)
			{ 
				if(mSelf.fullTalkList[i] == DialogScript.TalkType.DIALOG)
				{
					EditorGUILayout.LabelField("Dialog");
					EditorStyles.textField.wordWrap = true;
					scroll[dialogCount] = EditorGUILayout.BeginScrollView(scroll[dialogCount], GUILayout.Height (100));
					mSelf.dialogList[dialogCount].name = EditorGUILayout.TextField("Name", mSelf.dialogList[dialogCount].name); 
					mSelf.dialogList[dialogCount].dialog = EditorGUILayout.TextArea(mSelf.dialogList[dialogCount].dialog, GUILayout.Height(100)); 
					EditorGUILayout.EndScrollView();
					
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Delete!",GUILayout.Width(50))) 
					{
						mSelf.DeleteString(i, dialogCount);
						ResetVal();
						break;
					}
					GUILayout.EndHorizontal();
					
					dialogCount += 1;
				}
				else if(mSelf.fullTalkList[i] == DialogScript.TalkType.INTERACTION)
				{
					int replyCount = mSelf.interactionList[interactCount].answerList.Count;
					
					EditorGUILayout.LabelField("Question");
					mSelf.interactionList[interactCount].name = EditorGUILayout.TextField("Name", mSelf.interactionList[interactCount].name);
					mSelf.interactionList[interactCount].question = EditorGUILayout.TextArea(mSelf.interactionList[interactCount].question, GUILayout.Height(30));
					
					EditorGUILayout.LabelField("Replies");
					for(int j = 0; j < replyCount; j++)
					{
						EditorStyles.textField.wordWrap = true;
						mSelf.interactionList[interactCount].answerList[j].reply = EditorGUILayout.TextArea(mSelf.interactionList[interactCount].answerList[j].reply, GUILayout.Height(30)); 
						
						int responseCount = mSelf.interactionList[interactCount].answerList[j].responseList.Count;
						for(int k = 0; k < responseCount; k++)
						{
							GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							GUILayout.BeginVertical();
							
							if(responseCount - 1 - k == 0)
							{
								if (GUILayout.Button("+",GUILayout.Width(20), GUILayout.Height(15))) mSelf.ExtendResponse(interactCount, j);
								else if (GUILayout.Button("-",GUILayout.Width(20), GUILayout.Height(15))) 
								{
									mSelf.DeleteResponse(interactCount, j);
									ResetVal();
									break;
								}
							}
							
							GUILayout.EndVertical();
							
							mSelf.interactionList[interactCount].answerList[j].responseList[k] 
								= EditorGUILayout.TextArea(mSelf.interactionList[interactCount].answerList[j].responseList[k], GUILayout.Width(0.8f * Screen.width), GUILayout.Height(30)); 
							
							GUILayout.EndHorizontal();
						}
						
						mSelf.interactionList[interactCount].answerList[j].effect
							= (Response.AnswerEffect) EditorGUILayout.EnumPopup("Effect", mSelf.interactionList[interactCount].answerList[j].effect);
					}
					
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Add",GUILayout.Width(60))) mSelf.AddReply(interactCount);
					else if (GUILayout.Button("Remove",GUILayout.Width(60))) 
					{
						isDeleteInteract = mSelf.RemoveReply(i, interactCount);
						
						if(isDeleteInteract)
						{
							ResetVal();
							break;
						}
					}
					else if (GUILayout.Button("Delete",GUILayout.Width(60))) 
					{
						mSelf.DeleteInteraction(i, interactCount);
						ResetVal();
						break;
					}
					GUILayout.EndHorizontal();
					interactCount += 1;
				}
				
				if(dialogCount + interactCount == mSelf.fullTalkList.Count) ResetVal();
			}
			
			EditorGUILayout.Space ();
			if (GUILayout.Button("Add Dialog!")) mSelf.AddString();
			else if (GUILayout.Button("Add Interaction!")) mSelf.AddInteraction();
			else if (GUILayout.Button("Reset")) { ResetVal(); mSelf.Reset(); }
		}
		/*if (foldoutStringArray)
		{
			if(size != mSelf.dialogList.Count)
			{
				size = mSelf.dialogList.Count;
				scroll = new Vector2[size];
			}
			
			EditorGUILayout.LabelField("Size", size.ToString ());
			for(int i = 0; i < size; i++)
			{ 
				EditorStyles.textField.wordWrap = true;
				scroll[i] = EditorGUILayout.BeginScrollView(scroll[i], GUILayout.Height (100));
				mSelf.dialogList[i] = EditorGUILayout.TextArea(mSelf.dialogList[i], GUILayout.Height(100)); 
				EditorGUILayout.EndScrollView();
				if (GUILayout.Button("Delete!",GUILayout.Width(50))) 
				{
					mSelf.DeleteString(i);
					break;
				}
			}
			
			EditorGUILayout.LabelField("Interaction", mSelf.interactionList.Count.ToString ());
			for(int i = 0; i < mSelf.interactionList.Count; i++)
			{ 
				mSelf.interactionList[i] = EditorGUILayout.TextArea(mSelf.interactionList[i], GUILayout.Height(30)); 
				
				if (GUILayout.Button("Delete!",GUILayout.Width(50))) 
				{
					mSelf.DeleteInteraction(i);
					break;
				}
			}
			
			EditorGUILayout.Space ();
			if (GUILayout.Button("Add Dialog!")) mSelf.AddString();
			else if (GUILayout.Button("Add Interaction!")) mSelf.AddInteraction();
			//if (GUI.changed) { EditorUtility.SetDirty(target); }
			
		}*/
	}
	
	void ResetVal()
	{
		dialogCount = 0; 
		interactCount = 0;
	}
}