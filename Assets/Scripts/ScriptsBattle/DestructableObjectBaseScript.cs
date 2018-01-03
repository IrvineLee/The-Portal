using UnityEngine;
using System.Collections;

public class DestructableObjectBaseScript : MonoBehaviour 
{
	public int MaxHp, CurrHp;
	public bool mKnockback;
	
	bool mIsHit = false;
	
	void Start () 
	{
	}
	
	void Update () 
	{
	}
	
	public bool IsHit
	{
		get { return mIsHit; }
		set { mIsHit = value; }
	}
	
	public void DealDamage(int damage)
	{
		CurrHp = Mathf.Clamp(CurrHp - damage, 0, MaxHp);
		
		if(transform.tag == "Enemy2")
		{
			PL_Anim plAnim = GetComponent<PL_Anim>();
			plAnim.SetAnim (PL_Anim.State.GOT_HIT);
		}
	}
	
	public void HealHp(int amount)
	{
		CurrHp = Mathf.Clamp(CurrHp + amount, 0, MaxHp);
	}
}
