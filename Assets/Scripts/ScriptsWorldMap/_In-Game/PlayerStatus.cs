using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour 
{
	public int Level = 7, BaseAtk = 78, BaseDef = 5, CurrHp = 850, MaxHp = 850, Str, End, Agi, Lck, MaxAccSlot = 2;
	public float HpRegen, DropRate, DmgReduction, SpConsumptionRate, ComboMultiplier;
	public int mAtk, mDef;
	
	PlayerController mPlayerController;
	Inventory mInventory;
	
	void Start () 
	{
		mAtk = BaseAtk;
		mDef = BaseDef;
		
		mPlayerController = transform.GetComponent<PlayerController>();
		mInventory = transform.GetComponent<Inventory>();
	}
	
	/*public void UpdateStatus()
	{
		int count = mInventory.AccEquippedList.Count;
		for(int i = 0; i < count; i++)
		{
			UpdateEffect(ItemList.GetAccInfo(mInventory.AccEquippedList[i]));
		}
	}*/
	
	public void SetEquipActive(ItemList.Type type, int slotIndex, bool isEquip)
	{
		if(type == ItemList.Type.ACCESSORIES)
		{
			UpdateAccEffect(ItemList.GetAccInfo(mInventory.AccEquippedList[slotIndex]), isEquip);
		}
		else if(type == ItemList.Type.SOL_CRYST)
		{
			UpdateSolEffect(mInventory.GetSolCrystInfo(mInventory.SolEquippedList[slotIndex]), isEquip);
		}
	}
	
	void UpdateAccEffect(ItemList.AccInfo acc, bool isEquip)
	{
		ItemList.AccInfo.Effects effect = acc.effects;
		float percentage = acc.percentage;
		
		if(effect == ItemList.AccInfo.Effects.NONE) 
		{ 
			string itemName = acc.name;
			if(isEquip) Debug.Log ("Equipping " + itemName + " : Accessory has no effect."); 
			else if(!isEquip) Debug.Log ("Removing " + itemName + " : Accessory has no effect."); 
			return; 
		}
		
		if(effect == ItemList.AccInfo.Effects.ATK) StatChangePerc(ref mAtk, BaseAtk, percentage, isEquip);
		else if(effect == ItemList.AccInfo.Effects.DEF) StatChangePerc(ref mDef, BaseDef, percentage, isEquip);
		/*else if(effect == ItemList.AccInfo.Effects.HP_REGEN) StatChangePerc(ref HpRegen, percentage, isEquip);
		else if(effect == ItemList.AccInfo.Effects.DMG_REDUCTION) StatChangePerc(ref DmgReduction, percentage, isEquip);
		else if(effect == ItemList.AccInfo.Effects.SP_USAGE_REDUC) StatChangePerc(ref SpConsumptionRate, percentage, isEquip);
		else if(effect == ItemList.AccInfo.Effects.DROP_RATE) StatChangePerc(ref DropRate, percentage, isEquip);
		else if(effect == ItemList.AccInfo.Effects.ENC_RATE_INC) StatChangePerc(ref Atk, percentage, true);
		else if(effect == ItemList.AccInfo.Effects.ENC_RATE_DEC) StatChangePerc(ref Atk, percentage, true);
		else if(effect == ItemList.AccInfo.Effects.COMBO_MULTIPLIER) StatChangePerc(ref Atk, percentage, true);
		 */
	}
	
	void StatChangePerc(ref int stat, int baseVal, float percentage, bool isEquip)
	{
		int amount = Mathf.CeilToInt((float)baseVal * percentage);
		
		if(isEquip) stat += amount;
		else if(!isEquip) stat -= amount;
	}
	
	void StatChangePerc(ref float stat, float baseVal, float percentage, bool isEquip)
	{
		int amount = Mathf.CeilToInt((float)baseVal * percentage);
		
		if(isEquip) stat += amount;
		else if(!isEquip) stat -= amount;
	}
	
	void UpdateSolEffect(Inventory.SolCryst sol, bool isEquip)
	{
		ItemList.AttributeBonus.StatType effect = sol.stat.type;
		int amount = sol.stat.amount;
		
		if(effect == ItemList.AttributeBonus.StatType.STR) StatChangeDirect(ref Str, amount, isEquip);
		else if(effect == ItemList.AttributeBonus.StatType.END) StatChangeDirect(ref End, amount, isEquip);
		else if(effect == ItemList.AttributeBonus.StatType.AGI) StatChangeDirect(ref Agi, amount, isEquip);
		else if(effect == ItemList.AttributeBonus.StatType.AGI) StatChangeDirect(ref Lck, amount, isEquip);
	}
	
	void StatChangeDirect(ref int stat, int amount, bool isEquip)
	{
		if(isEquip) stat += amount;
		else if(!isEquip) stat -= amount;
	}
}
