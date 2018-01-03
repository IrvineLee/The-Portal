using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NeighbourInfo
{
	public int neighbourID;
	public enum Location
	{
		TOP_RIGHT = 0,
		RIGHT,
		BTM_RIGHT,
		BTM_LEFT,
		LEFT,
		TOP_LEFT
	}
	public Location mLocation;
	
	public NeighbourInfo(int neighbourID, int location)
	{
		this.neighbourID = neighbourID;
		mLocation = (Location) location;
	}
}

[System.Serializable]
public class TileInfo
{
	public int tileID;
	public List<int> neighbourIDList;
	public Vector3 pos;
	
	public TileInfo(int tileID, List<int> neighbourIDList, Vector3 pos)
	{
		this.tileID = tileID;
		this.neighbourIDList = neighbourIDList;
		this.pos = pos;
	}
};

[System.Serializable]
public class Tile
{
	public GameObject tile;
	public bool isBoundary = false;
	
	public Tile()
	{
		this.tile = null;
		this.isBoundary = false;
	}
	
	public Tile(GameObject tile, bool isBoundary)
	{
		this.tile = tile;
		this.isBoundary = isBoundary;
	}
};

[ExecuteInEditMode]
public class HexGenerator : MonoBehaviour 
{
	public GameObject prefabHex;
	public GameObject prefabBoundary;
	public GameObject prefabOuterBoundary;
	public GameObject prefabWall;
	public int mRow = 1, mCol = 1, mAddID = -1;
	
	public enum RelinkType
	{
		ADD_COLLIDER = 0,
		DELETE
	};
	public RelinkType mRelinkType = RelinkType.ADD_COLLIDER;
	
	public enum AddType
	{
		TILE = 0,
		COLLIDER
	};
	public AddType mAddType = AddType.TILE;
	
	[SerializeField]
	public List<TileInfo> mDeletedTileList = new List<TileInfo>();
	
	[SerializeField]
	public Tile[] mTileArray = new Tile[0];
	
	int prevRow = -1, prevCol = -1;
	
	// Scan surrounding tile and spawn walls.
	float mRayCastDistance = 2.5f;
	LayerMask mTileLayerMask = 1 << 8;
	
	// Testing purposes. Deleting soon.
	int OriROW = 3, OriCOL = 3;
	
	public void GenerateTiles()
	{
		DestroyChildren();
		GenerateTilesAndLink();
	}
	
	// Destroy all children.
	public void DestroyChildren()
	{
		if(mTileArray.Length > 0)
		{
			int childCount = transform.childCount;
			for(int i = childCount - 1; i >= 0; i--) 
			{
				Transform child = transform.GetChild(i);
				if(child.tag != "FOW") DestroyImmediate(child.gameObject);
			}
			mTileArray = new Tile[0];
			mDeletedTileList.Clear ();
			Resources.UnloadUnusedAssets ();
		}
	}
	
	// Refresh neighbours ID after tile has been deleted.
	public void Refresh()
	{
		// Check whether any tiles have been deleted. 
		if(mDeletedTileList.Count > 0)
		{
			// Loop number of tiles deleted.
			for(int i = 0; i < mDeletedTileList.Count; i++)
			{
				// Get deleted tiles neighbour's ID.
				foreach(int neighbourID in mDeletedTileList[i].neighbourIDList)
				{
					// Ensure neighbour is a valid target. Only time this is null is when multiple side-by-side tiles are deleted.
					if(mTileArray[neighbourID].tile != null && !mTileArray[neighbourID].isBoundary)
					{
						// Unlinked connected tiles neighbour's ID.
						foreach(int id in mTileArray[neighbourID].tile.GetComponent<HexScript>().NeighbourIDList)
						{
							if(id == mDeletedTileList[i].tileID)
							{
								Debug.Log("Tile " + neighbourID + " deleted id  " + mDeletedTileList[i].tileID);
								mTileArray[neighbourID].tile.GetComponent<HexScript>().NeighbourIDList.Remove (id);
								break;
							}
						}
					}
				}
				if(mRelinkType == RelinkType.ADD_COLLIDER) AddInnerBoundaries(mDeletedTileList[i]);
				else if(mRelinkType == RelinkType.DELETE) 
				{
					bool isResize = true;
					
					for(int x = 0; x < mRow; x++)
					{
						for(int y = 0; y < mCol; y++)
						{
							int index = x + y * mRow;
							if(y == mCol - 1 && mTileArray[index].tile != null) isResize = false;
						}
					}
					
					if(isResize) mCol -= 1;
				}
			}
			mDeletedTileList.Clear ();
			Resources.UnloadUnusedAssets ();
			Debug.Log ("Linking redone.");
		}
		else
		{ Debug.Log ("Nothing was deleted. No need to relink."); }
	}
	
