using UnityEngine;
using System.Collections;

public class EnemyGeneratorScript : MonoBehaviour 
{
	public GameObject mSpawnerTemplate;
	public bool mEnabled = true;
	
	public float mTimerCurr,mTimerMax = 5.0f;
	// Use this for initialization
	void Start () 
	{
		mTimerCurr = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mEnabled)
		{
			Debug.DrawRay(transform.position,transform.forward* 5.0f);
			mTimerCurr += Time.deltaTime;
			if(mTimerCurr >= mTimerMax)
			{
				mTimerCurr = 0.0f;
				GameObject temp = (GameObject)Instantiate(mSpawnerTemplate,transform.position * 5.0f,Quaternion.identity);
				temp.SetActive(true);
			}
		}
	}
}
