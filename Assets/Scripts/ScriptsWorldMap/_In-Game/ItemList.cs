using UnityEngine;
using System.Collections.Generic;

public class ItemList : MonoBehaviour 
{
	public enum Type
	{
		ACCESSORIES = 0,
		SOL_CRYST
	};
	
	public class AccInfo
	{
		public enum Effects
		{
			NONE = 0,
			ATK,
			DEF,
			HP_REGEN,
			DMG_REDUCTION,
			SP_USAGE_REDUC,
			DROP_RATE,
			ENC_RATE_INC,
			ENC_RATE_DEC,
			COMBO_MULTIPLIER
		};
		
		public string name;
		public string desciption;
		public Effects effects = Effects.NONE;
		public float percentage;
		
		public AccInfo(string name, string desciption, Effects effects, float percentage)
		{
			this.name = name;
			this.desciption = desciption;
			this.effects = effects;
			this.percentage = percentage / 100.0f;
		}
	};
	
	public class AttributeBonus
	{
		public enum StatType
		{
			STR = 0,
			END,
			AGI,
			LCK,
			EFFECT
		};
		
		public StatType type;
		public int amount;
		
		public AttributeBonus(StatType type, int amount)
		{
			this.type = type;
			this.amount = amount;
		}
	};
	
	public static List<AccInfo> AccList = new List<AccInfo>();
	
	public static void RegisterItems()
	{
		AccList.Add(new AccInfo("Strongman Bangle", "Increase ATK by 3%.", AccInfo.Effects.ATK, 3.0f));
		AccList.Add(new AccInfo("Strongman Bangle 2", "Increase ATK by 10%.", AccInfo.Effects.ATK, 10.0f));
		AccList.Add(new AccInfo("Lucky Charm", "Increase chances of getting higher-grade Solcryst.", AccInfo.Effects.DROP_RATE, 20.0f));
		AccList.Add(new AccInfo("Moonlight Boots", "Increase chances of getting higher-grade Solcryst.", AccInfo.Effects.NONE, 0.0f));
		AccList.Add(new AccInfo("Barrier Padding", "Increase chances of getting higher-grade Solcryst.", AccInfo.Effects.NONE, 0.0f));
	}

	public static AccInfo GetAccInfo(string name)
	{
		for(int i = 0; i < AccList.Count; i++)
		{ if(name == AccList[i].name) return AccList[i]; }
		
		Debug.Log ("Couldn't find item.");
		return new AccInfo(name, "NONE.", AccInfo.Effects.NONE, 0.0f);
	}
}