	// Draw link between tiles.
	/*public void DrawLink()
	{
		for(int i = 0; i < mTileArray.Length; i++)
		{
			if(mTileArray[i] != null)
			{
				Vector3 tilePos = mTileArray[i].tile.GetComponent<HexScript>().Position;
				List<int> neighbourIDList = mTileArray[i].tile.GetComponent<HexScript>().NeighbourIDList;
				
				foreach (int id in neighbourIDList)
				{
					Vector3 neighbourPos = mTileArray[id].tile.GetComponent<HexScript>().Position;
					Debug.DrawLine(tilePos, neighbourPos, Color.red);
				}
			}
		}
	}*/
	
	// Get previously generated tiles after scene has been restarted.
	public void Reload()
	{
		if(transform.childCount != 0)
		{
			mTileArray = new Tile[mRow * mCol];
		
			foreach (Transform child in transform) 
			{
				if(child.name == "Hex")
				{
					int index = child.GetComponent<HexScript>().TileID;
					mTileArray[index] = new Tile(child.gameObject, false);
				}
			}
			Debug.Log ("Reloaded.");
		}
	}
	
	// Add new tile.
	public void AddTileByID(int addID)
	{
		if(mAddID > 0)
		{
			// If is within the mTileArray range, add new tile.
			if(addID < mRow * mCol)
			{
				Debug.Log ("Relinking...");
				Refresh ();
				
				// Check whether user is trying to add to an existing tile.
				if(mTileArray[addID].tile != null 
					&& ((mAddType == AddType.TILE && !mTileArray[addID].isBoundary)
					|| (mAddType == AddType.COLLIDER && mTileArray[addID].isBoundary)))
				{
					if(mAddType == AddType.TILE) Debug.Log ("Error! Specified ID is an existing tile.");
					if(mAddType == AddType.COLLIDER) Debug.Log ("Error! Specified ID is an existing collider.");
					return;
				}
				// If it's a boundary, delete boundary and add new tile.
				if(mAddType == AddType.TILE)
				{
					if(mTileArray[addID].isBoundary) 
					{
						Transform innerBoundaries = transform.FindChild("InnerBoundaries");
						int childCount = innerBoundaries.childCount;
						
						for(int i = 0; i < childCount; i++)
						{
							if(innerBoundaries.GetChild(i).name == "Boundary"
								&& innerBoundaries.GetChild(i).GetComponent<BoundaryScript>().TileID == addID)
							{ DestroyImmediate(innerBoundaries.GetChild(i).gameObject); break;}
						}
						mTileArray[addID] = null;
						AddNewTileAndLink(addID, mAddType);
					}
					else if(mTileArray[addID].tile == null) AddNewTileAndLink(addID, mAddType);
					Debug.Log ("Added: Tile " + addID);
				}
				// If it's a tile, delete tile and add collider.
				else if(mAddType == AddType.COLLIDER) 
				{
					if(!mTileArray[addID].isBoundary) 
					{
						for(int i = 0; i < transform.childCount; i++)
						{
							if(transform.GetChild(i).name == "Hex"
								&& transform.GetChild(i).GetComponent<HexScript>().TileID == addID)
							{ DestroyImmediate(transform.GetChild(i).gameObject); break;}
						}
						mTileArray[addID] = null;
						AddNewTileAndLink(addID, mAddType);
					}
					else if(mTileArray[addID].tile == null) AddNewTileAndLink(addID, mAddType);
					Debug.Log ("Added: Boundary " + addID);
				}
			}
			// If is NOT within the mTileArray range, expand the size, then add new tile.
			// Check whether specified tile ID will connect if range is expanded.
			else if(addID <= (mRow * (mCol + 1)) - 1)
			{
				mCol += 1;
				System.Array.Resize(ref mTileArray, mRow * mCol);
				if(mAddType == AddType.TILE) 
				{
					AddNewTileAndLink(addID, mAddType); 
					Debug.Log ("Added: Tile " + addID);
				}
				else if(mAddType == AddType.COLLIDER)
				{
					AddNewTileAndLink(addID, mAddType);
					Debug.Log ("Added: Boundary " + addID);
				}
			}
			else Debug.Log ("Error! Specified ID is not connected to any tiles.");
		}
		else Debug.Log ("Error! Please specified an ID greater than 0.");
	}
	
