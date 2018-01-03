using UnityEngine;
using System.Collections.Generic;

public class FOWScript : MonoBehaviour 
{
	public Material material;
	public int maxRow = 11;
	public int maxCol = 11;
	public float triangleSize = 1.0f;
	public float heightOffset = 0.0f;
	
	[HideInInspector]
	public Mesh mesh;
	[HideInInspector]
	public Color[] colors;
	
	FOWRevealer mFOWRevealer;
	
	void Awake () 
	{
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent("MeshRenderer");
		mesh = gameObject.GetComponent<MeshFilter>().mesh;
	
		// vertices ---------------------------------------------------------------------------------
		
		List<Vector3> vertPosList = new List<Vector3>();
		
		for (int currRow = 0; currRow < maxRow; currRow++)
		{
			for (int currCol = 0; currCol < maxCol; currCol++)
			{
				vertPosList.Add(new Vector3(currCol * triangleSize, heightOffset, currRow * triangleSize));
			}
		}
		mesh.vertices = vertPosList.ToArray();

		// uv's ----------------------------------------------------------------------------------------
		
		Vector2[] uvsArray = new Vector2[vertPosList.Count];
	
		for (int i = 0; i < uvsArray.Length; i++) 
		{ uvsArray[i] = new Vector2(vertPosList[i].x, vertPosList[i].z); }
		mesh.uv = uvsArray;
	
		// triangles ---------------------------------------------------------------------------------
		
		List<int> triangleList = new List<int>();
		for (int currRow = 0; currRow < maxRow - 1; currRow++)
		{
			for (int currCol = 0; currCol < maxCol - 1; currCol++)
			{
				int vertBelow = currCol + (currRow * maxCol);
				int vertAbove = currCol + ((currRow + 1) * maxCol);
				
				//	vertAbove
				//	|------/
				//	|    /  
				//	|  /
				//	|/
				//	vertBelow
				// Top left triangle.
				triangleList.Add(vertBelow);
				triangleList.Add(vertAbove);
				triangleList.Add(vertAbove + 1);
				
				//	vertAbove
				//		   /|
				//	     /  | 
				//	   /    |
				//	 /------|
				//	vertBelow
				// Btm right triangle.
				triangleList.Add(vertBelow);
				triangleList.Add(vertAbove + 1);
				triangleList.Add(vertBelow + 1);
			}
		}
		
		mesh.triangles = triangleList.ToArray ();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
		// vertex colors ------------------------------------------------------------------------------
		
		int vertTotal = mesh.vertices.Length;
		colors = new Color[vertTotal];

		for (int i = 0; i < vertTotal; i++)
		{
			colors[i] = new Color(0, 0, 0, 1);
		}
		mesh.colors = colors;

		renderer.castShadows = false;
		renderer.receiveShadows = false;
		renderer.material = material;
		
		gameObject.AddComponent<MeshCollider>();
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	
	void Start()
	{
		mFOWRevealer = GameObject.FindGameObjectWithTag ("Player").GetComponent<FOWRevealer>();
	}
	
	public void DefaultFOW()
	{
		int vertTotal = mesh.vertices.Length;
		colors = new Color[vertTotal];

		for (int i = 0; i < vertTotal; i++)
		{
			colors[i] = new Color(0, 0, 0, 1);
		}
		mesh.colors = colors;
		
		mFOWRevealer.ResetColor ();
	}
}
