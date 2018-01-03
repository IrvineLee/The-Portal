using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
	public class Acc
	{
		public string name;
		public bool isInitalEquipped = false;
		
		public Acc(string name)
		{
			this.name = name;
		}
		
		public Acc(string name, bool isInitalEquipped)
		{
			this.name = name;
			this.isInitalEquipped = isInitalEquipped;
		}
	};
	
	public class SolCryst
	{
		public enum SolType
		{
			BASIC = 0,
			COMBINED
		};
		
		public string name;
		public SolType type;
		public ItemList.AttributeBonus stat;
		
		// Handle solcryst that contains only 1 attribute.
		public SolCryst(SolType type, ItemList.AttributeBonus stat)
		{
			name = stat.type.ToString() + " + " + stat.amount.ToString();
			this.type = type;
			this.stat = stat;
		}
		
		/*public SolCryst(SolType type, List<AttributeBonus> statsList)
		{
			
		}*/
	};
	
	public List<Acc> AccList = new List<Acc>();
	public List<string> AccEquippedList = new List<string>();
	public List<string> AccSelectionList = new List<string>();
	
	public List<SolCryst> SolList = new List<SolCryst>();
	public List<string> SolEquippedList = new List<string>();
	public List<string> SolSelectionList = new List<string>();
	
	PlayerStatus mPlayerStatus;
	
	void Start () 
	{
		mPlayerStatus = transform.GetComponent<PlayerStatus>();
		
		AccList.Add(new Acc("Lucky Charm"));
		AccList.Add(new Acc("Strongman Bangle"));
		AccList.Add(new Acc("Strongman Bangle 2"));
		//AccList.Add(new Acc("Moonlight Boots"));
		//AccList.Add(new Acc("Barrier Padding"));
		//AccList.Add(new Acc("Auto-Trigger"));
		//AccList.Add(new Acc("Exp Trainer"));
		
		SolList.Add(new SolCryst(SolCryst.SolType.BASIC, new ItemList.AttributeBonus(ItemList.AttributeBonus.StatType.STR, 3)));
		//Debug.Log (SolList[0].stat.amount);
		
		foreach(Acc acc in AccList)
		{ 
			if(!acc.isInitalEquipped) AccSelectionList.Add(acc.name);
			else if(acc.isInitalEquipped) AccEquippedList.Add(acc.name);
		}
		
		foreach(Weapons.WeaponInfo weapon in Weapons.WeaponList)
		{ 
			int equippedSolCount = weapon.equippedSolCrystList.Count;
			
			for(int i = 0; i < equippedSolCount; i++)
			{
				
			}
		}
		
		// Set the amount of accessories slot.
		IncreaseAccSlot (mPlayerStatus.MaxAccSlot);
	}
	
	public int GetAccSelectionCount
	{
		get { return AccSelectionList.Count; }
	}
	
	public void ChangeAcc(int slotNumber, string newAccName)
	{
		UnEquippedItem(ItemList.Type.ACCESSORIES, slotNumber);
		
		// Register new equipment.
		AccEquippedList[slotNumber] = newAccName;
		mPlayerStatus.SetEquipActive(ItemList.Type.ACCESSORIES, slotNumber, true);
		
		// Remove it from the selection list.
		AccSelectionList.Remove (newAccName);
	}
	
	public void UnEquippedItem(ItemList.Type type, int slotNumber)
	{
		/*int 
		
		if(type == ItemList.Type.ACCESSORIES)
		{
			
		}*/
		
		
		
		
		// Un-equipped item if curr slot is not empty.
		if(AccEquippedList[slotNumber] != "")
		{
			// Remove equipment effect.
			mPlayerStatus.SetEquipActive(ItemList.Type.ACCESSORIES, slotNumber, false);
			// Add it back into the selection list.
			AccSelectionList.Add(AccEquippedList[slotNumber]);
			// Revert to empty slot.
			AccEquippedList[slotNumber] = "";
		}
	}
	
	public void IncreaseAccSlot(int amount)
	{
		if(AccEquippedList.Count < amount)
		{
			for(int i = 0; i < amount; i++)
			{ AccEquippedList.Add(""); }
		}
	}
	
	public SolCryst GetSolCrystInfo(string name)
	{
		for(int i = 0; i < SolEquippedList.Count; i++)
		{ if(name == SolList[i].name) return SolList[i]; }
		
		Debug.Log ("Couldn't find specified SolCryst");
		return SolList[0];
	}
}
