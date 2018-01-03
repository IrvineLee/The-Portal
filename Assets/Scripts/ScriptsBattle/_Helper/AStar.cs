using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class NodeLink
{
	public int neighbourIndex;
	public float cost;

	public NodeLink(int neighbourIndex, float cost)
	{
		this.neighbourIndex = neighbourIndex;
		this.cost = cost;
	}
};

[System.Serializable]
public class Node
{
	public Vector3 mPoint = Vector3.zero;
	public int mIndex = 0;
	public List<NodeLink> mNeighbourIndexCostList = new List<NodeLink>();
};

[ExecuteInEditMode]
public class AStar : MonoBehaviour
{
	public GameObject Player;
	//public Vector3 Destination;
	
	List<Node> pathFindList = new List<Node>();
	int Index = 0;
	bool IsMove = false;
	bool IsNextIndex = false;
	Vector3 nextPosition = Vector3.zero;
	SimpleController mSimpleController;
	
	void Start()
	{
		//Player = GameObject.FindGameObjectWithTag ("Enemy2");
		//Goal = GameObject.FindGameObjectWithTag ("Goal");
		//mSimpleController = Player.GetComponent<SimpleController>();
	}
	
	
	public LayerMask mLayer = -1;
	public float mRadius = 0.5f;
	public bool mIsDrawGizmo = false;
	
	//! Row colum used for generating the graph.
	public int mCol = 10, mRow = 10;

	[SerializeField]
	Node[] mNodes = new Node[0];

	bool testRayHit(Vector3 start, Vector3 end)
	{
		RaycastHit result;
		Vector3 dir = end - start;
		float dist = dir.magnitude;
		dir.Normalize();
		return
			Physics.SphereCast(start, mRadius, dir, out result, dist, mLayer) ||
			Physics.SphereCast(end, mRadius, -dir, out result, dist, mLayer);
	}

