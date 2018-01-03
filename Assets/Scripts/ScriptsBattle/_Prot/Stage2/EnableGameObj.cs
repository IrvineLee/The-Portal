using UnityEngine;
using System.Collections;

public class EnableGameObj : MonoBehaviour 
{
	public GameObject GO;
	public enum Mode
	{
		ENABLE = 0,
		DISABLE
	};
	public Mode mMode = Mode.ENABLE;
	
	public enum TriggerMode
	{
		COLLISION = 0,
		FUNC_CALL
	};
	public TriggerMode mTriggerMode = TriggerMode.COLLISION;
	
	void OnTriggerEnter(Collider other) 
	{
		if(mTriggerMode != TriggerMode.COLLISION || other.tag != "Player") return;
		
		if(mMode == Mode.ENABLE) GO.SetActive(true);
		else if(mMode == Mode.DISABLE) GO.SetActive(false);
	}
	
	public void ToggleGO(bool active)
	{
		GO.SetActive (active);
	}
}
