using UnityEngine;
using System.Collections;

public class DestructibleObstacleScript : DestructableObjectBaseScript 
{
	bool mIsDead;
	// Use this for initialization
	void Start () 
	{
		if(MaxHp == 0) MaxHp = 100;
		CurrHp = MaxHp;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(CurrHp <= 0 && !mIsDead)
		{
			mIsDead = true;
		}
		if(mIsDead)
		{
			GameObject.Destroy(gameObject);
		}
	}
}