	public void regenerateGraph()
	{
		// NOTE: This code assumes collider is a simple plane.
		Bounds bounds = collider.bounds;
		float cellHeight = bounds.size.x / mRow;
		float cellWidth = bounds.size.z / mCol;
		float halfCellHeight = cellHeight * 0.5f;
		float halfCellWidth = cellWidth * 0.5f;
		float diagonalCost = Mathf.Sqrt(cellHeight * cellHeight + cellWidth * cellWidth);

		Debug.Log("Generate nodes...");

		// generate nodes.
		int x, y; OnDestroy();
		mNodes = new Node[mRow * mCol];
		for (x = 0; x < mRow; ++x)
		{
			for (y = 0; y < mCol; ++y)
			{
				Vector3 point = new Vector3(
					bounds.min.x + halfCellHeight + cellHeight * x,
					bounds.max.y + mRadius,
					bounds.min.z + halfCellWidth + cellWidth * y);

				if (Physics.CheckSphere(point, mRadius, mLayer)) continue;

				Node node = new Node();
				node.mPoint = point;
				mNodes[x + y * mRow] = node;
			}
		}

		Debug.Log("Linking nodes...");

		// TODO! Link Nodes here!
		
		//the following code is to raycast for nodes, raycasting like this pattern
		//  o    o    o
		//    o  o  o
		//      o o 
		//       oooooooooo
		//
		//horizontal(col)-> vertical(row), in this case, i = horizontal, j = vertical
		
		for(x = 0; x < mRow; x++)
		{
			for(y = 0; y < mCol; y++)
			{
				int currIndex = x + y * mRow;
				Node currNode = mNodes[currIndex];
				
				if(currNode == null)continue;
				currNode.mIndex = currIndex;
				
				//check for top left
				if(x > 0 && y > 0)
				{
					int leftTopIndex = (x  - 1) + (y - 1) * mRow;
					Node leftTopNode = mNodes[leftTopIndex];
					if(leftTopNode != null && !testRayHit (currNode.mPoint, leftTopNode.mPoint))
					{
						NodeLink neighbourLink = new NodeLink(leftTopIndex,diagonalCost);
						NodeLink currLink = new NodeLink(currIndex, diagonalCost);
						currNode.mNeighbourIndexCostList.Add(neighbourLink);
						leftTopNode.mNeighbourIndexCostList.Add(currLink);
					}
				}
				//check for left 
				if(y > 0)
				{
					int topIndex = x + (y - 1) * mRow;
					Node topNodes = mNodes[topIndex];
					if(topNodes != null && !testRayHit (currNode.mPoint, topNodes.mPoint))
					{
						NodeLink neighbourLink = new NodeLink(topIndex,cellWidth);
						NodeLink currLink = new NodeLink(currIndex, cellWidth);
						currNode.mNeighbourIndexCostList.Add(neighbourLink);
						topNodes.mNeighbourIndexCostList.Add(currLink);
					}
				}
				//check for right 
				/*if(y < mCol - 1)
				{
					int top = x + (y + 1) * mRow;
					Node topNodes = mNodes[top];
					if(topNodes != null && !testRayHit (currNode.mPoint, topNodes.mPoint))
					{
						NodeLink neighbourLink = new NodeLink(top,cellWidth);
						NodeLink currLink = new NodeLink(currIndex, cellWidth);
						currNode.mNeighbourIndexCostList.Add(neighbourLink);
						topNodes.mNeighbourIndexCostList.Add(currLink);
					}
				}*/
				//check for top right
				if(x > 0 && y < mCol - 1)
				{
					int topRightIndex = (x - 1) + (y + 1) * mRow;
					Node topRightNodes = mNodes[topRightIndex];
					if(topRightNodes != null && !testRayHit (currNode.mPoint, topRightNodes.mPoint))
					{
						NodeLink neighbourLink = new NodeLink(topRightIndex,diagonalCost);
						NodeLink currLink = new NodeLink(currIndex, diagonalCost);
						currNode.mNeighbourIndexCostList.Add(neighbourLink);
						topRightNodes.mNeighbourIndexCostList.Add(currLink);
					}
				}
				//check for top
				if(x > 0)
				{
					int rightIndex = (x - 1) + y * mRow;
					Node rightNodes = mNodes[rightIndex];
					if(rightNodes != null && !testRayHit (currNode.mPoint, rightNodes.mPoint))
					{
						NodeLink neighbourLink = new NodeLink(rightIndex,cellHeight);
						NodeLink currLink = new NodeLink(currIndex, cellHeight);
						currNode.mNeighbourIndexCostList.Add(neighbourLink);
						rightNodes.mNeighbourIndexCostList.Add(currLink);
					}
				}
			}
		}

		Debug.Log("Done!");
	}

	public Node findNearestNode(Vector3 point)
	{
		// NOTE: We ignore Y values. Treat everything as 2D since we are operating on a 2D plane.
		Bounds bounds = collider.bounds;
		float cellHeight = bounds.size.x / mRow;
		float cellWidth = bounds.size.z / mCol;
		int col = (int)((point.x - bounds.min.x) / cellHeight);
		int row = (int)((point.z - bounds.min.z) / cellWidth);
		int nodeIndex = col + row * mRow;
		Node foundNode = mNodes[nodeIndex];
		if (foundNode != null && foundNode.mNeighbourIndexCostList.Count > 0) return foundNode;

		// no valid node found. search outwards.
		int colStart = col - 1;
		int colEnd = col + 2;
		int rowStart = row - 1;
		int rowEnd = row + 2;

		// only stop search once we have exausted the whole grid.
		while (
			colStart >= 0 || colEnd < mRow ||
			rowStart >= 0 || rowEnd < mCol)
		{
			float shortestFoundSqrDist = Mathf.Infinity;
			foundNode = null;

			for (int i = colStart; i < colEnd; ++i)
			{
				// skip when out of bound.
				if (i < 0 || i >= mRow) continue;

				for (int j = rowStart; j < rowEnd; ++j)
				{
					// skip when out of bound.
					if (j < 0 || j >= mCol) continue;
					// skip non border check. Last iteration would have already checked it.
					if (j > rowStart && j < (rowEnd - 1) &&
						i > colStart && i < (colEnd - 1)) continue;
					nodeIndex = i + j * mRow;

					Node tmpNode = mNodes[nodeIndex];
					if (tmpNode != null && tmpNode.mNeighbourIndexCostList.Count > 0)
					{
						float tmpSqrDist = (tmpNode.mPoint - point).sqrMagnitude;
						if (tmpSqrDist < shortestFoundSqrDist)
						{
							foundNode = tmpNode;
							shortestFoundSqrDist = tmpSqrDist;
						}
					}
				}
			}

			if (foundNode != null) return foundNode;

			--colStart;
			--rowStart;
			++colEnd;
			++rowEnd;
		}

		return null;
	}

