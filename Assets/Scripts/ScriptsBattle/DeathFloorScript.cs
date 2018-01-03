using UnityEngine;
using System.Collections;

public class DeathFloorScript : MonoBehaviour 
{
	public LayerMask mPlayerMask,mKillableMask;
	bool mGameOver;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void OnTriggerEnter(Collider collider)
	{
		if(LayerComparison(collider.gameObject, mPlayerMask))
		{
			mGameOver = true;
		}
		if (LayerComparison(collider.gameObject, mKillableMask))
		{
			Destroy(collider.gameObject);
		}
	}
	
	bool LayerComparison(GameObject obj, LayerMask layerToCompare)
	{
		int objLayer = 1 << obj.layer;
		if((objLayer & layerToCompare) > 0)
			return true;
		else
			return false;
	}
}
