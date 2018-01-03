using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour 
{
	public float ObjectiveTimer;
	public static bool IsCountDown = false;
	
	GameObject mDropAllGO;
	
	void Start()
	{
		IsCountDown = false;
		mDropAllGO = GameObject.FindGameObjectWithTag ("Trigger1");
	}
	
	void Update () 
	{
		if(!IsCountDown) return;
		
		ObjectiveTimer -= Time.deltaTime;
		if (ObjectiveTimer <= 0) 
		{
			IsCountDown = false;
			foreach(Transform child in mDropAllGO.transform)
			{
				Rigidbody rigid = child.GetComponent<Rigidbody>();
				if(rigid == null)
				{
					child.gameObject.AddComponent<Rigidbody>();
					
					foreach(Transform subchild in child.transform)
					{
						Rigidbody rigid2 = subchild.GetComponent<Rigidbody>();
						if(rigid2 != null) rigid2.constraints = ~RigidbodyConstraints.FreezeAll;
					}
				}
			}
		}
	}
	
	public void ActivateTimer()
	{
		IsCountDown = true;
	}
	
	void OnGUI()
	{
		if(!IsCountDown) return;
		
		int milliSec = GetMilliSec();
		
		string offsetStr;
		string str;
		
		if (milliSec >= 100) 
		{
			str = " : 00";
		}
		else
		{
			if(milliSec < 10) offsetStr = " : 0";
			else offsetStr = " : ";
			
			str = offsetStr + milliSec.ToString();
		}
		
		if((int)ObjectiveTimer >= 60)
		{
			int minutes = (int)ObjectiveTimer / 60;
			int sec = (int)ObjectiveTimer % 60;
			
			if(sec < 10) offsetStr = " : 0";
			else offsetStr = " : ";
			
			str = minutes.ToString () + offsetStr + sec.ToString() + str;
		}
		else if((int)ObjectiveTimer < 60)
		{
			str = (int)ObjectiveTimer + str;
		}
		
		GUI.skin.label.fontSize = 50;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.color = Color.red;
		GUI.Label(new Rect(Screen.width * 0.4f, Screen.height * 0.1f, Screen.width * 0.3f, Screen.height * 0.2f), str);
	}
	
	int GetMilliSec()
	{
		int timeInt = (int)ObjectiveTimer;
		float milliSec = (ObjectiveTimer % timeInt);
		milliSec = round (milliSec, 2);
		return (int)milliSec;
	}
	
	float round(float val, int decimalPlaces) 
	{
		return Mathf.Round(val * Mathf.Pow(10, decimalPlaces));
	}
}
