using UnityEngine;
using System.Collections;

public class TypeWriterStyle : MonoBehaviour 
{
	public float letterPause = 0.2f;
	
	static TypeWriterStyle mInstance = null;
	static bool isTexting = false;
	string mCurrentWord = "";
	
	void Awake()
    {
        DontDestroyOnLoad(this);
        mInstance = this;
    }
	
	static TypeWriterStyle Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = (new GameObject("TypeWriter")).AddComponent<TypeWriterStyle>();
            }
            return mInstance;
        }
    }
	
	public static bool IsTexting
	{
		get { return isTexting; }
		set { isTexting = value; }
	}
	
	public static string GetCurrWord
	{
		get { return Instance.mCurrentWord; }
	}
	
	public static void Register(string sentence)
	{
		isTexting = true;
		Instance.mCurrentWord = "";
		Instance.StartDisplayText(sentence);
	}
	
	private void StartDisplayText(string sentence)
    {
        StartCoroutine(DisplayText(sentence));
    }
	
	IEnumerator DisplayText(string sentence)
	{
		foreach (char letter in sentence.ToCharArray()) 
		{
			if(!isTexting) yield break;
			mCurrentWord += letter;
			yield return new WaitForSeconds (letterPause * Random.Range(0.5f, 2.0f));
		}
		isTexting = false;
	}
}
