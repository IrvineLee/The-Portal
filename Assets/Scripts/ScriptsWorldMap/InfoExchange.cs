using UnityEngine;
using System.Collections;

public class InfoExchange : MonoBehaviour 
{
	static bool mIsWinBattle = false;
	//GameObject mBattl
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static bool IsWinBattle
	{
		get { return mIsWinBattle; }
		set { mIsWinBattle = value; }
	}
	
	public void CacheScene(string tag, string scriptName)
	{
		//GameObject.FindGameObjectWithTag(tag).GetComponent(scriptName)();
	}
	
	public void StoreInfo()
	{
		//mIsWinBattle =
	}
}
