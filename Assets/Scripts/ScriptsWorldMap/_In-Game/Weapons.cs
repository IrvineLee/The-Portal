using UnityEngine;
using System.Collections.Generic;

public class Weapons : MonoBehaviour 
{
	public class WeaponInfo
	{
		public string name;
		public int atk;
		public float criticalRate;
		public int maxSolCrystSlot;
		public List<string> equippedSolCrystList;
		
		public WeaponInfo(string name, int atk, float criticalRate, int maxSolCrystSlot, List<string> equippedSolCrystList)
		{
			this.name = name;
			this.atk = atk;
			this.criticalRate = criticalRate;
			this.maxSolCrystSlot = maxSolCrystSlot;
			this.equippedSolCrystList = equippedSolCrystList;
		}
	};
	
	public static List<WeaponInfo> WeaponList = new List<WeaponInfo>();
	
	public static void RegisterWeapons()
	{
		List<string> equippedSolCryst = new List<string>();
		equippedSolCryst.Add ("Agi + 5");
		equippedSolCryst.Add ("Piercing");
		
		WeaponList.Add (new WeaponInfo("Soul Retriever", 128, 8, 2, equippedSolCryst));
		Debug.Log (WeaponList[0].equippedSolCrystList.Count);
		WeaponList.Add (new WeaponInfo("Freyr Krest", 172, 5, 2, equippedSolCryst));
	}
}
