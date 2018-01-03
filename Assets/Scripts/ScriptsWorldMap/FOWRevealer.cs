using UnityEngine;
using System.Collections.Generic;

public class FOWRevealer : MonoBehaviour 
{
	public int initRadius = 3;
	public float walkRadius = 2;
	public float eagleEyeRadius = 3;
	public float initRevealSpeed = 0.2f;
	public float initRevealTime = 2.0f;
	public float walkRevealSpeed = 0.01f;
	public bool isEnabled = true;
	public bool isDrawGizmo = false;
	public bool isEagleEye = false;
	
	GameObject FOW;
	Mesh mFOWMesh;
	Color[] mColors;
	LayerMask mLayerMask = 1 << 9;
	
	Vector3 mCurPos, mPrevPos = new Vector3(-1, -1, -1);
	
	float mRevealTimer = 0.0f;
	float mRayCastHeight = 2.5f;
	float mRevealSpeed;
	bool mIsInitalFOW = true;
	bool mIsRevealFOW = false;
	List<int> mVerticesList = new List<int>();
	
	float DEFAULT_WALK_RADIUS;

	void Start () 
	{
		FOW = GameObject.FindGameObjectWithTag("FOW");
		mFOWMesh = FOW.GetComponent<MeshFilter>().mesh;
		mColors = FOW.GetComponent<FOWScript>().colors;
		
		DEFAULT_WALK_RADIUS = walkRadius;
	}
	
	void Update () 
	{
		mCurPos = transform.position;
		if(mCurPos != mPrevPos)
		{
			if(isEnabled)
			{
				RevealFOW();
				mIsRevealFOW = true;
			}
		}
		
		if(mIsRevealFOW)
		{
			if(mIsInitalFOW) 
			{
				mRevealTimer += Time.deltaTime;
				if(mRevealTimer > initRevealTime) mIsInitalFOW = false;
			}
			
			for(int i = 0; i < mVerticesList.Count; i++)
			{
				int vertexIndex = mVerticesList[i];
				
				if (mColors[vertexIndex].a > 0) 
				{ mColors[vertexIndex].a -= Time.deltaTime * mRevealSpeed; }
				
				if(mColors[vertexIndex].a <= 0) 
				{
					mVerticesList.RemoveAt(i);
					i--;
				}
			}
			mFOWMesh.colors = mColors;

			if(mVerticesList.Count == 0) mIsRevealFOW = false;

			mPrevPos = mCurPos;
		}
	}
	
	public void BeginInitialFOW()
	{
		mIsInitalFOW = true;
		mRevealTimer = 0.0f;
		mVerticesList.Clear ();
		RevealFOW();
	}
	
	public void RevealFOW()
	{
		float x = transform.position.x;
		float z = transform.position.z;
		
		int segments = 32;
		float segmentAngle = (2.0f * Mathf.PI) / (float)segments;
		
		if(!mIsInitalFOW) RevealOuterFOW(x, z, segments, segmentAngle);
		else if (mIsInitalFOW) RevealInitialFOW(x, z, segments, segmentAngle);
	}
	
	public void CacheNewFOW()
	{
		GameObject newFOW = GameObject.FindGameObjectWithTag("FOW");
		mFOWMesh = newFOW.GetComponent<MeshFilter>().mesh;
		mColors = newFOW.GetComponent<FOWScript>().colors;
		FOW = newFOW;
		mIsInitalFOW = true;
		mVerticesList.Clear ();
	}
	
	public void ResetColor()
	{
		mColors = FOW.GetComponent<FOWScript>().colors;
	}
	
	// Reveal everything within the circle radius.
	void RevealInitialFOW(float x, float z, int segments, float segmentAngle)
	{
		int maxRadius = initRadius;
		
		for (int currRadius = 0; currRadius <= maxRadius; currRadius += 1)
		{
			for (int i = 0; i <= segments; i++)
			{
				float angle = segmentAngle * i;
				RevealFogOFWarAt(x + (Mathf.Cos(angle) * currRadius), z + (Mathf.Sin(angle) * currRadius));
			}
		}
		mRevealSpeed = initRevealSpeed;
		mIsRevealFOW = true;
	}
	
	// Reveal outer FOW while moving.
	void RevealOuterFOW(float x, float z, int segments, float segmentAngle)
	{
		if(!isEagleEye) walkRadius = DEFAULT_WALK_RADIUS;
		else if(isEagleEye) walkRadius = eagleEyeRadius;
		
		for (int i = 0; i <= segments; i++)
		{
			float angle = segmentAngle * i;
			RevealFogOFWarAt(x + (Mathf.Cos(angle) * walkRadius), z + (Mathf.Sin(angle) * walkRadius));
		}
		mRevealSpeed = walkRevealSpeed;
		mIsRevealFOW = true;
	}
	
	// Raycast in a circular manner,  
	// adding each vertices to a list which will be updated later.
	void RevealFogOFWarAt(float x, float z)
	{
		Ray ray = new Ray(new Vector3(x, transform.position.y + mRayCastHeight, z), -transform.up);
		RaycastHit hit;
		float mRayCastDistance = 2.5f;
		
		if(isDrawGizmo) 
		{ Debug.DrawRay(new Vector3(x, transform.position.y + mRayCastHeight, z), (-Vector3.up) * mRayCastDistance, Color.red); }
		
		if(Physics.Raycast (ray, out hit, mRayCastDistance, mLayerMask))
		{
			int[] trianglesArray = mFOWMesh.triangles;
			
			for(int i = 0; i < 3; i++)
			{
				int vertex = trianglesArray[hit.triangleIndex * 3 + i];
				mVerticesList.Add(vertex);
			}
		}
	}
}