	// returns heuristic cost from start node to end node.
	public float getHeuristicCost(Node start, Node end)
	{
		// let's use Euclidean distance.
		return Vector3.Distance(start.mPoint, end.mPoint);
	}

	// for a-star search use.
	class SearchNode
	{
		public Node node;
		public SearchNode parent;
		public float priority;
		public float costFromStart;
		public float heuristic;
			
		public SearchNode(Node node, SearchNode parent, float costFromStart, float heuristic)
		{
			this.node = node;
			this.parent = parent;
			this.priority = costFromStart + heuristic;
			this.costFromStart = costFromStart;
			this.heuristic = heuristic;
		}
	}

	// helper function to get search node from list by node.
	// NOTE: Not optimal. should specialize for LinkedList(return LinkedListNode) and List(return index) implementation.
	private SearchNode getSearchNodeInList(Node node, ICollection<SearchNode> list)
	{
		foreach (SearchNode searchNode in list)
			if (searchNode.node == node) return searchNode;
		return null;
	}

	public bool findPath(Vector3 start, Vector3 end, out List<Node> result)
	{
		result = new List<Node>();
		
		Node startNode = findNearestNode(start);
		Node endNode = findNearestNode(end);
		
		/*Debug.Log ("Index: " + startNode.mIndex);
		Debug.Log ("Position: " + startNode.mPoint);
		Debug.Log ("Index: " + endNode.mIndex);
		Debug.Log ("Position: " + endNode.mPoint);*/
		
		// make sure we have start and end node.
		if (startNode == null || endNode == null)
		{
			result = null;
			return false;
		}
		
		// TODO! Find path using A-Star Algorithm and put result into the output list.
		
		LinkedList<SearchNode> openList = new LinkedList<SearchNode>();
		List<SearchNode> closeList = new List<SearchNode>();
		openList.AddLast(new SearchNode(startNode, null, 0.0f , 0.0f));

		while(openList != null && openList.Last.Value.node != endNode)
		{
			SearchNode currSearchNode = openList.Last.Value;
			openList.RemoveLast();
			closeList.Add(currSearchNode);
			
			for(int i = 0; i < currSearchNode.node.mNeighbourIndexCostList.Count; i++)
			{
				float cost = currSearchNode.costFromStart + currSearchNode.node.mNeighbourIndexCostList[i].cost;
				int neighbourIndex = currSearchNode.node.mNeighbourIndexCostList[i].neighbourIndex;
				Node neighbourNode = mNodes[neighbourIndex];
				SearchNode neighbourSearchNode = new SearchNode(neighbourNode, null, cost, getHeuristicCost (neighbourNode, endNode));

				/*if(openList.Contains(neighbourSearchNode) && cost < neighbourSearchNode.costFromStart)
				{
					neighbourSearchNode.parent = currSearchNode;
				}
				if (closeList.Contains(neighbourSearchNode) && cost < neighbourSearchNode.costFromStart)
				{
					closeList.Remove (neighbourSearchNode);
				}
				if(!openList.Contains(neighbourSearchNode) && !closeList.Contains(neighbourSearchNode))
				{
					openList.AddLast (neighbourSearchNode);
					openList.Last.Value.parent = currSearchNode;
				}*/
				
				if(openList.Any (x => neighbourSearchNode.node == x.node && cost < x.costFromStart))
				{
					neighbourSearchNode.parent = currSearchNode;
				}
				if (closeList.Any (x => neighbourSearchNode.node == x.node && cost < x.costFromStart))
				{
					closeList.Remove (neighbourSearchNode);
				}
				if(!openList.Any (x => neighbourSearchNode.node == x.node) && !closeList.Any (x => neighbourSearchNode.node == x.node))
				{
					openList.AddLast (neighbourSearchNode);
					openList.Last.Value.parent = currSearchNode;
				}
			}
			if(openList.Count == 0)
			{
				result = null;
				Debug.Log ("Couldn't find path..");
				return false;
			}
			else if(openList.Count > 0)
			{
				SortLinkedList (ref openList);
			}
		}
		
		if(openList.Last.Value.node == endNode) 
		{
			result.Add(endNode);
			SearchNode ParentSeachNode = openList.Last.Value.parent;
			
			while(ParentSeachNode != null)
			{
				result.Add (ParentSeachNode.node);
				ParentSeachNode = ParentSeachNode.parent;
			}
			result.Reverse ();
			IsMove = true;
		}
		result = result;
		return true;
	}
	