	// Reset all values to default.
	public void Reset()
	{
		mRow = OriROW;
		mCol = OriCOL;
		prevRow = -1;
		prevCol = -1;
		System.Array.Resize(ref mTileArray, mRow * mCol);
		mTileArray = new Tile[mRow * mCol];
		Debug.Log ("All values have been reset to default.");
	}
	
	// Testing purposes. Deleting soon.
	public void Check()
	{
		Debug.Log ("Row: " + mRow + "Col: " + mCol);
		Debug.Log ("Checked");
	}
	
	// Delete boundaries not linking to a tile.
	public void Delete()
	{
		DeleteUnusedInnerBoundaries ();
		Debug.Log ("Deleted unused inner boundaries.");
	}
	
	// Add outer boundaries.
	public void GenerateOuterBoundaries()
	{
		GameObject outerBoundaries = null;
		for(int i = 0; i < transform.childCount; i++)
		{
			if(transform.GetChild (i).name == "OuterBoundaries")
			{ outerBoundaries = transform.GetChild (i).gameObject; break; }
		}
		
		if(outerBoundaries == null || prevRow != mRow || prevCol != mCol)
		{
			prevRow = mRow;
			prevCol = mCol;
			DeleteOuterBoundaries();
			AddOuterBoundaries();
			Debug.Log ("Refreshed outer boundaries.");
		}
		else { Debug.Log ("No need to refresh outer boundaries."); }
	}
	
	public void GenerateWalls()
	{
		Transform walls = transform.FindChild("Walls");
		if(walls != null) { Debug.Log ("Already have walls."); return; }
		
		float height = prefabHex.renderer.bounds.size.z / 2;
		
		GameObject empty = new GameObject();
		empty.name = "Walls";
		empty.transform.position = transform.position;
		empty.transform.parent = transform;
		
		for(int i = 0; i < mTileArray.Length; i++)
		{
			GameObject tile = mTileArray[i].tile;
			if(mTileArray[i].tile != null && !mTileArray[i].isBoundary)
			{
				if(tile.GetComponent<HexScript>().NeighbourIDList.Count < 6)
				{
					Vector3 pos = tile.transform.position;
					//float startAngle = segmentAngle * i;
					
					int segments = 6;
					float segmentAngle = (2.0f * Mathf.PI) / (float)segments;
					
					for (int j = 0; j <= segments; j++)
					{
						float angle = segmentAngle * j;
						AddWallsAt(pos.x + (Mathf.Cos(angle) * height), pos.z + (Mathf.Sin(angle) * height), angle, pos, empty);
					}
				}
			}
		}
	}
	
	// Destroy walls.
	public void DestroyWalls()
	{
		DeleteWalls();
	}
	
	
	// Destroy outer boundaries.
	public void DestroyOuterBoundaries()
	{
		DeleteOuterBoundaries();
	}
	
	// --------------------------------------------//
	// ----- All functions below are private ----- //
	// --------------------------------------------//
	
