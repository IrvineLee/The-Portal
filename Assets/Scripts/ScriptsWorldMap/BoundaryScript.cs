using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BoundaryScript : MonoBehaviour 
{
	public int TileID;
	public List<int> NeighbourIDList = new List<int>();
	
	void OnDestroy()
	{
		if(!Application.isPlaying)
		{
			HexGenerator parentTemp = transform.parent.parent.GetComponent<HexGenerator>();
			TileInfo tileTemp = new TileInfo(TileID, NeighbourIDList, transform.position);
			parentTemp.mTileArray[TileID] = null;
			parentTemp.mDeletedTileList.Add(tileTemp);
			Resources.UnloadUnusedAssets ();
		}
	}
}
