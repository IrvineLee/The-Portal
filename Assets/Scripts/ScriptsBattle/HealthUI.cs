using UnityEngine;
using System.Collections;
 
public class HealthUI : MonoBehaviour 
{
	public enum Type
	{
		BAR = 0,
		LIFE
	};
	public Type DisplayType = Type.BAR;
	
	public float barDisplay; //current progress
	public Vector2 pos = new Vector2(0.05f, 0.05f);
	public Vector2 size = new Vector2(0.05f, 0.05f);
	public Texture2D emptyTex;
	public Texture2D fullTex;
	
	public int mLifeNo = 5;
	
	void Start()
	{
		pos.x = Screen.width * pos.x;
		pos.y = Screen.height * pos.y;
		size.x = Screen.width * size.x;
		size.y = Screen.height * size.y;
	}
	
	void OnGUI() 
	{
		if(DisplayType == Type.BAR)
		{
			//draw the background:
			GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
			GUI.Box(new Rect(0, 0, size.x, size.y), emptyTex);
			 
			//draw the filled-in part:
			GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
			GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
			GUI.DrawTexture(new Rect(0, 0, size.x, size.y), fullTex);
			GUI.EndGroup();
			GUI.EndGroup();
		}
		else if(DisplayType == Type.LIFE)
		{
			for(int i = 1; i <= mLifeNo; i++)
			{
				GUI.DrawTexture(new Rect(pos.x * (i * 2.0f), pos.y, Screen.height * 0.03f, Screen.height * 0.03f), fullTex);
			}
		}
	}
	
	public float Hp_Ratio
	{
		set { barDisplay = value; mLifeNo -= 1; }
	}
	 
	void Update() 
	{
		//for this example, the bar display is linked to the current time,
		//however you would set this value based on your desired display
		//eg, the loading progress, the player's health, or whatever.
		//barDisplay = Time.time*0.05f;
		// barDisplay = MyControlScript.staticHealth;
	}
}