	void GenerateTilesAndLink()
	{
		mTileArray = new Tile[mRow * mCol];
		
		for (int x = 0; x < mRow; x++)
		{
			for (int y = 0; y < mCol; y++)
			{
				int index = x + y * mRow;
				AddTileToScene(index, x, y, mAddType, true);
			}
		}
		LinkInitalTiles();
	}
	
	void AddNewTileAndLink(int addID, AddType type)
	{
		for (int x = 0; x < mRow; x++)
		{
			for (int y = 0; y < mCol; y++)
			{
				int index = x + y * mRow;
				if(index == addID) AddTileToScene(index, x, y, type, false);
			}
		}
		LinkNewTile(addID);
	}
	
	void AddTileToScene(int index, int currRow, int currCol, AddType type, bool isFirstTime)
	{
		float width = prefabHex.renderer.bounds.size.x;
		float height = prefabHex.renderer.bounds.size.z * 3/4;
		
		Vector3 position = new Vector3(
				transform.position.x + (currCol * width),
				transform.position.y,
				transform.position.z + (currRow * height));
			
		if(currRow % 2 != 0) position.x += width / 2;
		
		if(type == AddType.TILE || isFirstTime)
		{
			GameObject tile = (GameObject)Instantiate(prefabHex, position, Quaternion.identity);
			tile.name = prefabHex.name;
			tile.transform.parent = transform;
			tile.GetComponent<HexScript>().TileID = index;
			//tile.GetComponent<HexScript>().Position = position;
			mTileArray[index] = new Tile(tile, false);
		}
		else if(type == AddType.COLLIDER)
		{
			GameObject boundary = (GameObject)Instantiate(prefabBoundary, position, Quaternion.identity);
			boundary.name = prefabBoundary.name;
			boundary.transform.parent = transform;
			boundary.GetComponent<BoundaryScript>().TileID = index;
			mTileArray[index] = new Tile(boundary, true);
		}
	}
	
	void LinkInitalTiles()
	{
		for (int x = 0; x < mRow; x++)
		{
			for (int y = 0; y < mCol; y++)
			{
				int currIndex = x + y * mRow;
				LinkTiles(currIndex, x, y, false);
			}
		}
	}
	
	void LinkNewTile(int addID)
	{
		for (int x = 0; x < mRow; x++)
		{
			for (int y = 0; y < mCol; y++)
			{
				int currIndex = x + y * mRow;
				if(currIndex == addID) LinkTiles(currIndex, x, y, true);
			}
		}
	}
	
	// ---- Linking clockwise ----
	// 	  3 o
	//		  o
	//	 2 o o o Tile
	//		  o
	//	  1 o
	// ---------------------------
	void LinkTiles(int currIndex, int currRow, int currCol, bool isCheckAllAngles)
	{
		List<int> neighbourList = new List<int>();
		
		if(mTileArray[currIndex].tile != null)
		{
			// Even row
			if(currRow % 2 == 0)
			{
				if(isCheckAllAngles)
				{
					// Check top right.
					if(currRow < mRow - 1) neighbourList.Add((currRow + 1) + currCol * mRow);

					// Check right.
					if(currCol < mCol - 1) neighbourList.Add(currRow + (currCol + 1) * mRow);
	
					// Check bottom right.
					if(currRow > 0) neighbourList.Add((currRow - 1) + currCol * mRow);
				}
				
				// Check bottom left.
				if(currRow > 0 && currCol > 0) neighbourList.Add((currRow - 1) + (currCol - 1) * mRow);

				// Check left.
				if(currCol > 0) neighbourList.Add(currRow + (currCol - 1) * mRow);

				// Check top left.
				if(currCol > 0 && currRow < mRow - 1) neighbourList.Add((currRow + 1)+ (currCol - 1) * mRow);
			}
			//Odd row
			else if(currRow % 2 != 0)
			{
				if(isCheckAllAngles)
				{
					// Check top right.
					if(currRow < mRow - 1 && currCol < mCol - 1) neighbourList.Add((currRow + 1) + (currCol + 1) * mRow);

					// Check right.
					if(currCol < mCol - 1) neighbourList.Add(currRow + (currCol + 1) * mRow);
	
					// Check bottom right.
					if(currRow > 0 && currCol < mCol - 1) neighbourList.Add((currRow - 1) + (currCol + 1) * mRow);
				}
				
				
				// Check bottom left.
				if(currCol >= 0) neighbourList.Add((currRow - 1) + currCol * mRow);

				// Check left.
				if(currCol > 0) neighbourList.Add(currRow + (currCol - 1) * mRow);

				// Check top left.
				if(currCol >= 0 && currRow < mRow - 1) neighbourList.Add((currRow + 1)+ currCol * mRow);
			}
			
			foreach(int neighbourIndex in neighbourList) 
			{
				// Make sure it doesn't add anything outside of mTileArray range.
				if(mTileArray[neighbourIndex] != null)
				{
					if(mTileArray[neighbourIndex].tile != null && !mTileArray[neighbourIndex].isBoundary) 
					{
						// isCheckAllAngles is only false during the initial tile generation.
						if(mAddType == AddType.TILE || !isCheckAllAngles)
						{
							mTileArray[currIndex].tile.GetComponent<HexScript>().NeighbourIDList.Add(neighbourIndex);
							mTileArray[neighbourIndex].tile.GetComponent<HexScript>().NeighbourIDList.Add(currIndex);
						}
						else if(mAddType == AddType.COLLIDER)
						{
							mTileArray[currIndex].tile.GetComponent<BoundaryScript>().NeighbourIDList.Add(neighbourIndex);
						}
					}
				}
			}
			neighbourList.Clear ();
		}
	}
	