	void SortLinkedList(ref LinkedList<SearchNode> list)
	{
		int count = list.Count;
			
		List<SearchNode> SortedList = new List<SearchNode>();
		SortedList = list.OrderBy(searchNode => searchNode.priority).ToList ();
		
		for(int i = 0; i < count; i++)
		{
			//Debug.Log(i + " : " + list.Last.Value.priority);
			list.RemoveLast ();
		}
		for(int i = 0; i < count; i++)
		{
			//Debug.Log(SortedList[i].priority);
		}
		for(int i = 0; i < count; i++)
		{
			list.AddFirst(SortedList[i]);
			/*Debug.Log("===============================");
			Debug.Log(list.First.Value.node.mIndex);
			Debug.Log(i + " : " + list.First.Value.priority);
			Debug.Log("Cost : " + list.First.Value.costFromStart);
			Debug.Log("H : " + list.First.Value.heuristic);
			Debug.Log("Position : " + list.First.Value.node.mPoint);*/
		}
		//Debug.Log("H : " + Player.transform.position);
	}
	
	void MovePlayer(Vector3 dir, float Speed)
	{
		//Vector3 dir = endPos - startPos;
		//Debug.Log (dir);
		Player.transform.Translate (dir * Time.deltaTime * Speed, Space.World);
	}
	
	void OnDestroy()
	{
		foreach (Node node in mNodes)
			if (node != null) node.mNeighbourIndexCostList.Clear();
	}

	void OnDrawGizmos()
	{
		if(!mIsDrawGizmo) return;
		
		Gizmos.color = Color.cyan;
		foreach (Node node in mNodes)
		{
			if (node == null || node.mNeighbourIndexCostList.Count == 0) continue;
			Gizmos.DrawIcon(node.mPoint, "node.icon.png");

			List<NodeLink> neighbourIndexCostList = node.mNeighbourIndexCostList;
			foreach (NodeLink nodeLink in neighbourIndexCostList)
			{
				Node neighbour = mNodes[nodeLink.neighbourIndex];
				Gizmos.DrawLine(node.mPoint, neighbour.mPoint);
			}
		}
	}
	
	// Set destination. Returns a list of vector3 pos to get there.
	public List<Vector3> SetDestination(GameObject go, Vector3 dest)
	{
		findPath (go.transform.position, dest, out pathFindList);
		
		List<Vector3> pathList = new List<Vector3>();
		for(int i = 0; i < pathFindList.Count - 1; i++)
		{
			pathList.Add(pathFindList[i].mPoint);
		}
		return pathList;
	}
	
	/*void Update() 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			findPath (Player.transform.position, Destination, out pathFindList);
		}
		
		if(IsMove)
		{
			nextPosition = pathFindList[Index].mPoint;
			Debug.Log (nextPosition);
			MovePlayer (Player.transform.position, nextPosition, 3.0f);
			IsMove = true;
			MovePlayer((mSimpleController.getDirection(nextPosition, out IsNextIndex)), 2.0f);
			if(IsNextIndex)
			{
				IsNextIndex = false;
				if(Index < pathFindList.Count - 1) Index += 1;
				else 
				{
					Index = 0;
					IsMove = false;
				}
			}
		}
	}*/
}