	void AddInnerBoundaries(TileInfo deletedTile)
	{
		GameObject empty = null;
		Transform innerBoundaries = transform.FindChild("InnerBoundaries");
		
		if(innerBoundaries != null) empty = innerBoundaries.gameObject;	
		else if(innerBoundaries == null)
		{
			empty = new GameObject();
			empty.name = "InnerBoundaries";
			empty.transform.position = transform.position;
			empty.transform.parent = transform;
		}
		
		GameObject boundaryTile = (GameObject)Instantiate(prefabBoundary, deletedTile.pos, Quaternion.identity);
		boundaryTile.name = prefabBoundary.name;
		boundaryTile.transform.parent = empty.transform;
		boundaryTile.GetComponent<BoundaryScript>().TileID = deletedTile.tileID;
		foreach(int id in deletedTile.neighbourIDList) boundaryTile.GetComponent<BoundaryScript>().NeighbourIDList.Add (id);
		mTileArray[deletedTile.tileID] = new Tile(boundaryTile, true);
	}
	
	void AddOuterBoundaries()
	{
		int boundaryRow = mRow + 2;
		int boundaryCol = mCol + 2;
		
		GameObject empty = new GameObject();
		empty.name = "OuterBoundaries";
		empty.transform.position = transform.position;
		empty.transform.parent = transform;
		
		for (int x = 0; x < boundaryRow; x++)
		{
			for (int y = 0; y < boundaryCol; y++)
			{
				// Ensure existing tile does not get added boundary.
				if(x != 0 && x != boundaryRow - 1 && y != 0 && y != boundaryCol - 1) 
				{ continue; }
				
				// Ensure unnecessary boundary does not get added.
				// Even row.
				if(mRow % 2 == 0 && ((x == 0 && y == boundaryCol - 1) || (x == boundaryRow - 1 && y == 0)))
				{ continue; }
				
				// Odd row.
				else if(mRow % 2 != 0 && (x == 0 || x ==  boundaryRow - 1) && y == boundaryCol - 1 )
				{ continue; }
				
				AddOuterBoundariesToScene(x, y, empty);
			}
		}
		
		
	}
	
	void AddOuterBoundariesToScene(int currRow, int currCol, GameObject emptyGroup)
	{
		float width = prefabHex.renderer.bounds.size.x;
		float height = prefabHex.renderer.bounds.size.z * 3/4;
		
		Vector3 position = new Vector3(
				(transform.position.x + (currCol * width) - (width / 2)),
				transform.position.y,
				(transform.position.z + (currRow * height) - height));
			
		if(currRow % 2 != 0) position.x -= width / 2;
		
		GameObject tile = (GameObject)Instantiate(prefabOuterBoundary, position, Quaternion.identity);
		tile.name = prefabOuterBoundary.name;
		tile.transform.parent = emptyGroup.transform;
	}
	
	void AddWallsAt(float x, float z, float angle, Vector3 oriPos, GameObject emptyGroup)
	{
		Ray ray = new Ray(new Vector3(x, 0.5f, z), -transform.up);
		RaycastHit hit;
		float width = prefabHex.renderer.bounds.size.x / 2;
		
		if(!Physics.Raycast (ray, out hit, mRayCastDistance, mTileLayerMask))
		{
			Vector3 edgePos = oriPos;
			edgePos.x = oriPos.x + (Mathf.Cos(angle) * (width + 0.025f));
			edgePos.z = oriPos.z + (Mathf.Sin(angle) * (width + 0.025f));
			edgePos.y += prefabWall.renderer.bounds.size.y / 2;
			
			Vector3 eularAngle = prefabWall.transform.eulerAngles;
			eularAngle.y = 360 - (angle * Mathf.Rad2Deg);
			Quaternion wallRotation = Quaternion.Euler(eularAngle);
			
			Vector3 wallSize = prefabWall.renderer.bounds.size;
			wallSize.z = 0.79f;
			prefabWall.transform.localScale = wallSize;
			
			GameObject wall = (GameObject)Instantiate(prefabWall, edgePos, wallRotation);
			wall.name = prefabWall.name;
			wall.transform.parent = emptyGroup.transform;
		}
	}
	
	void DeleteOuterBoundaries()
	{
		Transform outerBoundaries = transform.FindChild("OuterBoundaries");
		if(outerBoundaries != null) DestroyImmediate(outerBoundaries.gameObject);
	}
	
	void DeleteWalls()
	{
		Transform walls = transform.FindChild("Walls");
		if(walls != null) DestroyImmediate(walls.gameObject);
	}
	
	void DeleteUnusedOuterBoundaries()
	{
		
		/*Ray ray = new Ray(transform.position, transform.right);
		RaycastHit hit;
		mRayCastDistance = 2.5f;
		
		if(Physics.Raycast (ray, out hit, mRayCastDistance))
		{
			
		}*/
	}
	
	void DeleteUnusedInnerBoundaries()
	{
		bool isConnectTile = false;
		
		for(int i = 0; i < mTileArray.Length; i++)
		{
			if(mTileArray[i].isBoundary)
			{
				foreach(int neighbourID in mTileArray[i].tile.GetComponent<BoundaryScript>().NeighbourIDList)
				{
					// If boundary is connected to a tile, ignore it.
					if(!mTileArray[neighbourID].isBoundary) 
					{
						isConnectTile = true;
						break;
					}
				}
				
				if(!isConnectTile) // Delete the boundary.
				{
					Transform innerBoundaries = transform.FindChild("InnerBoundaries");
					
					if(innerBoundaries != null)
					{
						int childCount = innerBoundaries.childCount;
						
						for(int j = 0; j < childCount; j++)
						{
							if(innerBoundaries.GetChild(j).name == "Boundary"
								&& innerBoundaries.GetChild(j).GetComponent<BoundaryScript>().TileID == i)
							{ 
								foreach(int neighbourID in innerBoundaries.GetChild(j).GetComponent<BoundaryScript>().NeighbourIDList)
								{
									foreach(int id in mTileArray[neighbourID].tile.GetComponent<BoundaryScript>().NeighbourIDList)
									{
										if(id == i) 
										{
											mTileArray[neighbourID].tile.GetComponent<BoundaryScript>().NeighbourIDList.Remove (id);
											break;
										}
									}
								}
								
								DestroyImmediate(innerBoundaries.GetChild(j).gameObject); 
								break;
							}
						}
						mTileArray[i] = new Tile();
					}
				}
				isConnectTile = false;
			}
		}
	}
}